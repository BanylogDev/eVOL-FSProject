using eVOL.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVOL.Infrastructure.Persistence.Configurations
{
    public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");

            builder.HasKey(m => m.MessageId);
            builder.Property(m => m.MessageId)
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.Text)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(m => m.SenderId)
                   .IsRequired();

            builder.Property(m => m.ReceiverId)
                   .IsRequired();

            builder.Property(m => m.CreatedAt)
                   .IsRequired();

            // Indexes
            builder.HasIndex(m => m.SenderId);
            builder.HasIndex(m => m.ReceiverId);
        }
    }
}
