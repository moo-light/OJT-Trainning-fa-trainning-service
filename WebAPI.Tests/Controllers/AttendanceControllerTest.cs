using Application.Commons;
using Application.ViewModels.AtttendanceModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class AttendanceControllerTest : SetupTest
    {
        private readonly AttendanceController _attendanceController;

        public AttendanceControllerTest()
        {
            _attendanceController = new AttendanceController(_attendanceServiceMock.Object);
        }
        [Fact]
        public async Task GetAttendanceByClassId_Should_ReturnData()
        {
            // Arange

            var mockclass = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.Location)
                .Without(x => x.Applications).Create();
            var mockAttendence = _fixture.Build<Attendance>().Without(x => x.Application).Without(x => x.TrainingClass).Without(x => x.User).With(x => x.TrainingClass, mockclass).CreateMany(1).ToList();
            _attendanceServiceMock.Setup(x => x.GetAttendancesByTraineeClassID(It.IsAny<Guid>())).ReturnsAsync(mockAttendence);
            // Act
            var result = await _attendanceController.GetAttendanceByClassId(mockclass.Id);
            // Assert
            Assert.NotNull(result);
            _attendanceServiceMock.Verify(x => x.GetAttendancesByTraineeClassID(It.Is<Guid>(x => x.Equals(mockclass.Id))), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            ((OkObjectResult)result)!.Value.Should().Be(mockAttendence);
        }

        [Fact]
        public async Task GetAttendanceByTraineeId_Should_ReturnData()
        {
            var mockclass = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.Location)
                .Without(x => x.Applications).Create();
            var mockAttendance = _fixture.Build<Attendance>().With(x => x.TrainingClass, mockclass).Without(x => x.Application).Without(x => x.TrainingClass).Without(x => x.User).CreateMany(2).ToList();
            _attendanceServiceMock.Setup(x => x.GetAttendanceByTraineeID(It.IsAny<Guid>())).ReturnsAsync(mockAttendance);

            //act
            var result = await _attendanceController.GetAttendanceByTraineeId(mockclass.Id);

            //assert
            Assert.NotNull(result);
            _attendanceServiceMock.Verify(x => x.GetAttendanceByTraineeID(It.Is<Guid>(x => x.Equals(mockclass.Id))), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            ((OkObjectResult)result)!.Value.Should().Be(mockAttendance);
        }
        [Fact]
        public async Task GetAllAttendanceReports_ShouldReturnCorrectValue()
        {
            var mockData_1 = new Pagination<AttendanceViewDTO>();
            var mockData_2 = null as Pagination<AttendanceViewDTO>;
            Guid classId_1 = Guid.NewGuid();
            Guid classId_2 = Guid.Empty;
            _attendanceServiceMock.Setup(x => x.GetAllAttendanceWithFilter(classId_1, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockData_1);
            _attendanceServiceMock.Setup(x => x.GetAllAttendanceWithFilter(classId_2, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockData_2);
            // Act
            var result_1 = await _attendanceController.GetAllAttendanceReports(classId_1);
            var result_2 = await _attendanceController.GetAllAttendanceReports(classId_2);
            // Assert
            result_1.Should().BeOfType<OkObjectResult>();
            OkObjectResult okObjectResult = result_1 as OkObjectResult;
            okObjectResult.Value.Should().Be(mockData_1);
            result_2.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}