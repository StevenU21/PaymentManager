using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UsersViewModel
    {
        private readonly IUserService _userService;

        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        public Command RegisterUserCommand { get; }
        public Command<User> ShowUserCommand { get; }
        public Command<User> EditUserCommand { get; }
        public Command<User> DeleteUserCommand { get; }
        public Command LoadUsersCommand { get; }

        public bool IsBusy { get; set; }

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
            LoadUsersCommand = new Command(async () => await LoadUsersAsync());
            RegisterUserCommand = new Command(async () => await OpenRegisterModal());
            ShowUserCommand = new Command<User>(user => { /* lógica para mostrar */ });
            EditUserCommand = new Command<User>(async user => await OpenEditModal(user));
            DeleteUserCommand = new Command<User>(async user => await DeleteUserAsync(user));
        }

        private async Task LoadUsersAsync()
        {
            IsBusy = true;
            var users = await _userService.GetUsersAsync();
            Users.Clear();
            foreach (var user in users)
                Users.Add(user);
            IsBusy = false;
        }

        private async Task OpenRegisterModal()
        {
            var userFormPage = new Views.UserFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new UserFormViewModel(_userService, mainPage.Navigation);
                viewModel.UserSaved += user =>
                {
                    Users.Add(user); // Solo agrega el nuevo usuario
                };
                userFormPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(userFormPage);
            }
        }

        private async Task OpenEditModal(User user)
        {
            if (user == null) return;
            var userFormPage = new Views.UserFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new UserFormViewModel(user, _userService, mainPage.Navigation);
                viewModel.UserSaved += updatedUser =>
                {
                    var existing = Users.FirstOrDefault(u => u.Id == updatedUser.Id);
                    if (existing != null)
                    {
                        // Solo actualiza las propiedades, no reemplaces el objeto
                        existing.Name = updatedUser.Name;
                        existing.Email = updatedUser.Email;
                        existing.Phone = updatedUser.Phone;
                        // Si agregas más propiedades editables, actualízalas aquí también
                    }
                };
                userFormPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(userFormPage);
            }
        }

        private async Task DeleteUserAsync(User user)
        {
            if (user == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Eliminar usuario",
                $"¿Estás seguro de eliminar a {user.Name}?",
                "Sí", "No");

            if (!confirm) return;

            await _userService.DeleteUserAsync(user.Id);
            Users.Remove(user);
        }
    }
}