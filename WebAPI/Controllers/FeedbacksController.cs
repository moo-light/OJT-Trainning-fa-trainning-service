using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Application.ViewModels.FeedbackModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
public class FeedbacksController : BaseController
{
    private readonly IFeedbackService _feedbackService;
    private readonly IClaimsService _claimsService;
    private readonly IMapper _mapper;

    public FeedbacksController(IFeedbackService feedbackService, IClaimsService claimsService, IMapper mapper)
    {
        _feedbackService = feedbackService;
        _claimsService = claimsService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var feedbacks = await _feedbackService.GetFeedbacksAsync();
        return Ok(feedbacks);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
        return Ok(feedback);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FeedbackModel model)
    {
        await _feedbackService.CreateFeedbackAsync(model);
        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] FeedbackModel model)
    {
        var result = await _feedbackService.UpdateFeedbackAsync(id, model);
        if (result)
        {
            return NoContent();
        }
        return BadRequest();
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _feedbackService.DeleteFeedbackAsync(id);
        if (result)
        {
            return NoContent();
        }
        return BadRequest();
    }
    [HttpGet]
    public async Task<IActionResult> SendFeedbackLink(Guid feedbackId)
    {
        try
        {
            await _feedbackService.SendFeedbacklink(feedbackId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
