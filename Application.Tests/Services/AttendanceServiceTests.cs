using Application.Commons;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.ViewModels.AtttendanceModels;
using AutoFixture;
using Domain.Entities;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using Infrastructures;
using Infrastructures.Repositories;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace Application.Tests.Services
{
    public class AttendanceServiceTests : SetupTest
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceServiceTests()
        {
            _attendanceService = new AttendanceService(_unitOfWorkMock.Object, _mapperConfig, _appConfiguration, _currentTimeMock.Object);
            _unitOfWorkMock.Setup(x => x.AttendanceRepository).Returns(_attendanceRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.ApplicationRepository).Returns(_applicationRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository).Returns(_trainingClassRepositoryMock.Object);

            _attendanceRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Attendance>>())).Verifiable();
            _attendanceRepositoryMock.Setup(x => x.UpdateRange(It.IsAny<List<Attendance>>())).Verifiable();
            _attendanceRepositoryMock.Setup(x => x.Update(It.IsAny<Attendance>())).Verifiable();

            _unitOfWorkMock.Setup(s => s.SaveChangeAsync()).ReturnsAsync(1);
        }

        [Fact]
        public void UploadAttendanceFormAsync_ReturnsCorrectData()
        {
            // Setup
            var @class = new TrainingClass() { Id = Guid.NewGuid() };
            var applicationsMock = _fixture.Build<Applications>().Do(x => x.IsDeleted = false).Without(x => x.TrainingClass).Without(x => x.User)
                                           .CreateMany(5).ToList();
            var attendancesMock = _fixture.Build<Attendance>().Do(x => x.IsDeleted = false).Without(x => x.Application).Without(x => x.User).Without(x => x.TrainingClass)
                                          .CreateMany(10).ToList();
            //HttpPost
            var attendanceDtosMock_POST = _fixture.Build<AttendanceDTO>().Without(x => x.AttendanceId).CreateMany(10).ToList();
            _trainingClassRepositoryMock.Setup(x => x.GetByIdAsync(@class.Id)).ReturnsAsync(@class);
            foreach (var attendance in attendanceDtosMock_POST)
            {
                var index = attendanceDtosMock_POST.IndexOf(attendance);
                _applicationRepositoryMock.Setup(x => x.GetApplicationByUserAndClassId(attendance, @class.Id))
                               .ReturnsAsync(index < applicationsMock.Count ? applicationsMock[index] : null);
            }
            //HttpPatch
            var attendanceDtosMock_PATCH = _fixture.Build<AttendanceDTO>().CreateMany(10).ToList();
            foreach (var attendance in attendanceDtosMock_PATCH)
            {
                var index = attendanceDtosMock_PATCH.IndexOf(attendance);
                _attendanceRepositoryMock.Setup(x => x.GetByIdAsync(attendance.AttendanceId.Value)).ReturnsAsync(attendancesMock[index]);
                _applicationRepositoryMock.Setup(x => x.GetApplicationByUserAndClassId(attendance, @class.Id))
                               .ReturnsAsync(index < applicationsMock.Count ? applicationsMock[index] : null);
            }

            // Act
            var result_POST = _attendanceService.UploadAttendanceFormAsync(attendanceDtosMock_POST, @class.Id, "POST");
            var result_PATCH = _attendanceService.UploadAttendanceFormAsync(attendanceDtosMock_PATCH, @class.Id, "PATCH");
            // Assert
            result_POST.Result.Count.Should().Be(10);
            result_PATCH.Result.Count.Should().Be(10);
        }
        [Fact]
        public async void UpdateAttendanceAsync_ReturnCorrectValue_Present()
        {
            var @class = new TrainingClass() { Id = Guid.NewGuid() };
            //var attendanceMock = _fixture.Build<Attendance>().OmitAutoProperties().Create();
            var attendanceDtoMock = _fixture.Build<AttendanceDTO>().Create();
            attendanceDtoMock.Status = true;
            _trainingClassRepositoryMock.Setup(x => x.GetByIdAsync(@class.Id)).ReturnsAsync(@class);
            _applicationRepositoryMock.Setup(x => x.GetApplicationByUserAndClassId(attendanceDtoMock, @class.Id));
            // Act
            var result = await _attendanceService.UpdateAttendanceAsync(attendanceDtoMock, @class.Id);
            // Assert 
            result.Should().BeOfType<Attendance>();
            result.Status.Should().Be("Present");
        }
        [Fact]
        public async void UpdateAttendanceAsync_ReturnCorrectValue_Absent()
        {
            var @class = new TrainingClass() { Id = Guid.NewGuid() };
            //var attendanceMock = _fixture.Build<Attendance>().OmitAutoProperties().Create();
            var attendanceDtoMock = _fixture.Build<AttendanceDTO>().Create();
            var attendanceDtoMock_empty = _fixture.Build<AttendanceDTO>().Create();
            attendanceDtoMock.Status = false;
            Applications application = new();
            Applications application_empty = null;
            _trainingClassRepositoryMock.Setup(x => x.GetByIdAsync(@class.Id)).ReturnsAsync(@class);
            _applicationRepositoryMock.Setup(x => x.GetApplicationByUserAndClassId(attendanceDtoMock, @class.Id)).ReturnsAsync(application);
            _applicationRepositoryMock.Setup(x => x.GetApplicationByUserAndClassId(attendanceDtoMock_empty, @class.Id)).ReturnsAsync(application_empty);
            // Act
            var result = await _attendanceService.UpdateAttendanceAsync(attendanceDtoMock, @class.Id);
            var result_empty = await _attendanceService.UpdateAttendanceAsync(attendanceDtoMock_empty, @class.Id);
            // Assert 
            result.Should().BeOfType<Attendance>();
            result.Status.Should().Be(AttendanceStatusEnums.AbsentPermit.ToString());

            result_empty.Should().BeOfType<Attendance>();
            result_empty.Status.Should().Be(AttendanceStatusEnums.Absent.ToString());
        }
        [Fact]
        public async void GetAllAttendanceWithFilter_ShouldReturnCorrectValue()
        {
            // Setup
            Guid classId = Guid.Empty;
            Guid classId_empty = Guid.NewGuid();
            bool? containApplication = null;
            string search = null;
            string by = "Present";// prevent throwing exception
            int pageIndex = 0;
            int pageSize = 40;

            var mockData = _fixture.Build<Pagination<Attendance>>().Without(x => x.Items).Create();
            var mockData_Item = new List<Attendance>();
            mockData.Items = mockData_Item;
            Pagination<Attendance> mockData_empty = null;

            // Setup & Act
            DateTime dateTime = new DateTime();
            var result_empty = await _attendanceService.GetAllAttendanceWithFilter(classId, search, by, containApplication, dateTime, dateTime, pageIndex, pageSize);
            _attendanceRepositoryMock.Setup(x => x.GetAllAttendanceWithFilter(It.IsAny<Expression<Func<Attendance, bool>>>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mockData);
            var result_dto = await _attendanceService.GetAllAttendanceWithFilter(classId, search, by, containApplication, dateTime, dateTime, pageIndex, pageSize);
            // Assert
            var result = _mapperConfig.Map<Pagination<Attendance>>(result_dto);
            Assert.Equal(mockData_Item, result.Items);
            Assert.Null(result_empty);
        }

        [Fact]
        public async void GetAllAttendanceWithFilter_ShouldThrowEnumException()
        {
            // Act
            var result_throw = async () => await _attendanceService.GetAllAttendanceWithFilter(Guid.Empty, default, "this should throw", null, null, null, default);
            // Assert
            await result_throw.Should().ThrowAsync<InvalidEnumArgumentException>();
        }
    }
}
