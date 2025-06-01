using System.Linq;
using System.Threading.Tasks;
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
            var allUsers = await _userService.GetUsersAsync();
            var usersToValidate = isEdit
                ? allUsers.Where(u => u.Id != user.Id)
                : allUsers;

            var validator = new UserValidator(usersToValidate);
            var result = validator.Validate(user);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .SelectMany(kvp => kvp.Value)
                    .ToArray();
                var errorMessage = string.Join(" ", errors);
                return (false, errorMessage);
            }

            return (true, null);
        }
    }
}