using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;
using PaymentManager.Validators;

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
            // Validaciones individuales
            var nameError = UserValidator.ValidateName(Name);
            if (nameError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", nameError, "OK");
                return;
            }

            var emailError = UserValidator.ValidateEmail(Email);
            if (emailError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", emailError, "OK");
                return;
            }

            var phoneError = UserValidator.ValidatePhone(Phone);
            if (phoneError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", phoneError, "OK");
                return;
            }

            // Validaciones de unicidad (puedes obtener la lista de usuarios desde el servicio)
            var users = await _userService.GetUsersAsync();
            var uniqueEmailError = UserValidator.ValidateUniqueEmail(Email, users);
            if (uniqueEmailError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", uniqueEmailError, "OK");
                return;
            }

            var uniquePhoneError = UserValidator.ValidateUniquePhone(Phone, users);
            if (uniquePhoneError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", uniquePhoneError, "OK");
                return;
            }

            // Guardar usuario si todo es válido
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