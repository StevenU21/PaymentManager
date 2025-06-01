using PaymentManager.Models;
using PaymentManager.Validators;

namespace PaymentManager.Services
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;

        public UserValidationService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(User user, bool isEdit = false)
        {
            var nameError = UserValidator.ValidateName(user.Name);
            if (nameError != null) return (false, nameError);

            var emailError = UserValidator.ValidateEmail(user.Email);
            if (emailError != null) return (false, emailError);

            var phoneError = UserValidator.ValidatePhone(user.Phone);
            if (phoneError != null) return (false, phoneError);

            var users = await _userService.GetUsersAsync();
            var usersToValidate = isEdit ? users.Where(u => u.Id != user.Id) : users;

            var uniqueEmailError = UserValidator.ValidateUniqueEmail(user.Email, usersToValidate);
            if (uniqueEmailError != null) return (false, uniqueEmailError);

            var uniquePhoneError = UserValidator.ValidateUniquePhone(user.Phone ?? string.Empty, usersToValidate);
            if (uniquePhoneError != null) return (false, uniquePhoneError);

            return (true, null);
        }
    }
}