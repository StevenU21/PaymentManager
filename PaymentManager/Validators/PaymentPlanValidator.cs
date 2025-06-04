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
                ["DayOfMonthToPay"] = "required|integer|min:1|max:31",
                ["TotalAmount"] = "required|float",
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

                ["DayOfMonthToPay.required"] = "El día de pago es obligatorio.",
                ["DayOfMonthToPay.integer"] = "El día de pago debe ser un número entero.",
                ["DayOfMonthToPay.min"] = "El día de pago debe ser al menos 1.",
                ["DayOfMonthToPay.max"] = "El día de pago no puede ser mayor a 31.",

                ["TotalAmount.required"] = "El monto es obligatorio.",
                ["TotalAmount.float"] = "El monto debe ser un número válido.",
                ["TotalAmount.min"] = "El monto debe ser mayor a 0.",

                ["Active.required"] = "Debe indicar si el plan está activo."
            };
        }
    }
}