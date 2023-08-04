using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.FluentAPIs;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.TrainingClass).WithMany(t => t.Feedbacks).HasForeignKey(x => x.TrainingCLassId);
        builder.HasOne(x => x.User).WithMany(u => u.Feedbacks).HasForeignKey(x => x.CreatedBy);
    }
}
