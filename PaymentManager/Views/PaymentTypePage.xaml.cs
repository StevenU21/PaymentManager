using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class PaymentTypePage : ContentPage
{
    public PaymentTypePage()
    {
        InitializeComponent();
        BindingContext = new PaymentTypesViewModel(
            MauiProgram.Services.GetService<IPaymentTypeService>()!,
            MauiProgram.Services.GetService<IValidationService<PaymentType>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaymentTypesViewModel vm)
            vm.LoadPaymentTypesCommand.Execute(null);
    }
}