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
    public class SubmitQuizConfiguration : IEntityTypeConfiguration<SubmitQuiz>
    {
        public void Configure(EntityTypeBuilder<SubmitQuiz> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            //builder.HasOne(x => x.User).WithMany(x => x.SubmitQuizzes).HasForeignKey(x => x.UserID);
            //builder.HasMany<DetailQuizQuestion>(x => x.).WithOne(x => x.submitQuiz).HasForeignKey<SubmitQuiz>(x => x.DetailQuizQuestionID);
            builder.HasOne(x => x.DetailQuizQuestion).WithMany(x => x.submitQuiz).HasForeignKey(x => x.DetailQuizQuestionID);
        }

    }
}
