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

        private int? paymentPlanId;
        public int? PaymentPlanId
        {
            get => paymentPlanId;
            set => SetProperty(ref paymentPlanId, value);
        }

        public User? User { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public PaymentPlan? PaymentPlan { get; set; }
    }
}