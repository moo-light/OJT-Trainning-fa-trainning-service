using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class DetailTrainingClassParticipateController : BaseController
    {
        private readonly IDetailTrainingClassParticipateService _detailTrainingClassParticipateService;

        public DetailTrainingClassParticipateController(IDetailTrainingClassParticipateService detailTrainingClassParticipateService)
        {
            _detailTrainingClassParticipateService = detailTrainingClassParticipateService;
        }

        /// <summary>
        /// Update the user's training status to Joined for a given class.
        /// </summary>
        /// <param name="classId">The ID of the class to update.</param>
        /// <returns>Returns an <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <response code="200">Returns 200 if the update was successful.</response>
        /// <response code="400">Returns 400 if the classId and userid is invalid or the update failed.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateTrainingStatus(Guid classId)
        {
            bool participation = await _detailTrainingClassParticipateService.UpdateTrainingStatus(classId);
            if (participation)
            {
                return Ok();
            }
            return BadRequest();
        }
        /// <summary>
        /// Create new relation for user and class
        /// </summary>
        /// <param name="userId">user you want to add</param>
        /// <param name="classId">class you want to add</param>
        /// <returns></returns>
        /// <response code="200">Returns 200 if the Create was successful.</response>
        /// <response code="400">Returns 400 if the classId is invalid or the create failed.</response>
        [HttpPost]
        public async Task<IActionResult> CreateTrainingClassParticipate(Guid userId, Guid classId)
        {
            var create = await _detailTrainingClassParticipateService.CreateTrainingClassParticipate(userId, classId);
            if (create != null)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> SendInviteLink(string invLink, Guid classId)
        {
            try
            {
                await _detailTrainingClassParticipateService.SendInvitelink(invLink, classId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
