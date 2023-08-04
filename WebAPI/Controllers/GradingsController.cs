using Application.Interfaces;
using Application.Services;
using Application.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace WebAPI.Controllers;

public class GradingsController : BaseController
{
    private readonly IGradingService _gradingService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    public GradingsController(IGradingService gradingService, IClaimsService claimsService, IMapper mapper)
    {
        _gradingService = gradingService;
        _claimsService = claimsService;
        _mapper = mapper;
    }

    [HttpGet("classID")]
    public async Task<IActionResult> ExportMarkReportForClass(Guid classID, CancellationToken cancellationToken)
    {
        // query data from database  
        await Task.Yield();

        var dataList = _gradingService.GetMarkReportOfClass(classID);

        if (dataList.IsNullOrEmpty())
        {
            return NoContent();
        }

        var stream = FileUtils.ExportExcel(dataList);
        string excelName = $"MarkReportOfClass-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }

    [HttpGet("traineeId")]
    public async Task<IActionResult> ExportMarkReportForTrainee(Guid traineeId, CancellationToken cancellationToken)
    {
        // query data from database  
        await Task.Yield();
        var dataList = _gradingService.GetMarkReportOfTrainee(traineeId);

        if (dataList.IsNullOrEmpty())
        {
            return NoContent();
        }

        var stream = FileUtils.ExportExcel(dataList);
        string excelName = $"MarkReportOfClass-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }
}
