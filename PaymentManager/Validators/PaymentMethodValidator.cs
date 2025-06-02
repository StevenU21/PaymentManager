using PaymentManager.Models;

namespace PaymentManager.Validators
{
    public class PaymentMethodValidator : BaseValidator<PaymentMethod>
    {
        public PaymentMethodValidator(IEnumerable<PaymentMethod> paymentMethods) : base(paymentMethods) { }
        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["Name"] = "required|min:3|max:50|regex:^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$",
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["Name.required"] = "El nombre es obligatorio.",
                ["Name.min"] = "El nombre debe tener al menos 3 caracteres.",
                ["Name.max"] = "El nombre no puede superar los 50 caracteres.",
                ["Name.regex"] = "El nombre solo puede contener letras y espacios.",
            };
        }
    }
}
