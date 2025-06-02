namespace PaymentManager.Models
{
    public class Payment : ObservableObject
    {
        public int Id { get; set; }

        private int userId;
        public int UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        private DateTime paymentDate;
        public DateTime PaymentDate
        {
            get => paymentDate;
            set => SetProperty(ref paymentDate, value);
        }

        private decimal amountPaid;
        public decimal AmountPaid
        {
            get => amountPaid;
            set => SetProperty(ref amountPaid, value);
        }

        private ushort periodsPaid;
        public ushort PeriodsPaid
        {
            get => periodsPaid;
            set => SetProperty(ref periodsPaid, value);
        }

        private DateTime nextDueDate;
        public DateTime NextDueDate
        {
            get => nextDueDate;
            set => SetProperty(ref nextDueDate, value);
        }

        private int? paymentMethodId;
        public int? PaymentMethodId
        {
            get => paymentMethodId;
            set => SetProperty(ref paymentMethodId, value);
        }

        private DateTime createdAt = DateTime.Now;
        public DateTime CreatedAt
        {
            get => createdAt;
            set => SetProperty(ref createdAt, value);
        }

        private DateTime updatedAt = DateTime.Now;
        public DateTime UpdatedAt
        {
            get => updatedAt;
            set => SetProperty(ref updatedAt, value);
        }

        public User? User { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}