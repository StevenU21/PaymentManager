
namespace PaymentManager.Models
{
    public class PaymentType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ushort IntervalDays { get; set; }

        public ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();
    }
}
