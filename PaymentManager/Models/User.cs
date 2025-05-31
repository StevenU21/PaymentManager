using System.ComponentModel; 

namespace PaymentManager.Models
{
    public class User : INotifyPropertyChanged 
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string email = string.Empty;
        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        private string? phone;
        public string? Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();
        public PaymentStatus? PaymentStatus { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}