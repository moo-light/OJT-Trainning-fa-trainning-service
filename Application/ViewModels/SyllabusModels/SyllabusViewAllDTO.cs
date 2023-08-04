using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels;

public class SyllabusViewAllDTO
{
    public Guid ID { get; set; }
    public string SyllabusName { get; set; }
    public string Code { get; set; }
    // public string syllabusStatus { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public DurationView Duration { get; set; }
    public ICollection<string> OutputStandard { get; set; }
}


