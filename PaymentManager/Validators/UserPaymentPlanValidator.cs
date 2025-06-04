using PaymentManager.Models;
using System.Collections.Generic;

namespace PaymentManager.Validators
{
    public class UserPaymentPlanValidator : BaseValidator<UserPaymentPlan>
    {
        public UserPaymentPlanValidator(IEnumerable<UserPaymentPlan> userPaymentPlans) : base(userPaymentPlans) { }

        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["UserId"] = "required|integer|min:1",
                ["PaymentPlanId"] = "required|integer|min:1",
                ["JoinDate"] = "required|date",
                ["Status"] = "required|min:2|max:20",
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

                ["PaymentPlanId.required"] = "El plan de pago es obligatorio.",
                ["PaymentPlanId.integer"] = "El plan de pago seleccionado no es válido.",
                ["PaymentPlanId.min"] = "Debe seleccionar un plan de pago.",

                ["JoinDate.required"] = "La fecha de inicio es obligatoria.",
                ["JoinDate.date"] = "La fecha de inicio no es válida.",

                ["Status.required"] = "El estado es obligatorio.",
                ["Status.min"] = "El estado debe tener al menos 2 caracteres.",
                ["Status.max"] = "El estado no puede superar los 20 caracteres.",

                ["Active.required"] = "Debe indicar si el plan está activo."
            };
        }
    }
}