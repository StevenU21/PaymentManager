using System.Text.RegularExpressions;

namespace PaymentManager.Validators
{
    public abstract class BaseValidator<T>
    {
        protected readonly IEnumerable<T> _items;

        protected BaseValidator(IEnumerable<T> items)
        {
            _items = items;
        }

        public abstract Dictionary<string, string> Rules();
        public abstract Dictionary<string, string> Messages();

        public ValidationResult Validate(T model)
        {
            var errors = new Dictionary<string, List<string>>();
            var rules = Rules();
            var messages = Messages();

            foreach (var kvp in rules)
            {
                var field = kvp.Key;
                var parts = kvp.Value.Split('|');
                var prop = typeof(T).GetProperty(field);
                var rawValue = prop != null ? prop.GetValue(model) : null;
                var value = rawValue?.ToString();

                foreach (var rulePart in parts)
                {
                    var seg = rulePart.Split(new[] { ':' }, 2);
                    var name = seg[0];
                    var param = seg.Length > 1 ? seg[1] : null;

                    if (!ApplyRule(name, param ?? string.Empty, value ?? string.Empty, field))
                    {
                        var key = $"{field}.{name}";
                        var msg = messages.ContainsKey(key)
                            ? messages[key]
                            : $"El campo {field} no cumple la regla {name}.";
                        if (!errors.ContainsKey(field)) errors[field] = new List<string>();
                        errors[field].Add(msg);
                    }
                }
            }

            return new ValidationResult(errors);
        }

        private bool ApplyRule(string rule, string param, string value, string field)
        {
            switch (rule)
            {
                case "required":
                    return !string.IsNullOrWhiteSpace(value);
                case "min":
                    return value?.Length >= int.Parse(param);
                case "max":
                    return value?.Length <= int.Parse(param);
                case "regex":
                    return Regex.IsMatch(value ?? "", param);
                case "email":
                    return Regex.IsMatch(value ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                case "digits":
                    return Regex.IsMatch(value ?? "", @"^\d+$");
                case "between":
                    var nums = param.Split(',');
                    int min = int.Parse(nums[0]), max = int.Parse(nums[1]);
                    return value?.Length >= min && value.Length <= max;
                case "unique":
                    var p = typeof(T).GetProperty(param);
                    if (p == null) return true;
                    return !_items.Any(x =>
                        string.Equals(p.GetValue(x)?.ToString() ?? string.Empty, value ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                case "integer":
                    return int.TryParse(value, out _);
                case "float":
                    return float.TryParse(value, out _);
                case "date":
                    return DateTime.TryParse(value, out _);
                case "datetime":
                    return DateTime.TryParse(value, out _);
                default:
                    return true;
            }
        }
    }
}