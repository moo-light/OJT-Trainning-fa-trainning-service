using Application.ViewModels.GradingModels;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IGradingService
{
    Task<List<Grading>> GetAllGradingsAsync();
    Task<Grading> GetGradingsAsync(Guid gradingId);
    Task<bool> UpdateGradingAsync(Guid gradingId, GradingModel model);
    Task<bool> DeleteGradingAsync(Guid gradingId);
    Task CreateGradingAsync(GradingModel model);
    List<MarkReportDto> GetMarkReportOfClass(Guid classID);
    List<MarkReportDto> GetMarkReportOfTrainee(Guid traineeId);
    Task<bool> AddToGrading(GradingModel gradingModel);
    Task UpdateGradingReports();
    Task<List<ViewQuizAndMarkBelowDTO>> ViewAllQuizMark();
    Task<double> ViewMarkQuizByQuizID(Guid LectureID);
}
