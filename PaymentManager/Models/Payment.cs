namespace PaymentManager.Models
{
    public class Payment : ObservableObject
    {
        public int Id { get; set; }

        public int UserPaymentPlanId { get; set; }
        public UserPaymentPlan? UserPaymentPlan { get; set; }

        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public ushort PeriodsPaid { get; set; } = 1;

        // Cambiado a propiedad auto-implementada para persistencia correcta
        public DateTime? NextDueDate { get; set; }

        public int? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}