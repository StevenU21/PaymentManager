using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class UserPaymentPlansViewModel : BaseListViewModel<UserPaymentPlan>
    {
        private readonly IMessagingService _messagingService;
        private readonly IUserPaymentPlanService _userPaymentPlanService;
        private readonly IValidationService<UserPaymentPlan> _userPaymentPlanValidationService;

        public ObservableCollection<UserPaymentPlan> UserPaymentPlans => Items;

        public ICommand RegisterUserPaymentPlanCommand { get; }
        public ICommand EditUserPaymentPlanCommand { get; }
        public ICommand DeleteUserPaymentPlanCommand { get; }

        public UserPaymentPlansViewModel(
            IUserPaymentPlanService userPaymentPlanService,
            IValidationService<UserPaymentPlan> userPaymentPlanValidationService,
            IMessagingService messagingService)
            : base()
        {
            _userPaymentPlanService = userPaymentPlanService;
            _userPaymentPlanValidationService = userPaymentPlanValidationService;
            _messagingService = messagingService;
            RegisterUserPaymentPlanCommand = new Command(async () => await OpenRegisterModal());
            EditUserPaymentPlanCommand = new Command<UserPaymentPlan>(async upp => await OpenEditModal(upp));
            DeleteUserPaymentPlanCommand = new Command<UserPaymentPlan>(async upp => await DeleteUserPaymentPlanAsync(upp));
        }

        protected override async Task<IEnumerable<UserPaymentPlan>> GetItemsAsync()
        {
            return await _userPaymentPlanService.GetAllAsync();
        }

        private async Task OpenRegisterModal()
        {
            var formPage = new Views.UserPaymentPlanFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new UserPaymentPlanFormViewModel(
                    _userPaymentPlanService,
                    _userPaymentPlanValidationService,
                    _messagingService,
                    MauiProgram.Services.GetService<IUserService>()!,
                    MauiProgram.Services.GetService<IPaymentPlanService>()!,
                    mainPage.Navigation
                );
                viewModel.EntitySaved += async upp =>
                {
                    await ReloadUserPaymentPlansAsync(); // Recarga toda la lista
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }
        
        private async Task OpenEditModal(UserPaymentPlan userPaymentPlan)
        {
            if (userPaymentPlan == null) return;
            var formPage = new Views.UserPaymentPlanFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new UserPaymentPlanFormViewModel(
                    userPaymentPlan,
                    _userPaymentPlanService,
                    _userPaymentPlanValidationService,
                    _messagingService,
                    MauiProgram.Services.GetService<IUserService>()!,
                    MauiProgram.Services.GetService<IPaymentPlanService>()!,
                    mainPage.Navigation
                );
                viewModel.EntitySaved += async updated =>
                {
                    await ReloadUserPaymentPlansAsync(); 
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }

        private async Task DeleteUserPaymentPlanAsync(UserPaymentPlan userPaymentPlan)
        {
            if (userPaymentPlan == null) return;

            bool confirm = await _messagingService.ShowConfirmationAsync(
                "Eliminar relación usuario-plan",
                $"¿Estás seguro de eliminar esta relación?",
                "Sí", "No"
            );

            if (!confirm) return;

            await _userPaymentPlanService.DeleteAsync(userPaymentPlan.Id);

            await ReloadUserPaymentPlansAsync();
        }

        private async Task ReloadUserPaymentPlansAsync()
        {
            var items = await GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
    }
}