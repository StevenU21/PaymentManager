using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UsersViewModel
    {
        private readonly IUserService _userService;

        public ObservableCollection<User> Users { get; } = new();

        public Command RegisterUserCommand { get; }
        public Command<User> ShowUserCommand { get; }
        public Command<User> EditUserCommand { get; }
        public Command<User> DeleteUserCommand { get; }

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
            LoadUsersCommand = new Command(async () => await LoadUsersAsync());
            RegisterUserCommand = new Command(async () => await OpenRegisterModal());
            ShowUserCommand = new Command<User>(user => { /* lógica para mostrar */ });
            EditUserCommand = new Command<User>(user => { /* lógica para editar */ });
            DeleteUserCommand = new Command<User>(user => { /* lógica para eliminar */ });
        }

        public Command LoadUsersCommand { get; }

        public bool IsBusy { get; set; }

        private async Task LoadUsersAsync()
        {
            IsBusy = true;
            Users.Clear();
            var users = await _userService.GetUsersAsync();
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
                userFormPage.BindingContext = new UserFormViewModel(_userService, mainPage.Navigation);
                await mainPage.Navigation.PushModalAsync(userFormPage);
            }
        }
    }
}