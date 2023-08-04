using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace Domains.Test.Controller
{
    public class SyllabusClassTest : SetupTest
    {
        private readonly SyllabusController _syllabusController;

        public SyllabusClassTest()
        {
            _syllabusController = new SyllabusController(_syllabusServiceMock.Object, _unitServiceMock.Object, _lectureServiceMock.Object);
        }

        [Fact]
        public async Task GetAllSyllabus_ShouldReturnContains()
        {
            var mockData = _fixture.Build<Syllabus>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.User).Without(x => x.Units).Create<Syllabus>();
            _syllabusServiceMock.Setup(u => u.DeleteSyllabussAsync(mockData.Id.ToString())).ReturnsAsync(true);
            var result = await _syllabusController.DeleteSyllabus(mockData.Id.ToString());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteWrongUser_ShouldReturnBadRquest()
        {
            var mockData = _fixture.Build<Syllabus>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.User).Without(x => x.Units).Create<Syllabus>();
            _syllabusServiceMock.Setup(u => u.DeleteSyllabussAsync("wrongdate")).ReturnsAsync(false);
            var result = await _syllabusController.DeleteSyllabus("wrongdate");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
