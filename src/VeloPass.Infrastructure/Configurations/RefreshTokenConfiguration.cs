using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeloPass.Infrastructure.Authentication;

namespace VeloPass.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.ToTable("refresh_tokens");

        builder.HasKey(
            token => token.Id);

        builder.Property(
            token => token.UserId).HasMaxLength(300);

        builder.Property(
            token => token.Token).HasMaxLength(1000);

        builder.HasIndex(
            token => token.Token).IsUnique();

        builder.HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}