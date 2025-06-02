namespace PaymentManager.Models
{
    public class PaymentStatus : ObservableObject
    {
        public int UserId { get; set; }

        private DateTime? lastPaymentDate;
        public DateTime? LastPaymentDate
        {
            get => lastPaymentDate;
            set => SetProperty(ref lastPaymentDate, value);
        }

        private DateTime? nextDueDate;
        public DateTime? NextDueDate
        {
            get => nextDueDate;
            set => SetProperty(ref nextDueDate, value);
        }

        private string status = "On Time";
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private DateTime updatedAt = DateTime.Now;
        public DateTime UpdatedAt
        {
            get => updatedAt;
            set => SetProperty(ref updatedAt, value);
        }

        public User? User { get; set; }
    }
}