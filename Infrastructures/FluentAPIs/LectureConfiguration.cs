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
    public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
    {
        public void Configure(EntityTypeBuilder<Lecture> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.HasOne<Quiz>(x => x.Quiz).WithOne(x => x.Lecture).HasForeignKey<Quiz>(x => x.LectureID);
        }
    }
}
