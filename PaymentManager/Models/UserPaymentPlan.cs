using System.Linq;

namespace PaymentManager.Models
{
    public class UserPaymentPlan : ObservableObject
    {
        public int Id { get; set; }

        private int userId;
        public int UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        private User? user;
        public User? User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        private int paymentPlanId;
        public int PaymentPlanId
        {
            get => paymentPlanId;
            set => SetProperty(ref paymentPlanId, value);
        }

        private PaymentPlan? paymentPlan;
        public PaymentPlan? PaymentPlan
        {
            get => paymentPlan;
            set => SetProperty(ref paymentPlan, value);
        }

        public decimal ShareAmount
        {
            get
            {
                if (PaymentPlan == null || PaymentPlan.TotalAmount == 0)
                    return 0;
                int usersInPlan = PaymentPlan.UserPaymentPlans?.Count(u => u.Active) ?? 1;
                return usersInPlan > 0 ? PaymentPlan.TotalAmount / usersInPlan : 0;
            }
        }

        private DateTime joinDate = DateTime.Now;
        public DateTime JoinDate
        {
            get => joinDate;
            set => SetProperty(ref joinDate, value);
        }

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

        private bool active = true;
        public bool Active
        {
            get => active;
            set => SetProperty(ref active, value);
        }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}