using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AssignmentSubmissionService : IAssignmentSubmisstionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        public AssignmentSubmissionService(IUnitOfWork unitOfWork, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
        }

        public async Task<Guid> AddSubmisstion(Guid assignmentID,Guid ClassId, IFormFile file)
        {
            //Check User already join class
            var createdBy = _claimsService.GetCurrentUserId;
            var checkJoinClass = await _unitOfWork.DetailTrainingClassParticipate.FindAsync(p=>p.TrainingClassID==ClassId && p.UserId==createdBy);
            if(checkJoinClass.Count() == 0) throw new Exception("User not join in class!");
            var assignment = await _unitOfWork.AssignmentRepository
                 .FindAsync(a =>(
                 a.Id == assignmentID &&
                 a.IsOverDue == true &&
                 a.IsDeleted == false )|| 
                ( a.Id == assignmentID &&
                 a.IsDeleted == true));
            if (assignment.Count()==1) throw new Exception("Assignment is overdue!");
            var checkSubmiss = await _unitOfWork.AssignmentSubmissionRepository
                 .FindAsync(a =>
                 a.AssignmentId == assignmentID &&
                 a.IsDeleted == false &&
                 a.CreatedBy == createdBy
                 );
            if (checkSubmiss.Count > 0) throw new Exception("Submission has already existed!");
            var dbPath = file.ImportFile("AssignmentSubmissions", 1, _claimsService.GetCurrentUserId);
            var submission = new AssignmentSubmission
            {
                Id=Guid.NewGuid(),
                FileName = dbPath,
                Version = 1,
                AssignmentId = assignmentID,
            };
            await _unitOfWork.AssignmentSubmissionRepository.AddAsync(submission);
            await _unitOfWork.SaveChangeAsync();
            return submission.Id; ;
        }

        public async Task<FileEntity> DownloadSubmiss(Guid assignmentSubmissId)
        {
            var submission = await _unitOfWork.AssignmentSubmissionRepository.GetByIdAsync(assignmentSubmissId);
            if (submission == null) throw new Exception("Submission is not existed!");
            if (submission.IsDeleted == true) throw new Exception("Submission had been deleted!");
            FileEntity fileEntity = new FileEntity();
            fileEntity = submission.FileName.GetFileEntity();
            return fileEntity;
        }

        public async Task<bool> RemoveSubmisstion(Guid assignmentSubmissId)
        {
            var submiss = await _unitOfWork.AssignmentSubmissionRepository.GetByIdAsync(assignmentSubmissId);
            if (submiss == null) throw new Exception("Submission is not existed!");

            if (submiss.IsDeleted == true) { throw new Exception("Submission is also deleted!"); }
            submiss.IsDeleted = true;
            _unitOfWork.AssignmentSubmissionRepository.Update(submiss);
            var result = await _unitOfWork.SaveChangeAsync() > 0;
            return result;
        }

        public async Task<bool> EditSubmisstion(Guid assignmentID, IFormFile file)
        {
            var submission = await _unitOfWork.AssignmentSubmissionRepository.GetByIdAsync(assignmentID, s => s.Assignment);
            if (submission == null) throw new Exception("Submission is not existed!");
            if (submission.Assignment.IsOverDue == true) throw new Exception("Assignment is overdue!");
            var dbPath = file.ImportFile("AssignmentSubmissions", submission.Version.Value + 1, _claimsService.GetCurrentUserId);

            submission.FileName = dbPath;
            submission.Version = submission.Version.Value + 1;

            _unitOfWork.AssignmentSubmissionRepository.Update(submission);
            var result = await _unitOfWork.SaveChangeAsync() > 0;

            return result;
        }

        public async Task<Guid> GradingandReviewSubmission(Guid assignmentSubmissId, int numberGrade, string comment)
        {

            var findSumission = await _unitOfWork.AssignmentSubmissionRepository.GetByIdAsync(assignmentSubmissId, s => s.Assignment);
            if (findSumission == null) throw new Exception("Submission Assignment is not exist!");
            findSumission.Grade = numberGrade;
            findSumission.Comment = comment;
            _unitOfWork.AssignmentSubmissionRepository.Update(findSumission);
            await _unitOfWork.SaveChangeAsync();
            return findSumission.Assignment.LectureID;

        }
    }
}
