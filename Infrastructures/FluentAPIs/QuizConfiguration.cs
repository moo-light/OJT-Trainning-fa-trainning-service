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
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            //builder.HasOne(s => s.QuizBank).WithMany(x => x.QuizTest).HasForeignKey(x => x.QuizBankId);
            //builder.HasMany(x => x.submitQuizzes).WithOne(x => x.QuizTest).HasForeignKey(x => x.QUizTestId);
            //builder.HasOne(x => x.Lecture).WithOne(x => x.Quiz);
            builder.HasMany(x => x.DetailQuizQuestion).WithOne(x => x.Quiz).HasForeignKey(x => x.QuizID);


        }
    }
}
