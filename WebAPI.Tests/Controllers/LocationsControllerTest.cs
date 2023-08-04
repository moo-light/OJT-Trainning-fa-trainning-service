using Application.ViewModels.Location;
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

namespace WebAPI.Tests.Controllers
{
    public class LocationsControllerTest : SetupTest
    {
        private readonly LocationsController locationsController;

        public LocationsControllerTest()
        {
            locationsController = new LocationsController(_locationServiceMock.Object, _claimsServiceMock.Object, _mapperConfig);
        }

        [Fact]
        public async Task CreateLocation_ReturnOK_WhenResultIsNotNull()
        {
            //arrange
            var mock = _fixture.Build<CreateLocationDTO>().Create();
            var mockLocationDTO = _fixture.Build<LocationDTO>().Create();
            _locationServiceMock.Setup(x => x.AddNewLocation(It.IsAny<CreateLocationDTO>())).ReturnsAsync(mockLocationDTO);

            //act
            var result = await locationsController.CreateLocation(mock);

            //assert
            _locationServiceMock.Verify(x => x.AddNewLocation(It.Is<CreateLocationDTO>(x => x.Equals(mock))), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            (result as OkObjectResult)!.Value.Should().Be(mockLocationDTO);
        }
        [Fact]
        public async Task CreateLocation_ReturnBadRequest_WhenResultIsNull()
        {
            //arrange
            var mock = _fixture.Build<CreateLocationDTO>().Create();
            LocationDTO mockLocationDTO = null!;
            _locationServiceMock.Setup(x => x.AddNewLocation(It.IsAny<CreateLocationDTO>())).ReturnsAsync(mockLocationDTO);

            //act
            var result = await locationsController.CreateLocation(mock);

            //assert
            _locationServiceMock.Verify(x => x.AddNewLocation(It.Is<CreateLocationDTO>(x => x.Equals(mock))), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("Create location fail");
        }
        [Fact]
        public async Task ViewAllLocation_ShouldReturnCorrectData()
        {
            //arrange
            var mock = _fixture.Build<LocationDTO>().CreateMany().ToList();
            _locationServiceMock.Setup(x => x.GetAllLocation()).ReturnsAsync(mock);

            //act
            var result = await locationsController.ViewAllLocation();

            //assert
            _locationServiceMock.Verify(x => x.GetAllLocation(), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            ((OkObjectResult)result).Value.Should().Be(mock);
        }
        [Fact]
        public async Task ViewAllLocation_ShouldReturnBadRequest_WhenDataIsNull()
        {
            //arrange

            //act
            var result = await locationsController.ViewAllLocation();

            //assert
            _locationServiceMock.Verify(x => x.GetAllLocation(), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            ((BadRequestObjectResult)result).Value.Should().Be("List empty");
        }
    }
}
