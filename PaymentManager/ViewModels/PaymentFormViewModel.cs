using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentFormViewModel : BaseFormViewModel<Payment>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IUserPaymentPlanService _userPaymentPlanService;

        public ObservableCollection<UserPaymentPlan> UserPaymentPlans { get; } = new();
        public ObservableCollection<PaymentMethod> PaymentMethods { get; } = new();

        private UserPaymentPlan? selectedUserPaymentPlan;
        public UserPaymentPlan? SelectedUserPaymentPlan
        {
            get => UserPaymentPlans.FirstOrDefault(upp => upp.Id == Payment.UserPaymentPlanId) ?? selectedUserPaymentPlan;
            set
            {
                if (value != null)
                {
                    selectedUserPaymentPlan = value;
                    Payment.UserPaymentPlanId = value.Id;
                    SetNextDueDateFromUserPaymentPlan(value);
                }
            }
        }

        private PaymentMethod? selectedPaymentMethod;
        public PaymentMethod? SelectedPaymentMethod
        {
            get => PaymentMethods.FirstOrDefault(pm => pm.Id == Payment.PaymentMethodId) ?? selectedPaymentMethod;
            set
            {
                if (value != null)
                {
                    selectedPaymentMethod = value;
                    Payment.PaymentMethodId = value.Id;
                }
            }
        }

        public Payment Payment
        {
            get => Entity!;
            set => Entity = value;
        }

        public decimal AmountPaid
        {
            get => Payment.AmountPaid;
            set
            {
                if (Payment.AmountPaid != value)
                {
                    Payment.AmountPaid = value;
                    OnPropertyChanged();
                }
            }
        }

        public PaymentFormViewModel(
            IPaymentService paymentService,
            IValidationService<Payment> paymentValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            IPaymentMethodService paymentMethodService,
            IUserPaymentPlanService userPaymentPlanService,
            INavigation navigation
        )
            : base(paymentValidationService, messagingService, navigation)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            _userPaymentPlanService = userPaymentPlanService ?? throw new ArgumentNullException(nameof(userPaymentPlanService));
            Payment = new Payment
            {
                PaymentDate = DateTime.Now
            };
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Payment.AmountPaid) || e.PropertyName == nameof(SelectedUserPaymentPlan))
                {
                    UpdatePeriodsPaid();
                }
            };
        }

        public PaymentFormViewModel(
            Payment payment,
            IPaymentService paymentService,
            IValidationService<Payment> paymentValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            IPaymentMethodService paymentMethodService,
            IUserPaymentPlanService userPaymentPlanService,
            INavigation navigation
        )
            : this(paymentService, paymentValidationService, messagingService, userService, paymentPlanService, paymentMethodService, userPaymentPlanService, navigation)
        {
            Payment = payment;
        }

        public async Task LoadCombosAsync()
        {
            var userPaymentPlans = await _paymentService.GetAllUserPaymentPlansAsync();
            UserPaymentPlans.Clear();
            foreach (var upp in userPaymentPlans) UserPaymentPlans.Add(upp);

            var methods = await _paymentMethodService.GetAllAsync();
            PaymentMethods.Clear();
            foreach (var m in methods) PaymentMethods.Add(m);

            if (Payment.UserPaymentPlanId != 0)
                selectedUserPaymentPlan = UserPaymentPlans.FirstOrDefault(upp => upp.Id == Payment.UserPaymentPlanId);
            if (Payment.PaymentMethodId != null)
                selectedPaymentMethod = PaymentMethods.FirstOrDefault(pm => pm.Id == Payment.PaymentMethodId);
        }

        private void SetNextDueDateFromUserPaymentPlan(UserPaymentPlan userPaymentPlan)
        {
            if (userPaymentPlan.PaymentPlan == null)
                return;

            var plan = userPaymentPlan.PaymentPlan;
            int dayOfMonth = plan.DayOfMonthToPay;
            int periodos = Payment.PeriodsPaid > 0 ? Payment.PeriodsPaid : 1;
            var fechaPago = Payment.PaymentDate;

            DateTime proximaFechaPago;
            if (fechaPago.Day < dayOfMonth)
            {
                proximaFechaPago = new DateTime(fechaPago.Year, fechaPago.Month, dayOfMonth);
            }
            else
            {
                var siguienteMes = fechaPago.AddMonths(1);
                proximaFechaPago = new DateTime(siguienteMes.Year, siguienteMes.Month, dayOfMonth);
            }

            proximaFechaPago = proximaFechaPago.AddMonths(periodos - 1);
            Payment.NextDueDate = proximaFechaPago;
        }

        private void UpdatePeriodsPaid()
        {
            if (SelectedUserPaymentPlan?.ShareAmount > 0)
            {
                var periods = (int)(Payment.AmountPaid / SelectedUserPaymentPlan.ShareAmount);
                Payment.PeriodsPaid = (ushort)Math.Max(1, periods);
            }
            else
            {
                Payment.PeriodsPaid = 1;
            }
            OnPropertyChanged(nameof(Payment));
        }

        protected override async Task SaveAsync()
        {
            if (SelectedUserPaymentPlan != null)
            {
                SetNextDueDateFromUserPaymentPlan(SelectedUserPaymentPlan);
            }

            if (Payment.NextDueDate == default || Payment.NextDueDate < new DateTime(2000, 1, 1))
            {
                await _messagingService.ShowMessageAsync("Error", "La fecha de próximo vencimiento (NextDueDate) no es válida.");
                return;
            }
            if (Payment.PaymentDate == default || Payment.PaymentDate < new DateTime(2000, 1, 1))
            {
                await _messagingService.ShowMessageAsync("Error", "La fecha de pago no es válida.");
                return;
            }

            await base.SaveAsync();

            await LoadCombosAsync();
            OnPropertyChanged(nameof(SelectedUserPaymentPlan));
            OnPropertyChanged(nameof(SelectedPaymentMethod));
        }

        protected override async Task SaveOrUpdateAsync()
        {
            UpdatePeriodsPaid();

            if (SelectedUserPaymentPlan != null && SelectedUserPaymentPlan.PaymentPlan != null)
            {
                var pagosPrevios = SelectedUserPaymentPlan.Payments?.Sum(p => p.AmountPaid) ?? 0;
                var totalPagado = pagosPrevios + Payment.AmountPaid;
                var cuota = SelectedUserPaymentPlan.ShareAmount;

                if (totalPagado >= cuota)
                {
                    SelectedUserPaymentPlan.Status = "Pagado";
                    SetNextDueDateFromUserPaymentPlan(SelectedUserPaymentPlan);
                }
                else
                {
                    SelectedUserPaymentPlan.Status = "Pendiente";
                }
                SelectedUserPaymentPlan.LastPaymentDate = Payment.PaymentDate;
                SelectedUserPaymentPlan.NextDueDate = Payment.NextDueDate;
                await _userPaymentPlanService.UpdateAsync(SelectedUserPaymentPlan);
            }

            if (Payment.Id != 0)
                await _paymentService.UpdateAsync(Payment);
            else
                await _paymentService.AddAsync(Payment);

            await LoadCombosAsync();
            OnPropertyChanged(nameof(SelectedUserPaymentPlan));
            OnPropertyChanged(nameof(SelectedPaymentMethod));
        }

        protected override bool GetIsEdit() => Payment.Id != 0;
    }
}