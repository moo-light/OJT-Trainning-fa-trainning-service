using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.FluentAPIs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.UserName).HasMaxLength(100);
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.LoginDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.RoleId).HasDefaultValue("4");
            builder.Property(x => x.IsDeleted).HasDefaultValue("False");
            builder.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasMany(u => u.Applications).WithOne(r => r.User).HasForeignKey(u => u.UserId);
            builder.HasMany(u => u.Syllabuses).WithOne(r => r.User).HasForeignKey(u => u.UserId);
            builder.HasMany(u => u.Attendances).WithOne(r => r.User).HasForeignKey(u => u.UserId);
        }
    }
}
