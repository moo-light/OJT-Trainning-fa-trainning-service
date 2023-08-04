using Application.ViewModels.GradingModels;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers;

public class GradingsControllerTest : SetupTest
{
    private readonly GradingsController _gradingController;
    public GradingsControllerTest()
    {
        _gradingController = new GradingsController(_gradingServiceMock.Object,
                                                    _claimsServiceMock.Object,
                                                    _mapperMock.Object);
    }

    [Fact]
    public async Task ExportMarkReportForClass_ShouldReturnNoContentResult()
    {
        //Arrange
        var classID = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        //var dataList = _fixture.Build<MarkReportDto>().CreateMany(5);
        var dataList = new List<MarkReportDto>();
        _gradingServiceMock.Setup(x => x.GetMarkReportOfClass(classID)).Returns(dataList);
        //Act

        var result = await _gradingController.ExportMarkReportForClass(classID, cancellationToken);
        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ExportMarkReportForClass_ShouldReturnFileStreamResult()
    {
        //Arrange
        var classID = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var dataList = _fixture.Build<MarkReportDto>().CreateMany(5).ToList();
        _gradingServiceMock.Setup(x => x.GetMarkReportOfClass(classID)).Returns(dataList);
        //Act

        var result = await _gradingController.ExportMarkReportForClass(classID, cancellationToken);
        //Assert
        result.Should().BeOfType<FileStreamResult>();
    }

    [Fact]
    public async Task ExportMarkReportForTrainee_ShouldReturnNoContentResult()
    {
        //Arrange
        var traineeID = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        //var dataList = _fixture.Build<MarkReportDto>().CreateMany(5);
        var dataList = new List<MarkReportDto>();
        _gradingServiceMock.Setup(x => x.GetMarkReportOfTrainee(traineeID)).Returns(dataList);
        //Act

        var result = await _gradingController.ExportMarkReportForTrainee(traineeID, cancellationToken);
        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ExportMarkReportForTrainee_ShouldReturnFileStreamResult()
    {
        //Arrange
        var traineeId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var dataList = _fixture.Build<MarkReportDto>().CreateMany(5).ToList();
        _gradingServiceMock.Setup(x => x.GetMarkReportOfTrainee(traineeId)).Returns(dataList);
        //Act

        var result = await _gradingController.ExportMarkReportForTrainee(traineeId, cancellationToken);
        //Assert
        result.Should().BeOfType<FileStreamResult>();
    }
}
