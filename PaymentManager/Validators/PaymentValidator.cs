using PaymentManager.Models;
using System.Collections.Generic;

namespace PaymentManager.Validators
{
    public class PaymentValidator : BaseValidator<Payment>
    {
        public PaymentValidator(IEnumerable<Payment> payments) : base(payments) { }

        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["UserId"] = "required|integer|min:1",
                ["PaymentDate"] = "required|date",
                ["AmountPaid"] = "required|float",
                ["PeriodsPaid"] = "required|integer|min:1",
                ["NextDueDate"] = "required|date",
                ["PaymentMethodId"] = "required|integer|min:1",
                ["PaymentPlanId"] = "required|integer|min:1"
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["UserId.required"] = "El usuario es obligatorio.",
                ["UserId.integer"] = "El usuario seleccionado no es válido.",
                ["UserId.min"] = "Debe seleccionar un usuario.",

                ["PaymentDate.required"] = "La fecha de pago es obligatoria.",
                ["PaymentDate.date"] = "La fecha de pago no es válida.",

                ["AmountPaid.required"] = "El monto pagado es obligatorio.",
                ["AmountPaid.float"] = "El monto pagado debe ser un número válido.",
                ["AmountPaid.min"] = "El monto pagado debe ser mayor a 0.",

                ["PeriodsPaid.required"] = "El número de periodos pagados es obligatorio.",
                ["PeriodsPaid.integer"] = "El número de periodos debe ser un número entero.",
                ["PeriodsPaid.min"] = "Debe pagar al menos un periodo.",

                ["NextDueDate.required"] = "La próxima fecha de vencimiento es obligatoria.",
                ["NextDueDate.date"] = "La próxima fecha de vencimiento no es válida.",

                ["PaymentMethodId.required"] = "El método de pago es obligatorio.",
                ["PaymentMethodId.integer"] = "El método de pago seleccionado no es válido.",
                ["PaymentMethodId.min"] = "Debe seleccionar un método de pago.",

                ["PaymentPlanId.required"] = "El plan de pago es obligatorio.",
                ["PaymentPlanId.integer"] = "El plan de pago seleccionado no es válido.",
                ["PaymentPlanId.min"] = "Debe seleccionar un plan de pago."
            };
        }
    }
}