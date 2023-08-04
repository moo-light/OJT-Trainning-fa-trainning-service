using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class UnitController : BaseController
    {

        private readonly IUnitService _unitService;
        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitBSyllabusID(Guid id)
        {

            var comsuon = await _unitService.GetSyllabusDetail(id);
            if (comsuon is not null)
            {
                return Ok(comsuon);

            }
            return NotFound("This syllabus doesn't have untis");


        }
    }
}
