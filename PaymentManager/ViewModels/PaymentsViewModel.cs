    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using PaymentManager.Models;
    using PaymentManager.Services;
    
    namespace PaymentManager.ViewModels
    {
        public class PaymentsViewModel
        {
            private readonly IMessagingService _messagingService;
            private readonly IPaymentService _paymentService;
            private readonly IValidationService<Payment> _paymentValidationService;
            private readonly IUserService _userService;
            private readonly IPaymentPlanService _paymentPlanService;
            private readonly IPaymentMethodService _paymentMethodService;
    
            public ObservableCollection<Payment> Payments { get; } = new ObservableCollection<Payment>();
            public ICommand RegisterPaymentCommand { get; }
            public ICommand EditPaymentCommand { get; }
            public ICommand DeletePaymentCommand { get; }
            public ICommand LoadPaymentsCommand { get; }
            public bool IsBusy { get; set; }
    
            public PaymentsViewModel(
                IPaymentService paymentService,
                IValidationService<Payment> paymentValidationService,
                IMessagingService messagingService,
                IUserService userService,
                IPaymentPlanService paymentPlanService,
                IPaymentMethodService paymentMethodService
            )
            {
                _paymentService = paymentService;
                _paymentValidationService = paymentValidationService;
                _messagingService = messagingService;
                _userService = userService;
                _paymentPlanService = paymentPlanService;
                _paymentMethodService = paymentMethodService;
                LoadPaymentsCommand = new Command(async () => await LoadPaymentsAsync());
                RegisterPaymentCommand = new Command(async () => await OpenRegisterModal());
                EditPaymentCommand = new Command<Payment>(async payment => await OpenEditModal(payment));
                DeletePaymentCommand = new Command<Payment>(async payment => await DeletePaymentAsync(payment));
            }
    
            private async Task LoadPaymentsAsync()
            {
                IsBusy = true;
                var payments = await _paymentService.GetAllAsync();
                Payments.Clear();
                foreach (var payment in payments)
                    Payments.Add(payment);
                IsBusy = false;
            }
    
            private async Task OpenRegisterModal()
            {
                var formPage = new Views.PaymentFormPage();
                var app = Application.Current;
                var mainWindow = app?.Windows.FirstOrDefault();
                var mainPage = mainWindow?.Page;
                if (mainPage?.Navigation != null)
                {
                    var viewModel = new PaymentFormViewModel(
                        _paymentService,
                        _paymentValidationService,
                        _messagingService,
                        _userService,
                        _paymentPlanService,
                        _paymentMethodService,
                        mainPage.Navigation
                    );
                    viewModel.PaymentSaved += payment =>
                    {
                        Payments.Add(payment);
                    };
                    formPage.BindingContext = viewModel;
                    await mainPage.Navigation.PushModalAsync(formPage);
                }
            }
    
            private async Task OpenEditModal(Payment payment)
            {
                if (payment == null) return;
                var formPage = new Views.PaymentFormPage();
                var app = Application.Current;
                var mainWindow = app?.Windows.FirstOrDefault();
                var mainPage = mainWindow?.Page;
                if (mainPage?.Navigation != null)
                {
                    var viewModel = new PaymentFormViewModel(
                        payment,
                        _paymentService,
                        _paymentValidationService,
                        _messagingService,
                        _userService,
                        _paymentPlanService,
                        _paymentMethodService,
                        mainPage.Navigation
                    );
                    viewModel.PaymentSaved += updated =>
                    {
                        var existing = Payments.FirstOrDefault(p => p.Id == updated.Id);
                        if (existing != null)
                        {
                            existing.AmountPaid = updated.AmountPaid;
                            existing.PaymentDate = updated.PaymentDate;
                            existing.PeriodsPaid = updated.PeriodsPaid;
                            existing.NextDueDate = updated.NextDueDate;
                            existing.PaymentMethodId = updated.PaymentMethodId;
                            existing.PaymentPlanId = updated.PaymentPlanId;
                            existing.UserId = updated.UserId;
                            // Actualiza otros campos si es necesario
                        }
                    };
                    formPage.BindingContext = viewModel;
                    await mainPage.Navigation.PushModalAsync(formPage);
                }
            }
    
            private async Task DeletePaymentAsync(Payment payment)
            {
                if (payment == null) return;
    
                bool confirm = await _messagingService.ShowConfirmationAsync(
                    "Eliminar pago",
                    $"¿Estás seguro de eliminar este pago?",
                    "Sí", "No"
                );
    
                if (!confirm) return;
    
                await _paymentService.DeleteAsync(payment.Id);
                Payments.Remove(payment);
            }
        }
    }