using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels
{
    public class ViewAllTrainingProgramDTO
    {
        public Guid Id { get; set; }
        public string TrainingTitle { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public double Duration { get; set; }

        public string Status { get; set; } = default!;
        public ICollection<Syllabus>? Content { get; set; }

    }
}
