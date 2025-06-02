using PaymentManager.Models;

namespace PaymentManager.Validators
{
    public class PaymentTypeValidator : BaseValidator<PaymentType>
    {
        public PaymentTypeValidator(IEnumerable<PaymentType> paymentTypes) : base(paymentTypes) { }
        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["Name"] = "required|min:3|max:20|regex:^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$",
                ["IntervalDays"] = "required|integer|min:1|max:365",
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["Name.required"] = "El nombre es obligatorio.",
                ["Name.min"] = "El nombre debe tener al menos 3 caracteres.",
                ["Name.max"] = "El nombre no puede superar los 20 caracteres.",
                ["Name.regex"] = "El nombre solo puede contener letras y espacios.",

                ["IntervalDays.required"] = "El intervalo es obligatorio.",
                ["IntervalDays.integer"] = "El intervalo debe ser un número entero.",
                ["IntervalDays.min"] = "El intervalo debe ser al menos 1 día.",
                ["IntervalDays.max"] = "El intervalo no puede superar los 365 días.",
            };
        }
    }
}
