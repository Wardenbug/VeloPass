using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;

namespace VeloPass.Infrastructure.Configurations;

internal sealed class OrganizationMembersConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ToTable("organization_members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(m => new { m.OrganizationId, m.UserId }).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}