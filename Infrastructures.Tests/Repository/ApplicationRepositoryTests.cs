using Application;
using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Application.ApplicationFilterByEnum;
using static System.Net.Mime.MediaTypeNames;
using FluentAssertions;
using Application.Models.ApplicationModels;

namespace Infrastructures.Tests.Repository
{
    public class ApplicationRepositoryTests : SetupTest
    {
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationRepositoryTests()
        {
            _applicationRepository = new ApplicationRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object);
        }

        /// <summary>
        /// This Test is 2 second long 
        /// if you can improve it i will allow it
        /// </summary>
        /// <returns>nothing</returns>
        /// warning: dont test on date "01/01/2023"
        [Fact]
        public async Task ToPagination_ShouldReturnCorrectValue()
        {
            #region CreatingData
            var trainingClassMock = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .With(x => x.Name)
                .With(x => x.Attendee)
                .With(x => x.StatusClassDetail)
                .With(x => x.Branch)
                .With(x => x.Code)
                .Create();
            _dbContext.Add(trainingClassMock);
            _dbContext.SaveChanges();
            var user_1_Mock = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.UserName)
                .With(x => x.PasswordHash)
                .With(x => x.AvatarUrl)
                .With(x => x.FullName)
                .With(x => x.Email)
                .With(x => x.IsDeleted, false)
                .With(x => x.Gender)
                .With(x => x.RoleId, 1)
                .Create();
            var user_2_Mock = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.UserName)
                .With(x => x.PasswordHash)
                .With(x => x.AvatarUrl)
                .With(x => x.FullName)
                .With(x => x.Email)
                .With(x => x.IsDeleted, false)
                .With(x => x.Gender)
                .With(x => x.RoleId, 1)
                .Create();
            // add user for filter with userid
            _dbContext.Add(user_1_Mock);
            _dbContext.Add(user_2_Mock);
            _dbContext.SaveChanges();
            var applicationList = new List<Applications>();
            for (int i = 0; i < 10; i++)
            {
                var application = _fixture.Build<Applications>().OmitAutoProperties().Create();
                application.UserId = user_1_Mock.Id;//add application to user 1
                application.TrainingClassId = trainingClassMock.Id;
                application.Reason = "a-bzy-c";//filter with search
                application.CreationDate = DateTime.Parse("01/01/2023").AddDays(1);// filter with createdDate
                applicationList.Add(application);

            }
            for (int i = 0; i < 10; i++)
            {
                var application = _fixture.Build<Applications>().OmitAutoProperties().Create();
                application.UserId = user_2_Mock.Id;//add application to user 2
                application.TrainingClassId = trainingClassMock.Id;
                application.Reason = "a-bzy";//filter with search
                if (i >= 5)
                    application.AbsentDateRequested = DateTime.Parse("01/01/2023").AddDays(1);//filter with absent date
                applicationList.Add(application);
            }
            _dbContext.AddRange(applicationList);
            _dbContext.SaveChanges();
            #endregion
            // Setup
            #region conditionSettings
            var condition_empty = new ApplicationFilterDTO();
            var condition_user_1 = new ApplicationFilterDTO()
            {
                UserID = user_1_Mock.Id,
            };
            var condition_search = new ApplicationFilterDTO()
            {
                Search = "a-b"
            };
            var condition_dateTime = new ApplicationFilterDTO()
            {
                FromDate = DateTime.Parse("01/01/2023").AddDays(1),
                ToDate = DateTime.Parse("01/01/2023").AddDays(2)
            };
            var condition_combine = new ApplicationFilterDTO()
            {
                ByDateType = nameof(RequestDate),
                UserID = user_2_Mock.Id,
                Search = "a-b",
                ToDate = DateTime.Parse("01/01/2023").AddDays(2),
                FromDate = DateTime.Parse("01/01/2023").AddDays(1),
                Approved = false
            };
            #endregion
            Guid classId = Guid.Empty;
            //Act & assert
            Expression<Func<Applications, bool>> expression = _applicationRepository.GetFilterExpression(classId, condition_empty);

            var applications = await _applicationRepository.ToPagination(expression: expression, 0, 10);

            expression = _applicationRepository.GetFilterExpression(classId, condition_user_1);

            var applicationsOfUser_1 = await _applicationRepository.ToPagination(expression: expression, 0, 10);

            expression = _applicationRepository.GetFilterExpression(classId, condition_search);

            var applicationsOfSearch = await _applicationRepository.ToPagination(expression: expression, 0, 10);

            expression = _applicationRepository.GetFilterExpression(classId, condition_dateTime);

            var applicationsOfDatetime = await _applicationRepository.ToPagination(expression: expression, 0, 10);

            expression = _applicationRepository.GetFilterExpression(classId, condition_combine);

            var applicationsCombine = await _applicationRepository.ToPagination(expression: expression, 0, 10);

            // Assert
            applications.Items.Should().NotBeEmpty();
            applications.TotalPagesCount.Should().Be(2);
            applications.TotalItemsCount.Should().Be(20);
            applications.TotalItemsCount.Should().Be(20);

            applicationsOfUser_1.TotalItemsCount.Should().Be(10);
            applicationsOfSearch.TotalItemsCount.Should().Be(20);
            applicationsOfDatetime.TotalItemsCount.Should().Be(10);
            applicationsCombine.Items.Should().NotBeEmpty();
            applicationsCombine.Items.Should().HaveCount(5);
            applicationsCombine.Items.AsQueryable()
                .Where(x => x.UserId == user_2_Mock.Id)
                .Where(x => x.Reason.Contains("a-b"))
                .Where(x => x.AbsentDateRequested >= condition_combine.FromDate)
                .Where(x => x.Approved == false)
                .Should().HaveCount(5);
        }
    }
}
