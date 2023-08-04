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
    public class DetailQuizQuestionConfiguration : IEntityTypeConfiguration<DetailQuizQuestion>
    {
        public void Configure(EntityTypeBuilder<DetailQuizQuestion> builder)
        {
            //throw new NotImplementedException();
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasDefaultValueSql("NEWID()");
            builder.Property(s => s.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.HasOne(x => x.Question).WithMany(x => x.DetailQuizQuestion).HasForeignKey(x => x.QuestionID);

        }
    }
}
