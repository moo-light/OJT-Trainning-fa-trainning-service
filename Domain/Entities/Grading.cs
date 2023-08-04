using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Grading : BaseEntity
{
    public string? LetterGrade { get; set; }
    public double? NumericGrade { get; set; }
    public Guid DetailTrainingClassParticipateId { get; set; }

    public virtual DetailTrainingClassParticipate DetailTrainingClassParticipate { get; set; }

    public Guid LectureId { get; set; }

    public virtual Lecture Lecture { get; set; }

    //public ICollection<SubmitQuiz> Quiz { get; set; }
}
