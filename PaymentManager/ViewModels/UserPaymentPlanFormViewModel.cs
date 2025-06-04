using System.Collections.ObjectModel;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UserPaymentPlanFormViewModel : BaseFormViewModel<UserPaymentPlan>
    {
        private readonly IUserPaymentPlanService _userPaymentPlanService;
        private readonly IUserService _userService;
        private readonly IPaymentPlanService _paymentPlanService;

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<PaymentPlan> PaymentPlans { get; } = new();

        private User? selectedUser;
        public User? SelectedUser
        {
            get => Users.FirstOrDefault(u => u.Id == UserPaymentPlan.UserId) ?? selectedUser;
            set
            {
                if (value != null)
                {
                    selectedUser = value;
                    UserPaymentPlan.UserId = value.Id;
                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }

        private PaymentPlan? selectedPaymentPlan;
        public PaymentPlan? SelectedPaymentPlan
        {
            get => PaymentPlans.FirstOrDefault(p => p.Id == UserPaymentPlan.PaymentPlanId) ?? selectedPaymentPlan;
            set
            {
                if (value != null)
                {
                    selectedPaymentPlan = value;
                    UserPaymentPlan.PaymentPlanId = value.Id;
                    OnPropertyChanged(nameof(SelectedPaymentPlan));
                }
            }
        }

        public UserPaymentPlan UserPaymentPlan
        {
            get => Entity!;
            set => Entity = value;
        }

        public UserPaymentPlanFormViewModel(
            IUserPaymentPlanService userPaymentPlanService,
            IValidationService<UserPaymentPlan> userPaymentPlanValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            INavigation navigation)
            : base(userPaymentPlanValidationService, messagingService, navigation)
        {
            _userPaymentPlanService = userPaymentPlanService ?? throw new ArgumentNullException(nameof(userPaymentPlanService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            UserPaymentPlan = new UserPaymentPlan();
            UserPaymentPlan.JoinDate = DateTime.Now;
            UserPaymentPlan.Status = "Pendiente";
            LoadCombos();
        }

        public UserPaymentPlanFormViewModel(
            UserPaymentPlan userPaymentPlan,
            IUserPaymentPlanService userPaymentPlanService,
            IValidationService<UserPaymentPlan> userPaymentPlanValidationService,
            IMessagingService messagingService,
            IUserService userService,
            IPaymentPlanService paymentPlanService,
            INavigation navigation)
            : this(userPaymentPlanService, userPaymentPlanValidationService, messagingService, userService, paymentPlanService, navigation)
        {
            UserPaymentPlan = userPaymentPlan;
            selectedUser = Users.FirstOrDefault(u => u.Id == userPaymentPlan.UserId);
            selectedPaymentPlan = PaymentPlans.FirstOrDefault(p => p.Id == userPaymentPlan.PaymentPlanId);
        }

        private async void LoadCombos()
        {
            var users = await _userService.GetAllAsync();
            Users.Clear();
            foreach (var u in users) Users.Add(u);

            var plans = await _paymentPlanService.GetAllAsync();
            PaymentPlans.Clear();
            foreach (var p in plans) PaymentPlans.Add(p);

            if (UserPaymentPlan.UserId != 0)
                selectedUser = Users.FirstOrDefault(u => u.Id == UserPaymentPlan.UserId);
            if (UserPaymentPlan.PaymentPlanId != 0)
                selectedPaymentPlan = PaymentPlans.FirstOrDefault(p => p.Id == UserPaymentPlan.PaymentPlanId);

            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(PaymentPlans));
            OnPropertyChanged(nameof(SelectedUser));
            OnPropertyChanged(nameof(SelectedPaymentPlan));
        }

        protected override async Task SaveOrUpdateAsync()
        {
            UserPaymentPlan.JoinDate = DateTime.Now;
            UserPaymentPlan.Status = "Pendiente";

            if (UserPaymentPlan.Id != 0)
                await _userPaymentPlanService.UpdateAsync(UserPaymentPlan);
            else
                await _userPaymentPlanService.AddAsync(UserPaymentPlan);

            var actualizado = (await _userPaymentPlanService.GetAllAsync())
                .FirstOrDefault(upp => upp.UserId == UserPaymentPlan.UserId && upp.PaymentPlanId == UserPaymentPlan.PaymentPlanId);

            if (actualizado != null)
            {
                UserPaymentPlan.Status = actualizado.Status;
                UserPaymentPlan.JoinDate = actualizado.JoinDate;
                UserPaymentPlan.LastPaymentDate = actualizado.LastPaymentDate;
                UserPaymentPlan.NextDueDate = actualizado.NextDueDate;
                UserPaymentPlan.Active = actualizado.Active;
                OnPropertyChanged(nameof(UserPaymentPlan));
            }
        }

        protected override bool GetIsEdit() => UserPaymentPlan.Id != 0;
    }
}