using Application.Commons;
using Application.Models.ApplicationModels;
using Application.ViewModels.ApplicationViewModels;
using AutoFixture;
using Domain.Entities;
using Domain.Enums.Application;
using Domains.Test;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class ApplicationControllerTest : SetupTest
    {
        private readonly ApplicationController _applicationController;

        public ApplicationControllerTest()
        {

            _applicationController = new ApplicationController(_applicationServiceMock.Object);
        }

        [Fact]
        public async Task CreateApplication_ShouldReturnNoContentResult()
        {
            var model = _fixture.Build<ApplicationDTO>().Create();
            // Act
            var result = await _applicationController.CreateApplication(model);
            _applicationServiceMock.Setup(x => x.CreateApplication(model)).ReturnsAsync(true);
            var result_success = await _applicationController.CreateApplication(model);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result_success.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task UpdateStatus_ShouldReturnNoContentResult()
        {
            var model = Guid.NewGuid();
            var status = _fixture.Create<bool>();
            // Act
            var result = await _applicationController.UpdateStatus(model,status);
            _applicationServiceMock.Setup(x => x.UpdateStatus(model, status)).ReturnsAsync(true);
            var result_success = await _applicationController.UpdateStatus(model, status);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result_success.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task ViewAllApplication_ShouldReturnCorrectValue()
        {
            Guid classId = _fixture.Create<Guid>();
            var mockData = _fixture.Build<Pagination<Applications>>().Without(x => x.Items).Create();
            int pageIndex = _fixture.Create<int>();
            int pageSize = _fixture.Create<int>();
            var result_badrequest = await _applicationController.ViewAllApplication(classId, pageIndex, pageSize);

            _applicationServiceMock.Setup(x => x.GetAllApplication(classId, It.IsAny<ApplicationFilterDTO>(), pageIndex, pageSize)).ReturnsAsync(mockData);

            var result = await _applicationController.ViewAllApplication(classId, pageIndex, pageSize);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(mockData);
            result_badrequest.Should().BeOfType<BadRequestObjectResult>();

        }
        [Fact]
        public async Task ViewAllApplicationFilter_ShouldReturnCorrectValue()
        {
            //Setup
            ApplicationFilterDTO filter = _fixture.Build<ApplicationFilterDTO>().Create();

            Guid classId = _fixture.Create<Guid>();

            var mockData = _fixture.Build<Pagination<Applications>>().Without(x => x.Items).Create();
            int pageIndex = _fixture.Create<int>();
            int pageSize = _fixture.Create<int>();

            _applicationServiceMock.Setup(x => x.GetAllApplication(classId, filter, pageIndex, pageSize)).ReturnsAsync(mockData);
            // Act
            var result = await _applicationController.ViewAllApplicationFilter(classId, filter, pageIndex, pageSize);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(mockData);
        }
        [Fact]
        public async Task ViewAllApplicationFilter_ShouldReturnBadRequest()
        {
            //Setup
            ApplicationFilterDTO filter = _fixture.Build<ApplicationFilterDTO>().Create();

            Guid classId = _fixture.Create<Guid>();

            var mockData = null as Pagination<Applications>;
            int pageIndex = _fixture.Create<int>();
            int pageSize = _fixture.Create<int>();

            _applicationServiceMock.Setup(x => x.GetAllApplication(classId, filter, pageIndex, pageSize)).ReturnsAsync(mockData);
            // Act
            var result = await _applicationController.ViewAllApplicationFilter(classId, filter);
            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
        }
    }
}