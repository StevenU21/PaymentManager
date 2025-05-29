using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
