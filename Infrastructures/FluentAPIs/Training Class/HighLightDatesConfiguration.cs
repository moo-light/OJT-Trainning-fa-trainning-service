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
    public class HighLightDatesConfiguration : IEntityTypeConfiguration<HighlightedDates>
    {
        public void Configure(EntityTypeBuilder<HighlightedDates> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.Property(x => x.IsDeleted).HasDefaultValue("false");
            builder.HasOne(x => x.TrainingClassTimeFrame).WithMany(x => x.HighlightedDates).HasForeignKey(x => x.TrainingClassTimeFrameId);
        }
    }
}
