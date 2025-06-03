namespace PaymentManager.Models
{
    public class PaymentPlan : ObservableObject
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private DateTime? dayOfMonth;
        public DateTime? DayOfMonth
        {
            get => dayOfMonth;
            set => SetProperty(ref dayOfMonth, value);
        }

        private decimal amount;
        public decimal Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        private bool active = true;
        public bool Active
        {
            get => active;
            set => SetProperty(ref active, value);
        }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}