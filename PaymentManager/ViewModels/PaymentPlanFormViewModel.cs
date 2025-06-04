using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentPlanFormViewModel : BaseFormViewModel<PaymentPlan>
    {
        private readonly IPaymentPlanService _paymentPlanService;

        public PaymentPlan PaymentPlan
        {
            get => Entity!;
            set => Entity = value;
        }

        public PaymentPlanFormViewModel(
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : base(paymentPlanValidationService, messagingService, navigation)
        {
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            PaymentPlan = new PaymentPlan();
        }

        public PaymentPlanFormViewModel(
            PaymentPlan paymentPlan,
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService,
            INavigation navigation)
            : this(paymentPlanService, paymentPlanValidationService, messagingService, navigation)
        {
            PaymentPlan = paymentPlan;
        }

        protected override async Task SaveOrUpdateAsync()
        {
            if (PaymentPlan.Id != 0)
                await _paymentPlanService.UpdateAsync(PaymentPlan);
            else
                await _paymentPlanService.AddAsync(PaymentPlan);
        }

        protected override bool GetIsEdit() => PaymentPlan.Id != 0;
    }
}