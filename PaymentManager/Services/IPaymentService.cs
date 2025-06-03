using PaymentManager.Models;

namespace PaymentManager.Services
{
    public interface IPaymentService : IBaseService<Payment>
    {
        public DateTime? CalculateNextDueDate(Payment payment, PaymentPlan plan)
        {
            if (plan?.DayOfMonth == null) return null;

            var today = DateTime.Now.Date;
            var dayOfMonth = plan.DayOfMonth.Value.Day;
            var nextMonth = today.Month;
            var nextYear = today.Year;

            if (today.Day >= dayOfMonth)
            {
                nextMonth++;
                if (nextMonth > 12)
                {
                    nextMonth = 1;
                    nextYear++;
                }
            }

            int periodsPaid = payment.PeriodsPaid > 0 ? payment.PeriodsPaid : 1;
            nextMonth += (periodsPaid - 1);

            while (nextMonth > 12)
            {
                nextMonth -= 12;
                nextYear++;
            }

            var daysInNextMonth = DateTime.DaysInMonth(nextYear, nextMonth);
            var dueDay = Math.Min(dayOfMonth, daysInNextMonth);

            return new DateTime(nextYear, nextMonth, dueDay, payment.PaymentDate.Hour, payment.PaymentDate.Minute, payment.PaymentDate.Second);
        }
    }
}
