using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.GradingModels;
using Application.ViewModels.QuizModels;
using AutoMapper;
using Domain.Entities;
using Hangfire;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class GradingService : IGradingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentTime _currentTime;
    private readonly AppConfiguration _configuration;
    private readonly IClaimsService _claimsService;

    public GradingService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, AppConfiguration configuration, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentTime = currentTime;
        _configuration = configuration;
        _claimsService = claimsService;
    }
    public async Task CreateGradingAsync(GradingModel model)
    {
        var grading = _mapper.Map<Grading>(model);
        await _unitOfWork.GradingRepository.AddAsync(grading);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<bool> DeleteGradingAsync(Guid gradingId)
    {
        var grading = await _unitOfWork.GradingRepository.GetByIdAsync(gradingId);
        if (grading == null)
        {
            return false;
        }
        _unitOfWork.GradingRepository.SoftRemove(grading);
        await _unitOfWork.SaveChangeAsync();
        return true;
    }

    public async Task<List<Grading>> GetAllGradingsAsync()
    {
        var gradings = await _unitOfWork.GradingRepository.GetAllAsync();
        return gradings;
    }

    public async Task<Grading> GetGradingsAsync(Guid gradingId)
    {
        var grading = await _unitOfWork.GradingRepository.GetByIdAsync(gradingId);
        return grading;
    }

    public List<MarkReportDto> GetMarkReportOfClass(Guid classID)
    {
        var result = _unitOfWork.GradingRepository.GetMarkReportOfClass(classID);
        return result;
    }

    public List<MarkReportDto> GetMarkReportOfTrainee(Guid traineeId)
    {
        var result = _unitOfWork.GradingRepository.GetMarkReportOfTrainee(traineeId);
        return result;
    }

    public async Task<bool> UpdateGradingAsync(Guid gradingId, GradingModel model)
    {
        var grading = await _unitOfWork.GradingRepository.GetByIdAsync(gradingId);
        if (grading == null)
        {
            return false;
        }
        grading.LectureId = model.LectureId;
        grading.DetailTrainingClassParticipateId = model.DetailTrainingClassParticipateId;
        grading.LetterGrade = model.LetterGrade;
        grading.NumericGrade = model.NumericGrade;
        _unitOfWork.GradingRepository.Update(grading);
        await _unitOfWork.SaveChangeAsync();
        return true;
    }

    public async Task<double> ViewMarkQuizByQuizID(Guid LectureID)
    {
        var grading_find = await _unitOfWork.GradingRepository.GetAllAsync();
        return (double)grading_find.Find(x => x.LectureId == LectureID).NumericGrade;
    }
    public async Task<List<ViewQuizAndMarkBelowDTO>> ViewAllQuizMark()
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(_claimsService.GetCurrentUserId);
        return _unitOfWork.GradingRepository.GetAllMarkOfTrainee(user.Id);
    }

    public async Task<bool> AddToGrading(GradingModel gradingModel)
    {
        var checkLecture = await _unitOfWork.LectureRepository.GetByIdAsync(gradingModel.LectureId);
        var checkDetailTrainingID = await _unitOfWork.DetailTrainingClassParticipate.GetByIdAsync(gradingModel.DetailTrainingClassParticipateId);
        var mapperGrading = _mapper.Map<Grading>(gradingModel);
        if (checkLecture == null || checkDetailTrainingID == null) return false;
        await _unitOfWork.GradingRepository.AddAsync(mapperGrading);
        return await _unitOfWork.SaveChangeAsync() > 0;

    }
    public async Task UpdateGradingReports()
    {        
        await _unitOfWork.GradingRepository.UpdateGradingReports();
        await _unitOfWork.SaveChangeAsync();
    }
}
