using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UserFormViewModel
    {
        private readonly IUserService _userService;
        private readonly INavigation _navigation;

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UserFormViewModel(IUserService userService, INavigation navigation)
        {
            _userService = userService;
            _navigation = navigation;
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        private async Task SaveAsync()
        {
            var user = new User
            {
                Name = Name,
                Email = Email,
                Phone = Phone
            };
            await _userService.AddUserAsync(user);
            await _navigation.PopModalAsync();
        }
    }
}