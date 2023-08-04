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
    public class ApplicationsConfiguration : IEntityTypeConfiguration<Domain.Entities.Applications>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Applications> builder)

        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasDefaultValueSql("NEWID()");
            builder.Property(a => a.Approved).HasDefaultValue(false);
            builder.Property(a => a.Reason).IsRequired();
        }
    }
}
