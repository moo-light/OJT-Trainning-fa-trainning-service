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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.HasOne(x => x.Topic).WithMany(x => x.QuizBanks).HasForeignKey(x => x.TopicID);
            builder.HasOne(x => x.QuizType).WithMany(x => x.QuizBanks).HasForeignKey(x => x.QuizTypeID);

            //builder.HasMany(x => x.QuizTest).WithOne(x => x.QuizBank);
        }
    }
}
