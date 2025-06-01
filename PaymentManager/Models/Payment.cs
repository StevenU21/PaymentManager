
namespace PaymentManager.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal AmountPaid { get; set; }

        public ushort PeriodsPaid { get; set; }

        public DateTime NextDueDate { get; set; }

        public int? PaymentMethodId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
    }
}
