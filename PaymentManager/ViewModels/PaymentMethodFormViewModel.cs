using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentMethodFormViewModel
    {
        private readonly IValidationService<PaymentMethod> _paymentMethodValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly INavigation _navigation;
        public event Action<PaymentMethod>? PaymentMethodSaved;

        public PaymentMethod PaymentMethod { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PaymentMethodFormViewModel(
            IPaymentMethodService paymentMethodService,
            IValidationService<PaymentMethod> paymentMethodValidationService,
            IMessagingService messagingService,
            INavigation navigation)
        {
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            _paymentMethodValidationService = paymentMethodValidationService ?? throw new ArgumentNullException(nameof(paymentMethodValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PaymentMethod = new PaymentMethod();
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        public PaymentMethodFormViewModel(
            PaymentMethod paymentMethod,
            IPaymentMethodService paymentMethodService,
            IValidationService<PaymentMethod> paymentMethodValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : this(paymentMethodService, paymentMethodValidationService, messagingService, navigation)
        {
            PaymentMethod = paymentMethod;
        }

        private async Task SaveAsync()
        {
            var (isValid, errorMessage) = await _paymentMethodValidationService.ValidateAsync(PaymentMethod, PaymentMethod.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (PaymentMethod.Id != 0)
            {
                await _paymentMethodService.UpdateAsync(PaymentMethod);
                PaymentMethodSaved?.Invoke(PaymentMethod);
            }
            else
            {
                await _paymentMethodService.AddAsync(PaymentMethod);
                PaymentMethodSaved?.Invoke(PaymentMethod);
            }

            await _navigation.PopModalAsync();
        }
    }
}