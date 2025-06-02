using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentPlanFormViewModel
    {
        private readonly IValidationService<PaymentPlan> _paymentPlanValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly IUserService _userService;
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly INavigation _navigation;
        public event Action<PaymentPlan>? PaymentPlanSaved;

        public PaymentPlan PaymentPlan { get; set; }

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<PaymentType> PaymentTypes { get; } = new();

        private User? selectedUser;
        public User? SelectedUser
        {
            get => Users.FirstOrDefault(u => u.Id == PaymentPlan.UserId) ?? selectedUser;
            set
            {
                if (value != null)
                {
                    selectedUser = value;
                    PaymentPlan.UserId = value.Id;
                }
            }
        }

        private PaymentType? selectedPaymentType;
        public PaymentType? SelectedPaymentType
        {
            get => PaymentTypes.FirstOrDefault(pt => pt.Id == PaymentPlan.PaymentTypeId) ?? selectedPaymentType;
            set
            {
                if (value != null)
                {
                    selectedPaymentType = value;
                    PaymentPlan.PaymentTypeId = value.Id;
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PaymentPlanFormViewModel(
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentTypeService paymentTypeService,
            INavigation navigation)
        {
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentPlanValidationService = paymentPlanValidationService ?? throw new ArgumentNullException(nameof(paymentPlanValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentTypeService = paymentTypeService ?? throw new ArgumentNullException(nameof(paymentTypeService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PaymentPlan = new PaymentPlan();
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
            LoadUsersAndTypes();
        }

        public PaymentPlanFormViewModel(
            PaymentPlan paymentPlan,
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentTypeService paymentTypeService,
            INavigation navigation)
            : this(paymentPlanService, paymentPlanValidationService, messagingService, userService, paymentTypeService, navigation)
        {
            PaymentPlan = paymentPlan;
            selectedUser = Users.FirstOrDefault(u => u.Id == paymentPlan.UserId);
            selectedPaymentType = PaymentTypes.FirstOrDefault(pt => pt.Id == paymentPlan.PaymentTypeId);
        }

        private async void LoadUsersAndTypes()
        {
            var users = await _userService.GetAllAsync();
            Users.Clear();
            foreach (var u in users) Users.Add(u);

            var types = await _paymentTypeService.GetAllAsync();
            PaymentTypes.Clear();
            foreach (var t in types) PaymentTypes.Add(t);

            // Selecciona automáticamente si ya hay valores
            if (PaymentPlan.UserId != 0)
                selectedUser = Users.FirstOrDefault(u => u.Id == PaymentPlan.UserId);
            if (PaymentPlan.PaymentTypeId != 0)
                selectedPaymentType = PaymentTypes.FirstOrDefault(pt => pt.Id == PaymentPlan.PaymentTypeId);
        }

        private async Task SaveAsync()
        {
            var (isValid, errorMessage) = await _paymentPlanValidationService.ValidateAsync(PaymentPlan, PaymentPlan.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (PaymentPlan.Id != 0)
            {
                await _paymentPlanService.UpdateAsync(PaymentPlan);
                PaymentPlanSaved?.Invoke(PaymentPlan);
            }
            else
            {
                await _paymentPlanService.AddAsync(PaymentPlan);
                PaymentPlanSaved?.Invoke(PaymentPlan);
            }

            await _navigation.PopModalAsync();
        }
    }
}