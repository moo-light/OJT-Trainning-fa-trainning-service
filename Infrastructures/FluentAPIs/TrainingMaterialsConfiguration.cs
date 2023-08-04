using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs
{
    public class TrainingMaterialsConfiguration : IEntityTypeConfiguration<TrainingMaterial>
    {
        public void Configure(EntityTypeBuilder<TrainingMaterial> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.TMatName).HasDefaultValueSql("null");
            builder.Property(s => s.TMatType).HasDefaultValueSql("null");
            builder.Property(s => s.TMatDescription).HasDefaultValueSql("null");
            builder.Property(s => s.TMatURL).HasDefaultValueSql("null");
            builder.Property(s => s.BlobName).HasDefaultValueSql("null");
            builder.HasOne(s => s.Lecture).WithMany(u => u.TrainingMaterials).HasForeignKey(s => s.lectureID);
        }
    }
}
