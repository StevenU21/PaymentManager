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
        public DbSet<PaymentStatus> PaymentStatuses => Set<PaymentStatus>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PaymentStatus: relación uno-a-uno con User
            modelBuilder.Entity<PaymentStatus>()
                .HasKey(ps => ps.UserId);

            modelBuilder.Entity<PaymentStatus>()
                .HasOne(ps => ps.User)
                .WithOne(u => u.PaymentStatus)
                .HasForeignKey<PaymentStatus>(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment → User (muchos a uno)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment → PaymentMethod (muchos a uno, nullable)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            // Payment → PaymentPlan (muchos a uno, nullable)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentPlan)
                .WithMany(pp => pp.Payments)
                .HasForeignKey(p => p.PaymentPlanId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
