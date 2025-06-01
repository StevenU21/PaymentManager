using PaymentManager.Models;

namespace PaymentManager.Services
{
    public interface IUserValidationService
    {
        Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(User user, bool isEdit = false);
    }
}