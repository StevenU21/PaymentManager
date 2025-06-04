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

        private decimal totalAmount;
        public decimal TotalAmount
        {
            get => totalAmount;
            set => SetProperty(ref totalAmount, value);
        }

        private int dayOfMonthToPay = 1;
        public int DayOfMonthToPay
        {
            get => dayOfMonthToPay;
            set => SetProperty(ref dayOfMonthToPay, value);
        }

        private bool active = true;
        public bool Active
        {
            get => active;
            set => SetProperty(ref active, value);
        }

        public ICollection<UserPaymentPlan> UserPaymentPlans { get; set; } = new List<UserPaymentPlan>();
    }
}