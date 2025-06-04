using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class UserPaymentPlansPage : ContentPage
{
    public UserPaymentPlansPage()
    {
        InitializeComponent();
        BindingContext = new UserPaymentPlansViewModel(
            MauiProgram.Services.GetService<IUserPaymentPlanService>()!,
            MauiProgram.Services.GetService<IValidationService<UserPaymentPlan>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UserPaymentPlansViewModel vm)
            vm.LoadItemsCommand.Execute(null);
    }
}