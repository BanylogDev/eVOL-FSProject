using eVOL.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eVOL.Infrastructure.Persistence.Configurations
{
    public sealed class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup>
    {
        public void Configure(EntityTypeBuilder<ChatGroup> builder)
        {
            builder.ToTable("ChatGroups");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(g => g.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(g => g.TotalUsers)
                   .IsRequired();

            builder.Property(g => g.OwnerId)
                   .IsRequired();

            builder.Property(g => g.CreatedAt)
                   .IsRequired();

            // Relationship: ChatGroup has many Users and Users have many Chat Groups
            builder.HasMany(g => g.GroupUsers)
                   .WithMany(u => u.ChatGroups)
                   .UsingEntity(j => j.ToTable("ChatGroupUsers"));
        }

    }
}
