using PaymentManager.ViewModels;
using PaymentManager.Services;
using PaymentManager.Models;

namespace PaymentManager.Views;

public partial class UserPage : ContentPage
{
    public UserPage()
    {
        InitializeComponent();
        BindingContext = new UsersViewModel(
            MauiProgram.Services.GetService<IUserService>()!,
            MauiProgram.Services.GetService<IValidationService<User>>()!,
            MauiProgram.Services.GetService<IMessagingService>()!
        );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UsersViewModel vm)
            vm.LoadUsersCommand.Execute(null);
    }
}