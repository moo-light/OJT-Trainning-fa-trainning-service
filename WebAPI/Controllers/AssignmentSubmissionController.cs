using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Application.ViewModels.GradingModels;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AssignmentSubmissionController : BaseController
    {
        private readonly IAssignmentSubmisstionService _assignmentSubmisstionService;
        private readonly IGradingService _gradingService;
        public AssignmentSubmissionController(IClaimsService claimsService,IAssignmentSubmisstionService assignmentSubmisstionService, IGradingService gradingService )
        {
            _assignmentSubmisstionService = assignmentSubmisstionService;
            _gradingService = gradingService;
        }
        [Authorize(Roles = "Trainee")]
        [HttpPost]
        public async Task<IActionResult> SubmissAssignment(Guid assignmentID,Guid ClassId, IFormFile file)
        {
            var result = await _assignmentSubmisstionService.AddSubmisstion(assignmentID,ClassId, file);
            if (result==Guid.Empty)
            {
                return NoContent();
            }
            return CreatedAtAction("DownLoadSubmission", result);
        }
        [Authorize(Roles = "Trainee")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSubmission(Guid SubbmissionID)
        {
            var result = await _assignmentSubmisstionService.RemoveSubmisstion(SubbmissionID);
            if (result == false) { return BadRequest(); }
            return Ok("Remove Successfully!");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownLoadSubmission(Guid submissionID)
        {
            var file = await _assignmentSubmisstionService.DownloadSubmiss(submissionID);
            if (file == null) return BadRequest();

            return File(file.FileData, FileUtils.GetMimeTypes()[file.FileType], file.FileName);
        }

        [Authorize(Roles = "Trainee")]
        [HttpPut]
        public async Task<IActionResult> EditSubmission(Guid submissionID, IFormFile file)
        {
            var result = await _assignmentSubmisstionService.EditSubmisstion(submissionID, file);
            if (result == false)
            {
                return BadRequest();
            }
            return Ok("Successfully");
        }

        [Authorize(Roles = "SuperAdmin,Admin,Trainer,Mentor")]
        [HttpPut]
        public async Task<IActionResult> GradingReview(Guid submissionID, int gradeNumber, string letterGrade, string comment, Guid detailTrainingClassID)
        {
            var lecture = await _assignmentSubmisstionService.GradingandReviewSubmission(submissionID, gradeNumber, comment);
            if (lecture == Guid.Empty) { return BadRequest(); }

            await _gradingService.AddToGrading(new GradingModel(lecture, detailTrainingClassID, letterGrade, gradeNumber));
            return Ok();
        }
    }
}
