using Application.Commons;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Application.ViewModels.SyllabusModels;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Microsoft.AspNetCore.Authorization;
using Application.Utils;
using Domain.Enums;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _syllabusService;
        private readonly IUnitService _unitService;
        private readonly ILectureService _lectureService;
        public SyllabusController(ISyllabusService syllabusService, IUnitService unitService, ILectureService lectureService)
        {
            _syllabusService = syllabusService;
            _unitService = unitService;
            _lectureService = lectureService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSyllabus()
        {
            var getSyllabusList = await _syllabusService.GetAllSyllabus();
            if (getSyllabusList != null)
            {
                return Ok(getSyllabusList);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FilerSyllabus(double firstDuration, double secondDuration)
        {
            List<Syllabus> filterSyllabusList = await _syllabusService.FilterSyllabus(firstDuration, secondDuration);
            if (filterSyllabusList.Count() > 0)
            {
                return Ok(filterSyllabusList);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteSyllabus(string id)
        {
            var checkSyllabus = await _syllabusService.DeleteSyllabussAsync(id);


            if (checkSyllabus)
            {
                return NoContent();
            }


            return BadRequest("Delete Syllabus Not Successfully");
        }

        //[HttpPost]
        //[Authorize]
        //[ClaimRequirement(nameof(PermissionItem.SyllabusPermission), nameof(PermissionEnum.Create))]
        //public async Task<IActionResult> AddNewSyllabus(SyllabusViewDTO NewSyllasbus)
        //{
        //    Syllabus syllabusBase;
        //    Unit unitBase;
        //    Lecture lectureBase;
        //    syllabusBase = await _syllabusService.AddSyllabusAsync(NewSyllasbus.SyllabusBase);
        //    if (NewSyllasbus.Units.Count == 0)
        //    {
        //        return BadRequest("Unit is Empty");
        //    }
        //    if (NewSyllasbus.Units is not null)
        //        foreach (UnitDTO unitDTO in NewSyllasbus.Units)
        //        {
        //            unitBase = await _unitService.AddNewUnit(unitDTO, syllabusBase);
        //            if (unitDTO.Lectures.Count == 0)
        //            {
        //                return BadRequest("Lectures is Empty");
        //            }
        //            if (unitDTO.Lectures is not null)
        //                foreach (LectureDTO lecture in unitDTO.Lectures)
        //                {
        //                    lectureBase = await _lectureService.AddNewLecture(lecture);
        //                    await _lectureService.AddNewDetailLecture(lectureBase, unitBase);

        //                }
        //        }

        //    if (syllabusBase is not null && await _syllabusService.SaveChangesAsync() > 0)
        //    {

        //        return Ok("Successfully");

        //    }
        //    return BadRequest();

        //}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _syllabusService.GetByName(name);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Cannot find");
        }


        [HttpPatch]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.SyllabusPermission), nameof(PermissionEnum.FullAccess))]
        public async Task<IActionResult> UpdateSyllabus(Guid syllabusId, UpdateSyllabusModel updateObject)
        {
            var result = await _syllabusService.UpdateSyllabus(syllabusId, updateObject);
            if (result) return NoContent();
            else return BadRequest("Update Failed");
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> ViewDetail(Guid SyllabusId)
        //{
        //    var result = await _syllabusService.ViewDetailSyllabus(SyllabusId);
        //    if (result != null)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest();
        //}


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewDetail(Guid SyllabusId)
        {
            var result = await _syllabusService.FinalViewSyllabusDTO(SyllabusId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Not Found Or Have Been Deleted ");
        }


        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.SyllabusPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> AddNewSyllabus(UpdateSyllabusModel updateSyllabusModel)
        {

            var syllabus = await _syllabusService.AddNewSyllabusService(updateSyllabusModel);
            if (syllabus is null) return BadRequest("Add Syllabus Unsuccessfully");
            foreach (var item in updateSyllabusModel.Outline)
            {

                foreach (var item1 in item.Content)
                {

                    var unit = await _unitService.AddNewUnitHotFix(item1, item.Day, syllabus.Id);
                    if (unit is null) return BadRequest($"Add Unit {item.Content.IndexOf(item1)+1} Unsuccessfully");
                    foreach (var item2 in item1.Lessons)
                    {
                        var lecture = await _lectureService.AddNewLectureHotFix(item2);
                        if (lecture is null) return BadRequest($"Add Lecture {item1.Lessons.IndexOf(item2)+1} Unsuccessfully");
                        await _lectureService.AddNewDetailLecture(lecture, unit);
                    }
                }
            }
            await _syllabusService.SaveChangesAsync();
            return Ok("Added Successfully");

        }
    }
}
