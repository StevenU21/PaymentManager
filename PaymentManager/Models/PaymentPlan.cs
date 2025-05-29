using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentManager.Models
{
    public class PaymentPlan
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PaymentTypeId { get; set; }

        public byte? DayOfMonth { get; set; }

        public decimal Amount { get; set; }

        public bool Active { get; set; } = true;

        public DateTime StartDate { get; set; }

        public User? User { get; set; }

        public PaymentType? PaymentType { get; set; }
    }
}
