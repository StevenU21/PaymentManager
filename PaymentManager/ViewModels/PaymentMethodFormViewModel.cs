using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentMethodFormViewModel : BaseFormViewModel<PaymentMethod>
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethod PaymentMethod
        {
            get => Entity!;
            set => Entity = value;
        }

        public PaymentMethodFormViewModel(
            IPaymentMethodService paymentMethodService,
            IValidationService<PaymentMethod> paymentMethodValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : base(paymentMethodValidationService, messagingService, navigation)
        {
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
            PaymentMethod = new PaymentMethod();
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

        protected override async Task SaveOrUpdateAsync()
        {
            if (PaymentMethod.Id != 0)
                await _paymentMethodService.UpdateAsync(PaymentMethod);
            else
                await _paymentMethodService.AddAsync(PaymentMethod);
        }

        protected override bool GetIsEdit() => PaymentMethod.Id != 0;
    }
}