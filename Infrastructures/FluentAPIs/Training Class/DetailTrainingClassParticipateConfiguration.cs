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
    public class DetailTrainingClassParticipateConfiguration : IEntityTypeConfiguration<DetailTrainingClassParticipate>
    {
        public void Configure(EntityTypeBuilder<DetailTrainingClassParticipate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue("false");
            builder.Property(x => x.TraineeParticipationStatus).HasDefaultValue("null");
            builder.HasOne(x => x.TrainingClass).WithMany(x => x.TrainingClassParticipates).HasForeignKey(x => x.TrainingClassID);
            builder.HasOne(x => x.User).WithMany(x => x.DetailTrainingClassParticipate).HasForeignKey(x => x.UserId);

        }
    }
}
