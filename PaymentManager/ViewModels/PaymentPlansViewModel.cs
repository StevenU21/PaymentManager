using System.Collections.ObjectModel;
using System.Windows.Input;
using PaymentManager.Models;
using PaymentManager.Services;

namespace PaymentManager.ViewModels
{
    public class PaymentPlansViewModel
    {
        private readonly IMessagingService _messagingService;
        private readonly IPaymentPlanService _paymentPlanService;
        private readonly IValidationService<PaymentPlan> _paymentPlanValidationService;
        public ObservableCollection<PaymentPlan> PaymentPlans { get; } = new ObservableCollection<PaymentPlan>();
        public ICommand RegisterPaymentPlanCommand { get; }
        public ICommand EditPaymentPlanCommand { get; }
        public ICommand DeletePaymentPlanCommand { get; }
        public ICommand LoadPaymentPlansCommand { get; }
        public bool IsBusy { get; set; }

        public PaymentPlansViewModel(
            IPaymentPlanService paymentPlanService,
            IValidationService<PaymentPlan> paymentPlanValidationService,
            IMessagingService messagingService)
        {
            _paymentPlanService = paymentPlanService;
            _paymentPlanValidationService = paymentPlanValidationService;
            _messagingService = messagingService;
            LoadPaymentPlansCommand = new Command(async () => await LoadPaymentPlansAsync());
            RegisterPaymentPlanCommand = new Command(async () => await OpenRegisterModal());
            EditPaymentPlanCommand = new Command<PaymentPlan>(async plan => await OpenEditModal(plan));
            DeletePaymentPlanCommand = new Command<PaymentPlan>(async plan => await DeletePaymentPlanAsync(plan));
        }

        private async Task LoadPaymentPlansAsync()
        {
            IsBusy = true;
            var plans = await _paymentPlanService.GetAllAsync();
            PaymentPlans.Clear();
            foreach (var plan in plans)
                PaymentPlans.Add(plan);
            IsBusy = false;
        }

        private async Task OpenRegisterModal()
        {
            var formPage = new Views.PaymentPlanFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentPlanFormViewModel(
                    _paymentPlanService,
                    _paymentPlanValidationService,
                    _messagingService,
                    mainPage.Navigation);
                viewModel.PaymentPlanSaved += plan =>
                {
                    PaymentPlans.Add(plan);
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }

        private async Task OpenEditModal(PaymentPlan paymentPlan)
        {
            if (paymentPlan == null) return;
            var formPage = new Views.PaymentPlanFormPage();
            var app = Application.Current;
            var mainWindow = app?.Windows.FirstOrDefault();
            var mainPage = mainWindow?.Page;
            if (mainPage?.Navigation != null)
            {
                var viewModel = new PaymentPlanFormViewModel(
                    paymentPlan,
                    _paymentPlanService,
                    _paymentPlanValidationService,
                    _messagingService,
                    mainPage.Navigation);
                viewModel.PaymentPlanSaved += updated =>
                {
                    var existing = PaymentPlans.FirstOrDefault(p => p.Id == updated.Id);
                    if (existing != null)
                    {
                        existing.Name = updated.Name;
                        existing.Amount = updated.Amount;
                        existing.DayOfMonth = updated.DayOfMonth;
                        existing.Active = updated.Active;
                    }
                };
                formPage.BindingContext = viewModel;
                await mainPage.Navigation.PushModalAsync(formPage);
            }
        }

        private async Task DeletePaymentPlanAsync(PaymentPlan paymentPlan)
        {
            if (paymentPlan == null) return;

            bool confirm = await _messagingService.ShowConfirmationAsync(
                "Eliminar plan de pago",
                $"¿Estás seguro de eliminar este plan?",
                "Sí", "No"
            );

            if (!confirm) return;

            await _paymentPlanService.DeleteAsync(paymentPlan.Id);
            PaymentPlans.Remove(paymentPlan);
        }
    }
}