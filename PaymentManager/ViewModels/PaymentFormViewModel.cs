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
            INavigation navigation)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _paymentValidationService = paymentValidationService ?? throw new ArgumentNullException(nameof(paymentValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            Payment = new Payment();
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
            INavigation navigation)
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
        }

        private async Task SaveAsync()
        {
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