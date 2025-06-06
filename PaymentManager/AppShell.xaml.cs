namespace PaymentManager
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnGitHubButtonClicked(object sender, EventArgs e)
        {
            var url = "https://github.com/StevenU21";
            try
            {
                await Launcher.Default.OpenAsync(url);
            }
            catch
            {
                // Manejo de error opcional
            }
        }
    }
}
