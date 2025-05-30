﻿using System;
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
        public DbSet<PaymentType> PaymentTypes => Set<PaymentType>();
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

            // PaymentPlan → User
            modelBuilder.Entity<PaymentPlan>()
                .HasOne(pp => pp.User)
                .WithMany(u => u.PaymentPlans)
                .HasForeignKey(pp => pp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentPlan → PaymentType
            modelBuilder.Entity<PaymentPlan>()
                .HasOne(pp => pp.PaymentType)
                .WithMany(pt => pt.PaymentPlans)
                .HasForeignKey(pp => pp.PaymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

    }
}
