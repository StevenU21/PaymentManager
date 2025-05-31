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
        public event Action<User>? UserSaved;

        public User User { get; set; }  // Referencia directa al usuario (para edición)

        public string Name
        {
            get => User != null ? User.Name : _name;
            set
            {
                if (User != null)
                    User.Name = value;
                else
                    _name = value;
            }
        }
        private string _name = string.Empty;

        public string Email
        {
            get => User != null ? User.Email : _email;
            set
            {
                if (User != null)
                    User.Email = value;
                else
                    _email = value;
            }
        }
        private string _email = string.Empty;

        public string Phone
        {
            get => User != null ? User.Phone ?? string.Empty : _phone;
            set
            {
                if (User != null)
                    User.Phone = value;
                else
                    _phone = value;
            }
        }
        private string _phone = string.Empty;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor para nuevo usuario
        public UserFormViewModel(IUserService userService, INavigation navigation)
        {
            _userService = userService;
            _navigation = navigation;
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        // Constructor para edición (recibe el objeto original)
        public UserFormViewModel(User user, IUserService userService, INavigation navigation)
            : this(userService, navigation)
        {
            User = user;
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

            var users = await _userService.GetUsersAsync();
            var usersToValidate = User != null
                ? users.Where(u => u.Id != User.Id).ToList()
                : users;

            var uniqueEmailError = UserValidator.ValidateUniqueEmail(Email, usersToValidate);
            if (uniqueEmailError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", uniqueEmailError, "OK");
                return;
            }

            var uniquePhoneError = UserValidator.ValidateUniquePhone(Phone, usersToValidate);
            if (uniquePhoneError != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", uniquePhoneError, "OK");
                return;
            }

            if (User != null)
            {
                // Actualizar usuario existente (ya editaste el objeto original)
                await _userService.UpdateUserAsync(User);
                UserSaved?.Invoke(User);
            }
            else
            {
                // Guardar usuario nuevo
                var newUser = new User
                {
                    Name = _name,
                    Email = _email,
                    Phone = _phone
                };
                await _userService.AddUserAsync(newUser);
                UserSaved?.Invoke(newUser);
            }

            await _navigation.PopModalAsync();
        }
    }
}