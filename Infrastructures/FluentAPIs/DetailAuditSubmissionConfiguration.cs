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
    public class DetailAuditSubmissionConfiguration : IEntityTypeConfiguration<DetailAuditSubmission>
    {
        public void Configure(EntityTypeBuilder<DetailAuditSubmission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasDefaultValueSql("NEWID()");
            builder.Property(x => x.CreationDate).HasDefaultValueSql("getutcdate()");
            builder.HasOne(x => x.DetailAuditQuestion).WithMany(x => x.DetailAuditSubmissions).HasForeignKey(x => x.DetailAuditQuestionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
