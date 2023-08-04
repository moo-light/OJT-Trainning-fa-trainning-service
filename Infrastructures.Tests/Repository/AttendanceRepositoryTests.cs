using AutoFixture;
using Domain.Entities;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.AttendanceStatusEnums;

namespace Infrastructures.Tests.Repository
{
    public class AttendanceRepositoryTests : SetupTest
    {
        private readonly AttendanceRepository _attendanceRepository;

        public AttendanceRepositoryTests()
        {
            _attendanceRepository = new AttendanceRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object, _mapperConfig);


        }
        [Fact]
        public async void GetAllAttendanceWithFilter_ShouldReturnPagination()
        {
            // Build data

            #region Building Data
            var trainingProgram = _fixture.Build<TrainingProgram>()
                .OmitAutoProperties()
                .With(x => x.ProgramName)
                .With(x => x.Status)
                .With(x => x.Duration)
                .With(x => x.IsDeleted, false)
                .Create();
            await _dbContext.AddRangeAsync(trainingProgram);
            _dbContext.SaveChanges();
            var trainingClasses = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .With(x => x.Name)
                .With(x => x.StartTime)
                .With(x => x.EndTime)
                .With(x => x.TrainingProgramId, trainingProgram.Id)
                .With(x => x.IsDeleted, false)
                .With(x => x.StatusClassDetail)
                .With(x => x.Branch)
                .With(x => x.Attendee)
                .With(x => x.Code)
                .CreateMany(2).ToArray();
            await _dbContext.AddRangeAsync(trainingClasses);
            _dbContext.SaveChanges();
            var users = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.UserName)
                .With(x => x.PasswordHash)
                .With(x => x.FullName)
                .With(x => x.Email)
                .With(x => x.DateOfBirth)
                .With(x => x.Gender)
                .With(x => x.AvatarUrl)
                .With(x => x.RoleId, 1)
                .With(x => x.IsDeleted, false)
                .CreateMany(5);
            users.ElementAt(2).FullName = "Đạo";
            _dbContext.AddRange(users);
            _dbContext.SaveChanges();
            var applications = _fixture.Build<Applications>()
                .OmitAutoProperties()
                .With(x => x.Reason)
                .With(x => x.Approved, true)
                .With(x => x.TrainingClassId, trainingClasses[0].Id)
                .CreateMany(10).ToArray();
            foreach (var app in applications)
            {
                app.Id = Guid.NewGuid();
            }
            _dbContext.AddRange(applications);

            var attendances = _fixture.Build<Attendance>()
                .OmitAutoProperties()
                .With(x => x.Date)
                .With(x => x.Status, nameof(Absent))
                .With(x => x.UserId, users.ElementAt(0).Id)
                .With(x => x.TrainingClassId, trainingClasses[0].Id)
                .CreateMany(5).ToList();
            var attendances_withApplication = _fixture.Build<Attendance>()
                .OmitAutoProperties()
                .With(x => x.Date)
                .With(x => x.Status, nameof(AbsentPermit))
                .With(x => x.UserId, users.ElementAt(1).Id)
                .With(x => x.TrainingClassId, trainingClasses[1].Id)
                .CreateMany(5).ToList();
            var attendances_withPresentStatus = _fixture.Build<Attendance>()
                .OmitAutoProperties()
                .With(x => x.Date)
                .With(x => x.Status, nameof(Present))
                .With(x => x.UserId, users.ElementAt(2).Id)
                .With(x => x.TrainingClassId, trainingClasses[0].Id)
                .CreateMany(5).ToList();
            attendances_withPresentStatus[0].Date = DateTime.MinValue;
            attendances.AddRange(attendances_withApplication);
            attendances.AddRange(attendances_withPresentStatus);
            for (int i = 5; i < attendances.Count; i++)
            {
                attendances[i].ApplicationId = applications[i - 5].Id;
            }
            _dbContext.AddRange(attendances);
            _dbContext.SaveChanges();
            #endregion

