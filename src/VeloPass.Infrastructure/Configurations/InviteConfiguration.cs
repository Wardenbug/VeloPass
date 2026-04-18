using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VeloPass.Domain.Invites;
using VeloPass.Domain.Organizations;
using VeloPass.Domain.Users;

namespace VeloPass.Infrastructure.Configurations;

internal sealed class InviteConfiguration : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("invites");
        
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.OrganizationRole).HasConversion<string>().HasMaxLength(32);
        
        builder.HasIndex(i => i.Token).IsUnique();
        
        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.InvitedByUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}