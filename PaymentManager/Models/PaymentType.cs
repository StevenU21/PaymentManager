namespace PaymentManager.Models
{
    public class PaymentType : ObservableObject
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private int intervalDays;
        public int IntervalDays
        {
            get => intervalDays;
            set => SetProperty(ref intervalDays, value);
        }

        public ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();
    }
}