            // Setup
            Guid classId = Guid.Empty;
            bool? containApplication = null;
            string search = "";
            string by = nameof(None);
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            Expression<Func<Attendance, bool>> expression = x =>
                (classId == Guid.Empty || classId == x.TrainingClassId)    // by classId
                    && (x.User != null && x.User.FullName.Contains(search))                                    // by Username
                    && (containApplication == null || (x.Application != null) == containApplication) // by application                                     // by Username
                    && (fromDate <= x.Date && x.Date <= toDate)  // by datetime 
                    && (by == nameof(None) || by != nameof(None) && by == x.Status);       // by Status
                                                                                           // by Status
            int pageIndex = 0;
            int pageSize = 10;

            // Setup & Act
            var result = await _attendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);

            containApplication = false;
            var result_application_false = await _attendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);
            containApplication = true;
            var result_application_true = await _attendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);
            search = "Đạo";
            var result_search = await _attendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);
            fromDate = DateTime.UtcNow.AddMonths(-5);
            toDate = DateTime.UtcNow.AddYears(5);
            var result_dateTime = await _attendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);
            // Assert
            result.Should().NotBeNull();
            result.TotalItemsCount.Should().Be(15);
            result.TotalPagesCount.Should().Be(2);
            result_application_false.TotalItemsCount.Should().Be(5);
            result_application_true.TotalItemsCount.Should().Be(10);
            result_search.TotalItemsCount.Should().Be(5);
            if (result_dateTime != null)
                result_dateTime.TotalItemsCount.Should().NotBe(5);
        }

        [Fact]
        public async Task GetAbsentAttendanceOfDay_ShouldReturnCorrectData()
        {
            // Arrange
            var date = DateTime.Now;
            var userMock = new User
            {
                Id = Guid.NewGuid(),
                UserName = "Test1",
                FullName = "ABC",
                Email = "johndoe@example.com"
            };
            var classMock = new TrainingClass
            {
                Id = Guid.NewGuid(),
                Name = "Test Class",
                Attendee = "Intern",
                Branch = "FSU",
                Code = "Senier",
                StatusClassDetail = nameof(StatusClassDetail.Active)
            };
            var attendanceMocks = new List<Attendance>
            {
                new Attendance
                {
                    Id = Guid.NewGuid(),
                    UserId = userMock.Id,
                    User = userMock,
                    TrainingClassId = classMock.Id,
                    TrainingClass = classMock,
                    Status = AttendanceStatusEnums.Absent.ToString(),
                    Date = date.AddDays(-1),
                    IsDeleted = false
                },
                new Attendance
                {
                    Id = Guid.NewGuid(),
                    UserId = userMock.Id,
                    User = userMock,
                    TrainingClassId = classMock.Id,
                    TrainingClass = classMock,
                    Status = AttendanceStatusEnums.Absent.ToString(),
                    Date = date,
                    IsDeleted = false
                },
                new Attendance
                {
                    Id = Guid.NewGuid(),
                    UserId = userMock.Id,
                    User = userMock,
                    TrainingClassId = classMock.Id,
                    TrainingClass = classMock,
                    Status = AttendanceStatusEnums.Present.ToString(),
                    Date = date,
                    IsDeleted = false
                },
                new Attendance
                {
                    Id = Guid.NewGuid(),
                    UserId = userMock.Id,
                    User = userMock,
                    TrainingClassId = classMock.Id,
                    TrainingClass = classMock,
                    Status = AttendanceStatusEnums.Absent.ToString(),
                    Date = date.AddDays(1),
                    IsDeleted = false
                }
            };
            await _dbContext.Attendances.AddRangeAsync(attendanceMocks);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _attendanceRepository.GetAbsentAttendanceOfDay(date);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(userMock.Id);
            result.First().TrainingClassId.Should().Be(classMock.Id);
            result.First().Status.Should().Be(AttendanceStatusEnums.Absent.ToString());
            result.First().Date.Should().Be(date);
            result.First().NumOfAbsented.Should().Be(3);
        }



    }
}
