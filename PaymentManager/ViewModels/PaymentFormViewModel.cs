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

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<PaymentPlan> PaymentPlans { get; } = new();
        public ObservableCollection<PaymentMethod> PaymentMethods { get; } = new();

        private User? selectedUser;
        public User? SelectedUser
        {
            get => Users.FirstOrDefault(u => u.Id == Payment.UserId) ?? selectedUser;
            set
            {
                if (value != null)
                {
                    selectedUser = value;
                    Payment.UserId = value.Id;
                }
            }
        }

        private PaymentPlan? selectedPaymentPlan;
        public PaymentPlan? SelectedPaymentPlan
        {
            get => PaymentPlans.FirstOrDefault(pp => pp.Id == Payment.PaymentPlanId) ?? selectedPaymentPlan;
            set
            {
                if (value != null)
                {
                    selectedPaymentPlan = value;
                    Payment.PaymentPlanId = value.Id;
                    SetNextDueDateFromPlan(value);
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

        public PaymentFormViewModel(
            IPaymentService paymentService,
            IValidationService<Payment> paymentValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            IPaymentMethodService paymentMethodService,
            INavigation navigation
        )
            : base(paymentValidationService, messagingService, navigation)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            Payment = new Payment
            {
                PaymentDate = DateTime.Now
            };
            LoadCombos();
        }

        public PaymentFormViewModel(
            Payment payment,
            IPaymentService paymentService,
            IValidationService<Payment> paymentValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            IPaymentMethodService paymentMethodService,
            INavigation navigation
        )
            : this(paymentService, paymentValidationService, messagingService, userService, paymentPlanService, paymentMethodService, navigation)
        {
            Payment = payment;
            selectedUser = Users.FirstOrDefault(u => u.Id == payment.UserId);
            selectedPaymentPlan = PaymentPlans.FirstOrDefault(pp => pp.Id == payment.PaymentPlanId);
            selectedPaymentMethod = PaymentMethods.FirstOrDefault(pm => pm.Id == payment.PaymentMethodId);
        }

        private async void LoadCombos()
        {
            var users = await _userService.GetAllAsync();
            Users.Clear();
            foreach (var u in users) Users.Add(u);

            var plans = await _paymentPlanService.GetAllAsync();
            PaymentPlans.Clear();
            foreach (var p in plans) PaymentPlans.Add(p);

            var methods = await _paymentMethodService.GetAllAsync();
            PaymentMethods.Clear();
            foreach (var m in methods) PaymentMethods.Add(m);

            if (Payment.UserId != 0)
                selectedUser = Users.FirstOrDefault(u => u.Id == Payment.UserId);
            if (Payment.PaymentPlanId != null)
                selectedPaymentPlan = PaymentPlans.FirstOrDefault(pp => pp.Id == Payment.PaymentPlanId);
            if (Payment.PaymentMethodId != null)
                selectedPaymentMethod = PaymentMethods.FirstOrDefault(pm => pm.Id == Payment.PaymentMethodId);

            if (SelectedPaymentPlan != null)
            {
                SetNextDueDateFromPlan(SelectedPaymentPlan);
            }
        }

        private void SetNextDueDateFromPlan(PaymentPlan plan)
        {
            var nextDueDate = _paymentService.CalculateNextDueDate(Payment, plan);
            Payment.NextDueDate = nextDueDate ?? default;
        }

        protected override async Task SaveAsync()
        {
            if (SelectedPaymentPlan != null)
            {
                SetNextDueDateFromPlan(SelectedPaymentPlan);
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

            if (SelectedPaymentPlan == null || !SelectedPaymentPlan.DayOfMonth.HasValue)
            {
                await _messagingService.ShowMessageAsync("Error", "Debe seleccionar un plan de pago válido con día de pago.");
                return;
            }

            await base.SaveAsync();
        }

        protected override async Task SaveOrUpdateAsync()
        {
            if (Payment.Id != 0)
                await _paymentService.UpdateAsync(Payment);
            else
                await _paymentService.AddAsync(Payment);
        }

        protected override bool GetIsEdit() => Payment.Id != 0;
    }
}