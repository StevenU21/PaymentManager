using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class PaymentPage : ContentPage
{
    public PaymentPage()
    {
        InitializeComponent();
        BindingContext = new PaymentsViewModel(
            MauiProgram.Services.GetService<IPaymentService>()!,
            MauiProgram.Services.GetService<IValidationService<Payment>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!,
            MauiProgram.Services.GetService<IUserService>()!,
            MauiProgram.Services.GetService<IPaymentPlanService>()!,
            MauiProgram.Services.GetService<IPaymentMethodService>()!,
            MauiProgram.Services.GetService<IUserPaymentPlanService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaymentsViewModel vm)
            vm.LoadItemsCommand.Execute(null);
    }
}