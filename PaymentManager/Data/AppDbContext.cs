using System;
using Microsoft.EntityFrameworkCore;
using PaymentManager.Models;

namespace PaymentManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentPlan> PaymentPlans => Set<PaymentPlan>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<UserPaymentPlan> UserPaymentPlans => Set<UserPaymentPlan>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User → UserPaymentPlan (uno a muchos)
            modelBuilder.Entity<UserPaymentPlan>()
                .HasOne(upp => upp.User)
                .WithMany(u => u.UserPaymentPlans)
                .HasForeignKey(upp => upp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentPlan → UserPaymentPlan (uno a muchos)
            modelBuilder.Entity<UserPaymentPlan>()
                .HasOne(upp => upp.PaymentPlan)
                .WithMany(pp => pp.UserPaymentPlans)
                .HasForeignKey(upp => upp.PaymentPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserPaymentPlan → Payment (uno a muchos)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.UserPaymentPlan)
                .WithMany(upp => upp.Payments)
                .HasForeignKey(p => p.UserPaymentPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment → PaymentMethod (muchos a uno, nullable)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}