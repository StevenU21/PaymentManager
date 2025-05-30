using PaymentManager.ViewModels;
using PaymentManager.Services;

namespace PaymentManager.Views;

public partial class UserPage : ContentPage
{
    public UserPage()
    {
        InitializeComponent();
        BindingContext = new UsersViewModel(MauiProgram.Services.GetService<IUserService>()!);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is UsersViewModel vm)
            vm.LoadUsersCommand.Execute(null);
    }
}