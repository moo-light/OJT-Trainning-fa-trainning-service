using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.FeedbackModels;

public class FeedbackModel
{
    public string FeedbackTitle { get; set; } = null!;
    public string FeedbackLink { get; set; } = null!;
    public Guid? TrainingCLassId { get; set; }

}
public class FeedbackVM : FeedbackModel
{
    public Guid Id { get; set; }
    public Guid? CreatedBy { get; set; }
    public User? User { get; set; }
    public TrainingClass? TrainingClass { get; set; }
}
