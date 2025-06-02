using System.Collections.ObjectModel;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentTypesViewModel
    {
        private readonly IMessagingService _messagingService;
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IValidationService<PaymentType> _paymentTypeValidationService;

        public ObservableCollection<PaymentType> PaymentTypes { get; } = new ObservableCollection<PaymentType>();
        public Command RegisterPaymentTypeCommand { get; }
        public Command<PaymentType> EditPaymentTypeCommand { get; }
        public Command<PaymentType> DeletePaymentTypeCommand { get; }
        public Command LoadPaymentTypesCommand { get; }
        public bool IsBusy { get; set; }

        public PaymentTypesViewModel(
            IPaymentTypeService paymentTypeService,
            IValidationService<PaymentType> paymentTypeValidationService,
            IMessagingService messagingService)
        {
            _paymentTypeService = paymentTypeService;
            _paymentTypeValidationService = paymentTypeValidationService;
            _messagingService = messagingService;
            LoadPaymentTypesCommand = new Command(async () => await LoadPaymentTypesAsync());
            RegisterPaymentTypeCommand = new Command(async () => await OpenRegisterModal());
            EditPaymentTypeCommand = new Command<PaymentType>(async pt => await OpenEditModal(pt));
            DeletePaymentTypeCommand = new Command<PaymentType>(async pt => await DeletePaymentTypeAsync(pt));
        }

        private async Task LoadPaymentTypesAsync()
        {
            IsBusy = true;
            var paymentTypes = await _paymentTypeService.GetAllAsync();
            PaymentTypes.Clear();
            foreach (var pt in paymentTypes)
                PaymentTypes.Add(pt);
            IsBusy = false;
        }

        private async Task OpenRegisterModal()
        {
            var paymentTypeFormPage = new Views.PaymentTypeFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentTypeFormViewModel(_paymentTypeService, _paymentTypeValidationService, _messagingService, mainPage.Navigation);
                viewModel.PaymentTypeSaved += paymentType =>
                {
                    PaymentTypes.Add(paymentType);
                };
                paymentTypeFormPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(paymentTypeFormPage);
            }
        }

        private async Task OpenEditModal(PaymentType paymentType)
        {
            if (paymentType == null) return;
            var paymentTypeFormPage = new Views.PaymentTypeFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentTypeFormViewModel(paymentType, _paymentTypeService, _paymentTypeValidationService, _messagingService, mainPage.Navigation);
                viewModel.PaymentTypeSaved += updatedPaymentType =>
                {
                    var existing = PaymentTypes.FirstOrDefault(pt => pt.Id == updatedPaymentType.Id);
                    if (existing != null)
                    {
                        existing.Name = updatedPaymentType.Name;
                        existing.IntervalDays = updatedPaymentType.IntervalDays;
                    }
                };
                paymentTypeFormPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(paymentTypeFormPage);
            }
        }

        private async Task DeletePaymentTypeAsync(PaymentType paymentType)
        {
            if (paymentType == null) return;

            bool confirm = await _messagingService.ShowConfirmationAsync(
                "Eliminar tipo de pago",
                $"¿Estás seguro de eliminar {paymentType.Name}?",
                "Sí", "No"
            );

            if (!confirm) return;

            await _paymentTypeService.DeleteAsync(paymentType.Id);
            PaymentTypes.Remove(paymentType);
        }
    }
}