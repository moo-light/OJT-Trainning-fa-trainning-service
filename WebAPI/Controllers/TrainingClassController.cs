using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.TrainingClassModels;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers
{
    // [Authorize]
    public class TrainingClassController : BaseController
    {
        private readonly ITrainingClassService _trainingClassService;
        private readonly IClaimsService _claimsService;
        private readonly IMapper _mapper;
        private ITrainingClassService @object;

        public TrainingClassController(ITrainingClassService trainingClassService, IClaimsService claimsService, IMapper mapper)
        {
            _trainingClassService = trainingClassService;
            _claimsService = claimsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> SearchClassByName(string name)
        {
            var result = await _trainingClassService.SearchClassByNameAsync(name);
            if (!result.IsNullOrEmpty())
            {
                return Ok(result);
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> DuplicateClass(Guid id)
        {
            var result = await _trainingClassService.DuplicateClassAsync(id);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// This method update training class
        /// </summary>
        /// <param name="trainingClassId">Training class id</param>
        /// <param name="classInput">Update training class api</param>
        /// <returns>Ok when class updated successfully; BadRequest when class updated fail or when there is error</returns>
        [HttpPatch]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> UpdateTrainingClassAsync(string trainingClassId, UpdateTrainingClassDTO2 classInput)
        {
            var classDTO = _mapper.Map<UpdateTrainingClassDTO>(classInput);
            return await UpdateTrainingClassAsync(trainingClassId, classDTO);
        }
        [NonAction]
        public virtual async Task<IActionResult> UpdateTrainingClassAsync(string trainingClassId, UpdateTrainingClassDTO updateTrainingCLassDTO)
        {
            try
            {
                if (await _trainingClassService.UpdateTrainingClassAsync(trainingClassId, updateTrainingCLassDTO))
                {
                    //return Ok("Update class successfully");
                    return NoContent();
                }
                else
                {
                    throw new Exception("Saving fail");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Update class fail: " + ex.Message);
            }
        }

        /// <summary>
        /// This method create training class
        /// </summary>
        /// <param name="classInput">Create training class api</param>
        /// <returns>Ok when new class saved successfully; BadRequest when new class saved fail or when there is error</returns>
        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> CreateTrainingClassAsync(CreateTrainingClassDTO2 classInput)
        {
            var classDTO = _mapper.Map<CreateTrainingClassDTO>(classInput);
            return await CreateTrainingClassAsync(classDTO);
        }
        [NonAction]
        public async Task<IActionResult> CreateTrainingClassAsync(CreateTrainingClassDTO classDTO)
        {
            try
            {
                var result = await _trainingClassService.CreateTrainingClassAsync(classDTO);
                if (result is null)
                {
                    throw new Exception("Saving fail");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Create training class fail: " + ex.Message);
            }

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllTraningClass()
        {
            var listResult = await _trainingClassService.GetAllTrainingClassesAsync();
            if (listResult != null)
            {
                return Ok(listResult);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> FilterResult(TrainingClassFilterModel trainingClassFilterModel)
        {
            var fiterResult = await _trainingClassService.FilterLocation(trainingClassFilterModel.locationName, trainingClassFilterModel.branchName, trainingClassFilterModel.date1, trainingClassFilterModel.date2, trainingClassFilterModel.classStatus, trainingClassFilterModel.attendInClass, trainingClassFilterModel.trainer);
            if (fiterResult.IsNullOrEmpty())
            {
                return NoContent();
            }
            return Ok(fiterResult);
        }
        [HttpGet]
        public async Task<IActionResult> GetTrainingClassDetail(Guid id)
        {
            var detailList = await _trainingClassService.GetFinalTrainingClassesAsync(id);
            if (detailList!=null)
            {
                return Ok(detailList);
            }
            return NoContent();
        }

        /// <summary>
        /// This method softly remove training class without delete it completely
        /// </summary>
        /// <param name="trainingClassId">Training class id</param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> SoftRemoveTrainingClass(string trainingClassId)
        {
            try
            {
                if (await _trainingClassService.SoftRemoveTrainingClassAsync(trainingClassId))
                {
                    //return Ok("SoftRemove class successfully");
                    return NoContent();
                }
                else
                {
                    throw new Exception("Saving fail");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("SoftRemove class fail: " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> ImportTrainingClass(IFormFile file)
        {
            var addSuccess = await _trainingClassService.ImportExcel(file);
            if (addSuccess != null)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
