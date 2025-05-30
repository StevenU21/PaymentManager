using PaymentManager.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace PaymentManager.Validators
{
    public static class UserValidator
    {
        public static string? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "El nombre es obligatorio.";
            if (name.Length < 3)
                return "El nombre debe tener al menos 3 caracteres.";
            if (name.Length > 50)
                return "El nombre no puede superar los 50 caracteres.";
            if (!Regex.IsMatch(name, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
                return "El nombre solo puede contener letras y espacios.";
            return null;
        }

        public static string? ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "El correo electrónico es obligatorio.";
            if (email.Length > 100)
                return "El correo electrónico no puede superar los 100 caracteres.";
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "El correo electrónico no es válido.";
            return null;
        }

        public static string? ValidatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "El teléfono es obligatorio.";
            if (!Regex.IsMatch(phone, @"^\d+$"))
                return "El teléfono solo debe contener números.";
            if (phone.Length < 8)
                return "El teléfono debe tener al menos 8 dígitos.";
            if (phone.Length > 15)
                return "El teléfono no puede superar los 15 dígitos.";
            return null;
        }

        public static string? ValidateUniqueEmail(string email, IEnumerable<User> users)
        {
            if (users.Any(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase)))
                return "El correo electrónico ya está registrado.";
            return null;
        }

        public static string? ValidateUniquePhone(string phone, IEnumerable<User> users)
        {
            if (users.Any(u => u.Phone == phone))
                return "El teléfono ya está registrado.";
            return null;
        }
    }
}