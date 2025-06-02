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
                ["UserId"] = "required|integer|min:1",
                ["PaymentTypeId"] = "required|integer|min:1",
                ["DayOfMonth"] = "required|integer|min:1|max:31",
                ["Amount"] = "required|float",
                ["StartDate"] = "required|date",
                ["Active"] = "required"
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["UserId.required"] = "El usuario es obligatorio.",
                ["UserId.integer"] = "El usuario seleccionado no es válido.",
                ["UserId.min"] = "Debe seleccionar un usuario.",

                ["PaymentTypeId.required"] = "El tipo de pago es obligatorio.",
                ["PaymentTypeId.integer"] = "El tipo de pago seleccionado no es válido.",
                ["PaymentTypeId.min"] = "Debe seleccionar un tipo de pago.",

                ["DayOfMonth.required"] = "El día de pago es obligatorio.",
                ["DayOfMonth.integer"] = "El día de pago debe ser un número.",
                ["DayOfMonth.min"] = "El día de pago debe ser al menos 1.",
                ["DayOfMonth.max"] = "El día de pago no puede ser mayor a 31.",

                ["Amount.required"] = "El monto es obligatorio.",
                ["Amount.float"] = "El monto debe ser un número válido.",
                ["Amount.min"] = "El monto debe ser mayor a 0.",

                ["StartDate.required"] = "La fecha de inicio es obligatoria.",
                ["StartDate.date"] = "La fecha de inicio no es válida.",

                ["Active.required"] = "Debe indicar si el plan está activo."
            };
        }
    }
}