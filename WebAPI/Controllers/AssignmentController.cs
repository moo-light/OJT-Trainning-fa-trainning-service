using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.AssignmentModel;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace WebAPI.Controllers
{
    public class AssignmentController : BaseController
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IRecurringJobManager _recurringJobManager;

        public AssignmentController(IAssignmentService assignmentService, IRecurringJobManager recurringJobManager)
        {
            _assignmentService = assignmentService;
            _recurringJobManager = recurringJobManager;
        }

        [Authorize(Roles = "SuperAdmin,Admin,Trainer,Mentor")]
        [HttpPut]
        public async Task<IActionResult> UpdateAssignment([FromForm] AssignmentUpdateModel assignmentUpdate)
        {
            var result = await _assignmentService.UpdateAssignment(assignmentUpdate);
            if (result == false)
            {
                return BadRequest();
            }
            return Ok();
        }
        [Authorize(Roles = "SuperAdmin,Admin,Trainer,Mentor")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAssignment(Guid assignmentID)
        {
            var result = await _assignmentService.DeleteAssignment(assignmentID);
            if (result == false)
            {
                return BadRequest();
            }
            return Ok();
        }

        [Authorize(Roles = "SuperAdmin,Admin,Trainer,Mentor")]
        [HttpGet]
        public async Task<IActionResult> ViewAssignmentByLectureID(Guid lectureID)
        {
            var result = await _assignmentService.GetAllAssignmentByLectureID(lectureID);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [Authorize(Roles = "SuperAdmin,Admin,Trainer,Mentor")]
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromForm] AssignmentViewModel assignmentViewModel)
        {
            var result = await _assignmentService.CreateAssignment(assignmentViewModel);
            if (result==Guid.Empty)
            {
                return NoContent();
            }
            return CreatedAtAction("DownloadAssignment", result);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadAssignment(Guid assignmentID)
        {
            var result = await _assignmentService.DownLoad(assignmentID);
            if (result == null)
            {
                return BadRequest();
            }
            return File(result.FileData, FileUtils.GetMimeTypes()[result.FileType], result.FileName);
        }
    }
}
