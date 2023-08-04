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
    public class DetailTrainingProgramSyllabusConfiguration : IEntityTypeConfiguration<DetailTrainingProgramSyllabus>
    {
        public void Configure(EntityTypeBuilder<DetailTrainingProgramSyllabus> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(dt => dt.IsDeleted).HasDefaultValue(false);

            builder.HasOne(dt => dt.Syllabus).WithMany(dt => dt.DetailTrainingProgramSyllabus).HasForeignKey(dt => dt.SyllabusId);
            builder.HasOne(dt => dt.TrainingProgram).WithMany(tp => tp.DetailTrainingProgramSyllabus).HasForeignKey(dt => dt.TrainingProgramId);
        }



    }
}
