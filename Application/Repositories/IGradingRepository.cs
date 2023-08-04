using Application.ViewModels.GradingModels;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories;

public interface IGradingRepository : IGenericRepository<Grading>
{
    List<MarkReportDto> GetMarkReportOfClass(Guid classID);
    List<MarkReportDto> GetMarkReportOfTrainee(Guid traineeId);

    List<ViewQuizAndMarkBelowDTO> GetAllMarkOfTrainee(Guid traineeId);
    Task UpdateGradingReports();
}
