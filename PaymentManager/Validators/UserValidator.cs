using PaymentManager.Models;

namespace PaymentManager.Validators
{
    public class UserValidator : BaseValidator<User>
    {
        public UserValidator(IEnumerable<User> users) : base(users) { }
        public override Dictionary<string, string> Rules()
        {
            return new Dictionary<string, string>
            {
                ["Name"]  = "required|min:3|max:50|regex:^[a-zA-ZáéíóúÁÉÍÓÚñÑ\\s]+$",
                ["Email"] = "required|email|max:100|unique:Email",
                ["Phone"] = "required|digits|between:8,15|unique:Phone"
            };
        }

        public override Dictionary<string, string> Messages()
        {
            return new Dictionary<string, string>
            {
                ["Name.required"]  = "El nombre es obligatorio.",
                ["Name.min"]       = "El nombre debe tener al menos 3 caracteres.",
                ["Name.max"]       = "El nombre no puede superar los 50 caracteres.",
                ["Name.regex"]     = "El nombre solo puede contener letras y espacios.",

                ["Email.required"] = "El correo electrónico es obligatorio.",
                ["Email.email"]    = "El correo electrónico no es válido.",
                ["Email.max"]      = "El correo electrónico no puede superar los 100 caracteres.",
                ["Email.unique"]   = "El correo electrónico ya está registrado.",

                ["Phone.required"] = "El teléfono es obligatorio.",
                ["Phone.digits"]   = "El teléfono solo debe contener números.",
                ["Phone.between"]  = "El teléfono debe tener entre 8 y 15 dígitos.",
                ["Phone.unique"]   = "El teléfono ya está registrado."
            };
        }
    }
}