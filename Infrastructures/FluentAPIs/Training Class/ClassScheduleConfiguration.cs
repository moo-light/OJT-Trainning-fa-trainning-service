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
    public class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
    {
        public void Configure(EntityTypeBuilder<ClassSchedule> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue("false");
            builder.Property(x => x.StatusClassSchedule).HasDefaultValue("null");
            builder.Property(x => x.ClassStartTime).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.ClassEndTime).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.RoomName).HasDefaultValue("null");
            builder.HasOne(x => x.TrainingClasses).WithMany(x => x.ClassSchedules).HasForeignKey(x => x.TrainingClassId);
        }
    }
}
