namespace PaymentManager.Models
{
    public class PaymentPlan : ObservableObject
    {
        public int Id { get; set; }

        private int userId;
        public int UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        private int paymentTypeId;
        public int PaymentTypeId
        {
            get => paymentTypeId;
            set => SetProperty(ref paymentTypeId, value);
        }

        private byte? dayOfMonth;
        public byte? DayOfMonth
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

        private DateTime startDate;
        public DateTime StartDate
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }

        public User? User { get; set; }
        public PaymentType? PaymentType { get; set; }
    }
}