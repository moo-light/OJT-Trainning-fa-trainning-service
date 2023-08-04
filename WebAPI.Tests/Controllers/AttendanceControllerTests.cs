using Application.ViewModels.AtttendanceModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class AttendanceControllerTests : SetupTest
    {
        private readonly AttendanceController _attendanceController;
        public AttendanceControllerTests()
        {
            _attendanceController = new AttendanceController(_attendanceServiceMock.Object);
        }

        [Fact]

        public async void CheckAttendance_ShouldReturnCorrectValue()
        {
            var attendanceMock = _fixture.Build<AttendanceDTO>().OmitAutoProperties()
                                         .CreateMany(10).ToList();
            var mockData = _fixture.Build<Attendance>().OmitAutoProperties()
                                         .CreateMany(10).ToList();
            var classIdMock = Guid.Empty;
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            _attendanceController.ControllerContext = new();
            _attendanceController.ControllerContext.HttpContext = context.Object;
            context.Setup(x => x.Request).Returns(request.Object);
            _attendanceController.Request.Method = "POST";

            var attendanceMock_empty = _fixture.Build<AttendanceDTO>().OmitAutoProperties().CreateMany(10).ToList();
            var mockData_empty = null as List<Attendance>;
            // Setup
            _attendanceServiceMock.Setup(x => x.UploadAttendanceFormAsync(attendanceMock, classIdMock, It.IsAny<string>())).ReturnsAsync(mockData);
            _attendanceServiceMock.Setup(x => x.UploadAttendanceFormAsync(attendanceMock_empty, classIdMock, It.IsAny<string>())).ReturnsAsync(mockData_empty);
            // Act
            var result = await _attendanceController.CheckAttendance(classIdMock, attendanceMock);
            var result_empty = await _attendanceController.CheckAttendance(classIdMock, attendanceMock_empty);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_empty.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async void EditAttendance_ShouldReturnCorrectValue()
        {
            var attendanceMock = _fixture.Build<Attendance>().OmitAutoProperties().Create();
            var attendanceDtoMock = _fixture.Build<AttendanceDTO>().OmitAutoProperties().Create();
            AttendanceDTO attendanceDtoMock_empty = null;
            var classIdMock = Guid.Empty;
            _attendanceServiceMock.Setup(x => x.UpdateAttendanceAsync(attendanceDtoMock, classIdMock)).ReturnsAsync(attendanceMock);
            _attendanceServiceMock.Setup(x => x.UpdateAttendanceAsync(attendanceDtoMock_empty, classIdMock)).ReturnsAsync(null as Attendance);
            // Act
            var result = await _attendanceController.EditAttendance(classIdMock, attendanceDtoMock);
            OkObjectResult okObjectResult = result as OkObjectResult;
            var result_empty = await _attendanceController.EditAttendance(classIdMock, attendanceDtoMock_empty);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            okObjectResult.Value.Should().Be(attendanceMock);
            result_empty.Should().BeOfType<BadRequestResult>();
        }
    }
}
