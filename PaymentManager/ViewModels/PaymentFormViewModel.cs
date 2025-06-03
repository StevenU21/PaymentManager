using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentFormViewModel
    {
        private readonly IValidationService<Payment> _paymentValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly INavigation _navigation;

        public event Action<Payment>? PaymentSaved;

        public Payment Payment { get; set; }

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
                    _ = SetNextDueDateFromPlanAsync(value);
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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PaymentFormViewModel(
            IPaymentService paymentService,
            IValidationService<Payment> paymentValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            IPaymentMethodService paymentMethodService,
            INavigation navigation
          )
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _paymentValidationService = paymentValidationService ?? throw new ArgumentNullException(nameof(paymentValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            Payment = new Payment
            {
                PaymentDate = DateTime.Now // Asigna la fecha actual por defecto
            };
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
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

            // Calcular NextDueDate si hay un plan seleccionado
            if (SelectedPaymentPlan != null)
            {
                await SetNextDueDateFromPlanAsync(SelectedPaymentPlan);
            }
        }

        private async Task SetNextDueDateFromPlanAsync(PaymentPlan plan)
        {
            // Usar DayOfMonth del PaymentPlan para calcular el próximo vencimiento
            if (plan.DayOfMonth.HasValue)
            {
                var today = DateTime.Now.Date;
                var dayOfMonth = plan.DayOfMonth.Value.Day;

                var nextMonth = today.Month;
                var nextYear = today.Year;

                // Si el día de pago de este mes ya pasó, ir al siguiente mes
                if (today.Day >= dayOfMonth)
                {
                    nextMonth++;
                    if (nextMonth > 12)
                    {
                        nextMonth = 1;
                        nextYear++;
                    }
                }

                // Calcular el número de periodos pagados (meses adelantados)
                int periodsPaid = Payment.PeriodsPaid > 0 ? Payment.PeriodsPaid : 1;
                nextMonth += (periodsPaid - 1);

                // Ajustar año y mes si se pasa de diciembre
                while (nextMonth > 12)
                {
                    nextMonth -= 12;
                    nextYear++;
                }

                // Asegura que el día no exceda el número de días del mes
                var daysInNextMonth = DateTime.DaysInMonth(nextYear, nextMonth);
                var dueDay = Math.Min(dayOfMonth, daysInNextMonth);

                Payment.NextDueDate = new DateTime(nextYear, nextMonth, dueDay, Payment.PaymentDate.Hour, Payment.PaymentDate.Minute, Payment.PaymentDate.Second);
            }
            else
            {
                Payment.NextDueDate = default;
            }
        }

        private async Task SaveAsync()
        {
            // Recalcula NextDueDate antes de guardar
            if (SelectedPaymentPlan != null)
            {
                await SetNextDueDateFromPlanAsync(SelectedPaymentPlan);
            }

            // DEBUG: Imprimir los datos que se intentan guardar
            System.Diagnostics.Debug.WriteLine("=== DEBUG: Intentando guardar pago ===");
            System.Diagnostics.Debug.WriteLine($"UserId: {Payment.UserId}");
            System.Diagnostics.Debug.WriteLine($"PaymentPlanId: {Payment.PaymentPlanId}");
            System.Diagnostics.Debug.WriteLine($"PaymentMethodId: {Payment.PaymentMethodId}");
            System.Diagnostics.Debug.WriteLine($"PaymentDate: {Payment.PaymentDate:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"NextDueDate: {Payment.NextDueDate:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"AmountPaid: {Payment.AmountPaid}");
            System.Diagnostics.Debug.WriteLine($"PeriodsPaid: {Payment.PeriodsPaid}");
            System.Diagnostics.Debug.WriteLine("===============================");

            // Validar que NextDueDate sea válido y que PaymentDate también lo sea
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

            // Validar que el plan y el día de pago existen
            if (SelectedPaymentPlan == null || !SelectedPaymentPlan.DayOfMonth.HasValue)
            {
                await _messagingService.ShowMessageAsync("Error", "Debe seleccionar un plan de pago válido con día de pago.");
                return;
            }

            var (isValid, errorMessage) = await _paymentValidationService.ValidateAsync(Payment, Payment.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (Payment.Id != 0)
            {
                await _paymentService.UpdateAsync(Payment);
                PaymentSaved?.Invoke(Payment);
            }
            else
            {
                await _paymentService.AddAsync(Payment);
                PaymentSaved?.Invoke(Payment);
            }

            await _navigation.PopModalAsync();
        }
    }
}