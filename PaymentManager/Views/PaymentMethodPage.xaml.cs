using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class PaymentMethodPage : ContentPage
{
    public PaymentMethodPage()
    {
        InitializeComponent();
        BindingContext = new PaymentMethodsViewModel(
            MauiProgram.Services.GetService<IPaymentMethodService>()!,
            MauiProgram.Services.GetService<IValidationService<PaymentMethod>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PaymentMethodsViewModel vm)
            vm.LoadPaymentMethodsCommand.Execute(null);
    }
}