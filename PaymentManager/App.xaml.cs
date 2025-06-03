namespace PaymentManager
{
    public partial class App : Application
    {
        private readonly AppShell _appShell;

        public App(AppShell appShell)
        {
            InitializeComponent();
            _appShell = appShell;

            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }
    }
}