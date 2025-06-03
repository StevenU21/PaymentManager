using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentPlanFormViewModel
    {
        private readonly IValidationService<PaymentPlan> _paymentPlanValidationService;
        private readonly IMessagingService _messagingService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly INavigation _navigation;
        public event Action<PaymentPlan>? PaymentPlanSaved;

        public PaymentPlan PaymentPlan { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PaymentPlanFormViewModel(
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService,
            INavigation navigation)
        {
            _paymentPlanService = paymentPlanService ?? throw new ArgumentNullException(nameof(paymentPlanService));
            _paymentPlanValidationService = paymentPlanValidationService ?? throw new ArgumentNullException(nameof(paymentPlanValidationService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PaymentPlan = new PaymentPlan();
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
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

        private async Task SaveAsync()
        {
            var (isValid, errorMessage) = await _paymentPlanValidationService.ValidateAsync(PaymentPlan, PaymentPlan.Id != 0);

            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }

            if (PaymentPlan.Id != 0)
            {
                await _paymentPlanService.UpdateAsync(PaymentPlan);
                PaymentPlanSaved?.Invoke(PaymentPlan);
            }
            else
            {
                await _paymentPlanService.AddAsync(PaymentPlan);
                PaymentPlanSaved?.Invoke(PaymentPlan);
            }

            await _navigation.PopModalAsync();
        }
    }
}