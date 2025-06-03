using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UsersViewModel : BaseListViewModel<User>
    {
        private readonly IMessagingService _messagingService;
        private readonly IUserService _userService;
        private readonly IValidationService<User> _userValidationService;

        public ObservableCollection<User> Users => Items;
        public ICommand RegisterUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public UsersViewModel(
            IUserService userService,
            IValidationService<User> userValidationService,
            IMessagingService messagingService)
            : base()
        {
            _userService = userService;
            _userValidationService = userValidationService;
            _messagingService = messagingService;
            RegisterUserCommand = new Command(async () => await OpenRegisterModal());
            EditUserCommand = new Command<User>(async user => await OpenEditModal(user));
            DeleteUserCommand = new Command<User>(async user => await DeleteUserAsync(user));
        }

        protected override async Task<IEnumerable<User>> GetItemsAsync()
        {
            return await _userService.GetAllAsync();
        }

        private async Task OpenRegisterModal()
        {
            var userFormPage = new Views.UserFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new UserFormViewModel(_userService, _userValidationService, _messagingService, mainPage.Navigation);
                viewModel.EntitySaved += user =>
                {
                    Users.Add(user);
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
                var viewModel = new UserFormViewModel(user, _userService, _userValidationService, _messagingService, mainPage.Navigation);
                viewModel.EntitySaved += updatedUser =>
                {
                    var existing = Users.FirstOrDefault(u => u.Id == updatedUser.Id);
                    if (existing != null)
                    {
                        existing.Name = updatedUser.Name;
                        existing.Email = updatedUser.Email;
                        existing.Phone = updatedUser.Phone;
                    }
                };
                userFormPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(userFormPage);
            }
        }

        private async Task DeleteUserAsync(User user)
        {
            if (user == null) return;

            bool confirm = await _messagingService.ShowConfirmationAsync(
                "Eliminar usuario",
                $"¿Estás seguro de eliminar a {user.Name}?",
                "Sí", "No"
            );

            if (!confirm) return;

            await _userService.DeleteAsync(user.Id);
            Users.Remove(user);
        }
    }
}