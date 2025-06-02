using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentMethodsViewModel
    {
        private readonly IMessagingService _messagingService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IValidationService<PaymentMethod> _paymentMethodValidationService;

        public ObservableCollection<PaymentMethod> PaymentMethods { get; } = new ObservableCollection<PaymentMethod>();
        public ICommand RegisterPaymentMethodCommand { get; }
        public ICommand EditPaymentMethodCommand { get; }
        public ICommand DeletePaymentMethodCommand { get; }
        public ICommand LoadPaymentMethodsCommand { get; }
        public bool IsBusy { get; set; }

        public PaymentMethodsViewModel(
            IPaymentMethodService paymentMethodService,
            IValidationService<PaymentMethod> paymentMethodValidationService,
            IMessagingService messagingService)
        {
            _paymentMethodService = paymentMethodService;
            _paymentMethodValidationService = paymentMethodValidationService;
            _messagingService = messagingService;
            LoadPaymentMethodsCommand = new Command(async () => await LoadPaymentMethodsAsync());
            RegisterPaymentMethodCommand = new Command(async () => await OpenRegisterModal());
            EditPaymentMethodCommand = new Command<PaymentMethod>(async pm => await OpenEditModal(pm));
            DeletePaymentMethodCommand = new Command<PaymentMethod>(async pm => await DeletePaymentMethodAsync(pm));
        }

        private async Task LoadPaymentMethodsAsync()
        {
            IsBusy = true;
            var methods = await _paymentMethodService.GetAllAsync();
            PaymentMethods.Clear();
            foreach (var pm in methods)
                PaymentMethods.Add(pm);
            IsBusy = false;
        }

        private async Task OpenRegisterModal()
        {
            var formPage = new Views.PaymentMethodFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentMethodFormViewModel(_paymentMethodService, _paymentMethodValidationService, _messagingService, mainPage.Navigation);
                viewModel.PaymentMethodSaved += pm =>
                {
                    PaymentMethods.Add(pm);
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }

        private async Task OpenEditModal(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null) return;
            var formPage = new Views.PaymentMethodFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentMethodFormViewModel(paymentMethod, _paymentMethodService, _paymentMethodValidationService, _messagingService, mainPage.Navigation);
                viewModel.PaymentMethodSaved += updated =>
                {
                    var existing = PaymentMethods.FirstOrDefault(pm => pm.Id == updated.Id);
                    if (existing != null)
                    {
                        existing.Name = updated.Name;
                    }
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }

        private async Task DeletePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null) return;

            bool confirm = await _messagingService.ShowConfirmationAsync(
                "Eliminar método de pago",
                $"¿Estás seguro de eliminar {paymentMethod.Name}?",
                "Sí", "No"
            );

            if (!confirm) return;

            await _paymentMethodService.DeleteAsync(paymentMethod.Id);
            PaymentMethods.Remove(paymentMethod);
        }
    }
}