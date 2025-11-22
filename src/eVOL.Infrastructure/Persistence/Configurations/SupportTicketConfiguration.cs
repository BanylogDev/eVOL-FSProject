using eVOL.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(t => t.Category)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(t => t.Text)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(t => t.ClaimedStatus).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        // Relationships
        builder.HasOne(t => t.OpenedBy)
               .WithMany(u => u.OpenedTickets)
               .HasForeignKey(t => t.OpenedById)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ClaimedBy)
               .WithMany(u => u.ClaimedTickets) 
               .HasForeignKey(t => t.ClaimedById)
               .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many users
        builder.HasMany(t => t.SupportTicketUsers)
               .WithMany()
               .UsingEntity(j => j.ToTable("SupportTicketUsers"));
    }
}
