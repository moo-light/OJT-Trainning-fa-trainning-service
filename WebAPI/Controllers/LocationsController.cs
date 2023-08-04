using Application.Interfaces;
using Application.ViewModels.Location;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers
{
    [Authorize]
    public class LocationsController : BaseController
    {
        private readonly ILocationService _locationService;
        private readonly IClaimsService _claimsService;
        private readonly IMapper _mapper;

        public LocationsController(ILocationService locationService, IClaimsService claimsService, IMapper mapper)
        {
            _locationService = locationService;
            _claimsService = claimsService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateLocation(CreateLocationDTO createLocationDTO)
        {
            var result = await _locationService.AddNewLocation(createLocationDTO);
            return (result != null) ? Ok(result) : BadRequest("Create location fail");
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllLocation()
        {
            var result = await _locationService.GetAllLocation();
            return (result.IsNullOrEmpty()) ? BadRequest("List empty") : Ok(result);
        }
    }
}
