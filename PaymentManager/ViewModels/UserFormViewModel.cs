using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UserFormViewModel
    {
        private readonly IUserValidationService _userValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IUserService _userService;
        private readonly INavigation _navigation;
        public event Action<User>? UserSaved;

        public User User { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserFormViewModel(
            IUserService userService,
            IUserValidationService userValidationService,
            IMessagingService messagingService,
            INavigation navigation)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userValidationService = userValidationService ?? throw new ArgumentNullException(nameof(userValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            User = new User();
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        public UserFormViewModel(
            User user,
            IUserService userService,
            IUserValidationService userValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : this(userService, userValidationService, messagingService, navigation)
        {
            User = user;
        }

        private async Task SaveAsync()
        {
            var (isValid, errorMessage) = await _userValidationService.ValidateAsync(User, User.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (User.Id != 0)
            {
                await _userService.UpdateUserAsync(User);
                UserSaved?.Invoke(User);
            }
            else
            {
                await _userService.AddUserAsync(User);
                UserSaved?.Invoke(User);
            }

            await _navigation.PopModalAsync();
        }
    }
}