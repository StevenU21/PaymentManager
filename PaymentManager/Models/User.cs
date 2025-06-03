namespace PaymentManager.Models
{
    public class User : ObservableObject
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private string? phone;
        public string? Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();
        public PaymentStatus? PaymentStatus { get; set; }
    }
}