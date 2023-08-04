using Domain.Entities.TrainingClassRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs.Training_Class
{
    public class TrainingClassAttendeesConfiguration : IEntityTypeConfiguration<TrainingClassAttendees>
    {
        public void Configure(EntityTypeBuilder<TrainingClassAttendees> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasOne(x => x.TrainingClass).WithOne(x => x.TrainingClassAttendee).HasForeignKey<TrainingClassAttendees>(x => x.TrainingClassId);
        }
    }
}
