using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentManager.Models
{
    public class PaymentStatus
    {
        public int UserId { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime? NextDueDate { get; set; }

        public string Status { get; set; } = "On Time"; // SQLite no admite enum directamente

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; } 
    }
}
