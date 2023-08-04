using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs
{
    internal class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasIndex(x => x.AssignmentName).IsUnique();
            builder.Property(x => x.AssignmentName).HasMaxLength(100);
            builder.Property(x => x.IsOverDue).HasDefaultValue(false);
            builder.Property(x => x.DeadLine).HasDefaultValue(default);
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue("False");
            builder.HasOne(x => x.Lecture).WithMany(l => l.Assignments).HasForeignKey(x => x.LectureID);
        }
    }
}
