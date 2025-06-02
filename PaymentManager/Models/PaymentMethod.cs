namespace PaymentManager.Models
{
    public class PaymentMethod : ObservableObject
    {
        public int Id { get; set; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}