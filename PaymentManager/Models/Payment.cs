namespace PaymentManager.Models
{
    public class Payment : ObservableObject
    {
        public int Id { get; set; }

        public int UserPaymentPlanId { get; set; }
        public UserPaymentPlan? UserPaymentPlan { get; set; }

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

        private ushort periodsPaid = 1;
        public ushort PeriodsPaid
        {
            get => periodsPaid;
            set => SetProperty(ref periodsPaid, value);
        }

        private DateTime? nextDueDate;
        public DateTime? NextDueDate
        {
            get => nextDueDate;
            set => SetProperty(ref nextDueDate, value);
        }

        public int? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}