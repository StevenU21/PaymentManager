using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UserFormViewModel : BaseFormViewModel<User>
    {
        private readonly IUserService _userService;

        public User User
        {
            get => Entity!;
            set => Entity = value;
        }

        public UserFormViewModel(
            IUserService userService,
            IValidationService<User> userValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : base(userValidationService, messagingService, navigation)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            User = new User();
        }

        public UserFormViewModel(
            User user,
            IUserService userService,
            IValidationService<User> userValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : this(userService, userValidationService, messagingService, navigation)
        {
            User = user;
        }

        protected override async Task SaveOrUpdateAsync()
        {
            if (User.Id != 0)
                await _userService.UpdateAsync(User);
            else
                await _userService.AddAsync(User);
        }

        protected override bool GetIsEdit() => User.Id != 0;
    }
}