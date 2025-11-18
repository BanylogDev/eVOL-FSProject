using eVOL.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVOL.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users"); // Table

            // Key

            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId)
                   .ValueGeneratedOnAdd();
             
            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Columns

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("User");

            builder.Property(u => u.CreatedAt)
                   .IsRequired();

            builder.Property(u => u.AccessToken)
                   .HasMaxLength(2048);

            builder.Property(u => u.RefreshToken)
                   .HasMaxLength(256);

            // Indexes & uniqueness
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Name).IsUnique();

            // Address ValueObject
            builder.OwnsOne(u => u.Address, addr =>
            {
                addr.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100);
                addr.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100);
                addr.Property(a => a.AddressName)
                    .IsRequired()
                    .HasMaxLength(150);
                addr.Property(a => a.AddressNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                addr.ToTable("Users"); // Set To Table
            });

            // Money ValueObject
            builder.OwnsOne(u => u.Money, money =>
            {
                money.Property(m => m.Balance)
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");

                money.Property(m => m.Currency)
                     .IsRequired()
                     .HasMaxLength(3);

                money.ToTable("Users"); // Set To Table
            });
        }
    }
}
