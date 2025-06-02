using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class PaymentPlanPage : ContentPage
{
    public PaymentPlanPage()
    {
        InitializeComponent();
        BindingContext = new PaymentPlansViewModel(
            MauiProgram.Services.GetService<IPaymentPlanService>()!,
            MauiProgram.Services.GetService<IValidationService<PaymentPlan>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!,
            MauiProgram.Services.GetService<IUserService>()!,
            MauiProgram.Services.GetService<IPaymentTypeService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaymentPlansViewModel vm)
            vm.LoadPaymentPlansCommand.Execute(null);
    }
}