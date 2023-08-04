using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.GradingModels;
using Application.ViewModels.QuizModels;
using Domain.Entities;

namespace Infrastructures.Repositories;

public class GradingRepository : GenericRepository<Grading>, IGradingRepository
{
    private readonly AppDbContext _context;

    public GradingRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
        _context = context;
    }
    public async Task UpdateGradingReports()
    {
        // sumary grading data
        var result = from g in _context.Gradings
                     join d in _context.DetailTrainingClassParticipates
                            on g.DetailTrainingClassParticipateId equals d.Id
                     join u in _context.Users
                            on d.UserId equals u.Id
                     join c in _context.TrainingClasses
                             on d.TrainingClassID equals c.Id
                     join l in _context.Lectures
                            on g.LectureId equals l.Id                     
                     select new GradingReport()
                     {
                         ClassId = c.Id,
                         ClassName = c.Name,
                         TraineeId = u.Id,
                         Username = u.UserName,
                         TraineeName = u.FullName,
                         LectureName = l.LectureName,
                         DeliveryType = l.DeliveryType,
                         NumericGrade = g.NumericGrade.Value,
                         LetterGrade = g.LetterGrade
                     };
        // update new grading report datas into database 
        _context.GradingReports.RemoveRange(_context.GradingReports);
        await _context.SaveChangesAsync();
        await _context.GradingReports.AddRangeAsync(result);
    }


    public List<MarkReportDto> GetMarkReportOfClass(Guid classID)
    {
        var result = _context.GradingReports
            .Where(x => x.ClassId == classID)
            .Select(x => new MarkReportDto()
            {
                ClassName = x.ClassName,
                Username = x.Username,
                TraineeName = x.TraineeName,
                DeliveryType = x.DeliveryType,
                LectureName = x.LectureName,
                LetterGrade = x.LetterGrade,
                NumericGrade = x.NumericGrade
            });   
        return result.ToList();
    }
    public List<MarkReportDto> GetMarkReportOfTrainee(Guid traineeId)
    {
        var result = _context.GradingReports
            .Where(x => x.TraineeId == traineeId)
            .Select(x => new MarkReportDto()
            {
                ClassName = x.ClassName,
                Username = x.Username,
                TraineeName = x.TraineeName,
                DeliveryType = x.DeliveryType,
                LectureName = x.LectureName,
                LetterGrade = x.LetterGrade,
                NumericGrade = x.NumericGrade
            });
        return result.ToList();
    }
    public List<ViewQuizAndMarkBelowDTO> GetAllMarkOfTrainee(Guid traineeId)
    {
        List<ViewQuizAndMarkBelowDTO> listMark = new List<ViewQuizAndMarkBelowDTO>();

        var result = from detailtraining in _context.DetailTrainingClassParticipates
                     join grading in _context.Gradings on detailtraining.Id equals grading.DetailTrainingClassParticipateId
                     join lecture in _context.Lectures on grading.LectureId equals lecture.Id
                     join quiz in _context.Quizzes on lecture.QuizID equals quiz.Id
                     where detailtraining.UserId == traineeId
                     select new ViewQuizAndMarkBelowDTO()
                     {
                         QuizMark = (int)grading.NumericGrade,
                         QuizName = quiz.QuizName

                     };
        foreach (var item in result)
        {
            listMark.Add(item);
        }
        return listMark;
    }

}
