using Application.Commons;
using Application.Interfaces;
using Application.Models.ApplicationModels;
using Application.Repositories;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;
namespace Application.Tests.Services
{
    public class ApplicationServiceTests : SetupTest
    {
        private readonly IApplicationService _applicationService;
        public ApplicationServiceTests()
        {
            _applicationService = new ApplicationService(_unitOfWorkMock.Object, _mapperConfig, _appConfiguration, _currentTimeMock.Object, _claimsServiceMock.Object);
            _unitOfWorkMock.Setup(x => x.ApplicationRepository).Returns(_applicationRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository).Returns(_trainingClassRepositoryMock.Object);

            _applicationRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Applications>>())).Verifiable();
            _applicationRepositoryMock.Setup(x => x.UpdateRange(It.IsAny<List<Applications>>())).Verifiable();
            _applicationRepositoryMock.Setup(x => x.Update(It.IsAny<Applications>())).Verifiable();

            _unitOfWorkMock.Setup(s => s.SaveChangeAsync()).ReturnsAsync(1);
        }
        [Fact]
        public async Task GetAllApplication_ShouldReturnCorrectValue()
        {
            ApplicationFilterDTO filter = new();
            Guid classId = default;
            var mockData_ListOf10 = _fixture.Build<Applications>().OmitAutoProperties().CreateMany(10).ToList();
            int pageIndex = 0;
            int pageSize = 10;
            Pagination<Applications> mockData = new()
            {
                Items = mockData_ListOf10,
                TotalItemsCount = 100,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            _applicationRepositoryMock.Setup(x => x.ToPagination(It.IsAny<Expression<Func<Applications, bool>>>(), pageIndex, pageSize)).ReturnsAsync(mockData);
            // Act
            Pagination<Applications> result = await _applicationService.GetAllApplication(classId, filter,pageIndex,pageSize);
            // Assert
            result.Should().Be(mockData);
            result.PageIndex.Should().Be(pageIndex);
            result.PageSize.Should().Be(pageSize);
        }
    }
}
