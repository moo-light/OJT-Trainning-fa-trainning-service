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
    public class TrainingClassTraineesConfiguration : IEntityTypeConfiguration<TrainingClassTrainer>
    {
        public void Configure(EntityTypeBuilder<TrainingClassTrainer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.HasOne(x => x.TrainingClass).WithMany(x => x.TrainingClassTrainers).HasForeignKey(x => x.TrainingClassId);
        }
    }
}
