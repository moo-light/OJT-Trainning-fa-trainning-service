using Application.ViewModels.FeedbackModels;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers;

public class FeedbackControllerTest : SetupTest
{
    private readonly FeedbacksController _feedbackController;

    public FeedbackControllerTest()
    {
        _feedbackController = new FeedbacksController(_feedbackServiceMock.Object,
                                                        _claimsServiceMock.Object,
                                                        _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        // Arange
        // Act
        var result = await _feedbackController.GetAll();
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnOkResult()
    {
        // Arange
        // Act
        var result = await _feedbackController.GetById(new Guid());
        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnNoContentResult()
    {
        // Arange
        var model = _fixture.Build<FeedbackModel>().Create();
        // Act
        var result = await _feedbackController.Create(model);
        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Edit_ShouldReturnNoContentResult()
    {
        // Arange
        var model = _fixture.Build<FeedbackModel>().Create();
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.UpdateFeedbackAsync(feedbackId, model)).ReturnsAsync(true);
        // Act
        var result = await _feedbackController.Edit(feedbackId, model);
        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Edit_ShouldReturnBadRequestResult()
    {
        // Arange
        var model = _fixture.Build<FeedbackModel>().Create();
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.UpdateFeedbackAsync(feedbackId, model)).ReturnsAsync(false);
        // Act
        var result = await _feedbackController.Edit(feedbackId, model);
        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContentResult()
    {
        // Arange
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.DeleteFeedbackAsync(feedbackId)).ReturnsAsync(true);
        // Act
        var result = await _feedbackController.Delete(feedbackId);
        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequestResult()
    {
        // Arange
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.DeleteFeedbackAsync(feedbackId)).ReturnsAsync(false);
        // Act
        var result = await _feedbackController.Delete(feedbackId);
        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task SendFeedbackLink_ShouldReturnOkResult()
    {
        // Arange
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.SendFeedbacklink(feedbackId)).ReturnsAsync(true);
        // Act
        var result = await _feedbackController.SendFeedbackLink(feedbackId);
        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SendFeedbackLink_ShouldReturnBadRequestResult()
    {
        // Arange
        var feedbackId = new Guid();
        _feedbackServiceMock.Setup(x => x.SendFeedbacklink(feedbackId)).ThrowsAsync(new Exception());
        // Act
        var result = await _feedbackController.SendFeedbackLink(feedbackId);
        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

}
