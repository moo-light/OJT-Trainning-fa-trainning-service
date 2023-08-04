using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs
{
    public class SyllabusConfiguration : IEntityTypeConfiguration<Syllabus>
    {
        public void Configure(EntityTypeBuilder<Syllabus> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.HasOne(s => s.User).WithMany(u => u.Syllabuses).HasForeignKey(s => s.UserId);
            builder.HasMany(s => s.Units).WithOne(u => u.Syllabus);
            builder.Property(x => x.Status).HasDefaultValueSql("null");
        }
    }
}
