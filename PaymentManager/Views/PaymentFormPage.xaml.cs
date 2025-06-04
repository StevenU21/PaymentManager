namespace PaymentManager.Views;

public partial class PaymentFormPage : ContentPage
{
	public PaymentFormPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is ViewModels.PaymentFormViewModel vm)
			await vm.LoadCombosAsync();
	}
}