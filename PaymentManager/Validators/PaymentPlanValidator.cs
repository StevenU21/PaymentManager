using PaymentManager.Models;

namespace PaymentManager.Validators
{
    public class PaymentPlanValidator : BaseValidator<PaymentPlan>
    {
        public PaymentPlanValidator(IEnumerable<PaymentPlan> paymentPlans) : base(paymentPlans) { }

        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["Name"] = "required|min:3|max:50",
                ["DayOfMonth"] = "required|date",
                ["Amount"] = "required|float",
                ["Active"] = "required"
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["Name.required"] = "El nombre es obligatorio.",
                ["Name.min"] = "El nombre debe tener al menos 3 caracteres.",
                ["Name.max"] = "El nombre no puede superar los 50 caracteres.",

                ["DayOfMonth.required"] = "El día de pago es obligatorio.",
                ["DayOfMonth.date"] = "El día de pago debe ser una fecha válida.",

                ["Amount.required"] = "El monto es obligatorio.",
                ["Amount.float"] = "El monto debe ser un número válido.",
                ["Amount.min"] = "El monto debe ser mayor a 0.",

                ["Active.required"] = "Debe indicar si el plan está activo."
            };
        }
    }
}