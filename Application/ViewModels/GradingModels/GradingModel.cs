using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.GradingModels;

public class GradingModel
{
    public GradingModel(Guid lectureId, Guid detailTrainingClassParticipateId, string? letterGrade, double? numericGrade)
    {
        LectureId = lectureId;
        DetailTrainingClassParticipateId = detailTrainingClassParticipateId;
        LetterGrade = letterGrade;
        NumericGrade = numericGrade;
    }

    public Guid LectureId { get; set; }
    public Guid DetailTrainingClassParticipateId { get; set; }
    public string? LetterGrade { get; set; }
    public double? NumericGrade { get; set; }

}
