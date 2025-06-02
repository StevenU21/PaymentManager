using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentTypeFormViewModel
    {
        private readonly IValidationService<PaymentType> _paymentTypeValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly INavigation _navigation;
        public event Action<PaymentType>? PaymentTypeSaved;

        public PaymentType PaymentType { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PaymentTypeFormViewModel(
            IPaymentTypeService paymentTypeService,
            IValidationService<PaymentType> paymentTypeValidationService,
            IMessagingService messagingService,
            INavigation navigation)
        {
            _paymentTypeService = paymentTypeService ?? throw new ArgumentNullException(nameof(paymentTypeService));
            _paymentTypeValidationService = paymentTypeValidationService ?? throw new ArgumentNullException(nameof(paymentTypeValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PaymentType = new PaymentType();
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        public PaymentTypeFormViewModel(
            PaymentType paymentType,
            IPaymentTypeService paymentTypeService,
            IValidationService<PaymentType> paymentTypeValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : this(paymentTypeService, paymentTypeValidationService, messagingService, navigation)
        {
            PaymentType = paymentType;
        }

        private async Task SaveAsync()
        {
            var (isValid, errorMessage) = await _paymentTypeValidationService.ValidateAsync(PaymentType, PaymentType.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (PaymentType.Id != 0)
            {
                await _paymentTypeService.UpdateAsync(PaymentType);
                PaymentTypeSaved?.Invoke(PaymentType);
            }
            else
            {
                await _paymentTypeService.AddAsync(PaymentType);
                PaymentTypeSaved?.Invoke(PaymentType);
            }

            await _navigation.PopModalAsync();
        }
    }
}