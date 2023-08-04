using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Feedback : BaseEntity
{
    public string FeedbackTitle { get; set; } = null!;

    public string FeedbackLink { get; set; } = null!;

    public User? User { get; set; }

    public Guid? TrainingCLassId { get; set; }

    public TrainingClass? TrainingClass { get; set; }

}
