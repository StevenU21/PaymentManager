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

            if (SelectedUserPaymentPlan != null)
            {
                SetNextDueDateFromUserPaymentPlan(SelectedUserPaymentPlan);
            }
        }

        private void SetNextDueDateFromUserPaymentPlan(UserPaymentPlan userPaymentPlan)
        {
            var nextDueDate = _paymentService.CalculateNextDueDate(Payment, userPaymentPlan.PaymentPlan!);
            Payment.NextDueDate = nextDueDate ?? default;
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