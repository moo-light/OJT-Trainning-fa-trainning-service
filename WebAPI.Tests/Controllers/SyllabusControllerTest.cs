using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.FixViewSyllabus;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using Application.ViewModels.SyllabusModels.ViewDetail;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Azure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using Xunit.Sdk;

namespace WebAPI.Tests.Controllers
{

    public class SyllabusControllerTest : SetupTest
    {
        private readonly SyllabusController _syllabusController;
        public SyllabusControllerTest()
        {

            _syllabusController = new SyllabusController(_syllabusServiceMock.Object, _unitServiceMock.Object, _lectureServiceMock.Object);

        }
        //[Fact]
        //public async Task SearchNameSyllabus_Get_ShouldReturnCorrectValues()
        //{

        //    var mockData_1 = _fixture.Build<SyllabusViewAllDTO>().CreateMany(2).ToList();
        //    var mockData_2 = null as List<SyllabusViewAllDTO>;


        //    string name1 = "anything";
        //    string name2 = "this should return nothing";
        //    _syllabusServiceMock.Setup(s => s.GetByName(name1)).ReturnsAsync(mockData_1);
        //    _syllabusServiceMock.Setup(s => s.GetByName(name2)).ReturnsAsync(mockData_2);
        //    var result_ok = await _syllabusController.SearchByName(name1);
        //    var result_badRequest = await _syllabusController.SearchByName(name2);

        //    result_ok.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<List<SyllabusViewAllDTO>>();
        //    result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        //}
        //[Fact]
        //public async Task ViewDetail_Get_ShouldReturnCorrectValues()
        //{

        //    var mockData_1 = _fixture.Build<SyllabusShowDetailDTO>().OmitAutoProperties().Create();
        //    var mockData_2 = null as SyllabusShowDetailDTO;


        //    Guid syll_id1 = Guid.NewGuid();
        //    Guid syll_id2 = Guid.NewGuid();
        //    _syllabusServiceMock.Setup(s => s.ViewDetailSyllabus(syll_id1)).ReturnsAsync(mockData_1);
        //    _syllabusServiceMock.Setup(s => s.ViewDetailSyllabus(syll_id2)).ReturnsAsync(mockData_2);
        //    var result_ok = await _syllabusController.ViewDetail(syll_id1);
        //    var result_badRequest = await _syllabusController.ViewDetail(syll_id2);

        //    result_ok.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<SyllabusShowDetailDTO>();
        //    result_badRequest.Should().BeOfType<BadRequestResult>();
        //}
        [Fact]
        public async Task GetAllSyllabus_Should_ReturnData()
        {
            // Arrange 
            var mockData_ok = _fixture.Build<List<SyllabusViewAllDTO>>().OmitAutoProperties().Create();
            _syllabusServiceMock.Setup(x => x.GetAllSyllabus()).ReturnsAsync(mockData_ok);

            // Act
            var result_ok = await _syllabusController.GetAllSyllabus();

            // Arrange 
            List<SyllabusViewAllDTO> mockData_badRequest = null;
            _syllabusServiceMock.Setup(x => x.GetAllSyllabus()).ReturnsAsync(mockData_badRequest);

            // Act
            var result_badRequest = await _syllabusController.GetAllSyllabus();

            //Assert
            result_ok.Should().BeOfType<OkObjectResult>();
            result_ok.As<OkObjectResult>().Value.Should().NotBeNull();
            result_badRequest.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ViewDetailFormat_Should_ReturnData()
        {
            var mockData_1 = _fixture.Build<FinalViewSyllabusDTO>().OmitAutoProperties().Create();
            var mockData_2 = null as FinalViewSyllabusDTO;


            Guid syll_id1 = Guid.NewGuid();
            Guid syll_id2 = Guid.NewGuid();
            _syllabusServiceMock.Setup(s => s.FinalViewSyllabusDTO(syll_id1)).ReturnsAsync(mockData_1);
            _syllabusServiceMock.Setup(s => s.FinalViewSyllabusDTO(syll_id2)).ReturnsAsync(mockData_2);
            var result_ok = await _syllabusController.ViewDetail(syll_id1);
            var result_badRequest = await _syllabusController.ViewDetail(syll_id2);

            result_ok.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<FinalViewSyllabusDTO>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task FilterSyllabus_Should_ReturnData()
        {
            //Arrange
            double firstDuration = 9;
            double secondDuration = 12;

            var mockData = _fixture.Build<Syllabus>().OmitAutoProperties().CreateMany(0).ToList();

            _syllabusServiceMock.Setup(x => x.FilterSyllabus(firstDuration, secondDuration)).ReturnsAsync(mockData);
            //Act
            var result_notFound = await _syllabusController.FilerSyllabus(firstDuration, secondDuration);
            mockData.Add(_fixture.Build<Syllabus>().OmitAutoProperties().Create());
            var result_ok = await _syllabusController.FilerSyllabus(firstDuration, secondDuration);
            //Assert
            result_notFound.Should().BeOfType<NotFoundResult>();
            result_ok.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(mockData);
        }
        [Fact]
        public async Task UpdateSyllabus_ShouldReturnNoContent()
        {
            Guid id = Guid.NewGuid();
            var updateSyllabusDTO = _fixture.Create<UpdateSyllabusModel>();
            _syllabusServiceMock.Setup(sm => sm.UpdateSyllabus(id, updateSyllabusDTO)).ReturnsAsync(true);
            var result = await _syllabusController.UpdateSyllabus(id, updateSyllabusDTO);
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task UpdateSyllabus_ShouldReturnBadRequest()
        {
            Guid id = Guid.NewGuid();
            var updateSyllabusDTO = _fixture.Create<UpdateSyllabusModel>();
            _syllabusServiceMock.Setup(sm => sm.UpdateSyllabus(id, updateSyllabusDTO)).ReturnsAsync(false);
            var result = await _syllabusController.UpdateSyllabus(id, updateSyllabusDTO);
            result.Should().BeOfType<BadRequestObjectResult>();
            var objResult = result as BadRequestObjectResult;
            if (objResult is not null)
                objResult.Value.Should().BeSameAs("Update Failed");
        }
        [Fact]
        public async Task AddNewSyllabus_ShouldReturnCorrectData()
        {
            //Arrange
            var mockData = _fixture.Build<UpdateSyllabusModel>()
                                   .Without(x => x.Outline)
                                   .Without(x => x.Others)
                                   .Create();
            var outlines = mockData.Outline = _fixture.Build<UpdateOutlineModel>().Without(x => x.Content).CreateMany(1).ToList();
            var contents = outlines[0].Content = _fixture.Build<UpdateContentModel>().Without(x => x.Lessons).CreateMany(2).ToList();
            contents[0].Lessons = _fixture.Build<UpdateLessonModel>().CreateMany(2).ToList();
            contents[1].Lessons = _fixture.Build<UpdateLessonModel>().CreateMany(2).ToList();
            var others = mockData.Others = _fixture.Build<UpdateOtherModel>().CreateMany(2).ToList();

            var syllabus = _fixture.Build<Syllabus>().OmitAutoProperties().With(x => x.Id).Create();
            var unit = _fixture.Build<Unit>().OmitAutoProperties().With(x => x.Id).Create();
            var lecture = _fixture.Build<Lecture>().OmitAutoProperties().With(x => x.Id).Create();
            // Setups & Acts

            var result_syllabus_null = await _syllabusController.AddNewSyllabus(mockData);
            // Assert
            result_syllabus_null.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().NotBeNull();

            _syllabusServiceMock.Setup(s => s.AddNewSyllabusService(mockData))
                                .ReturnsAsync(syllabus);
            foreach (var item in mockData.Outline)
            {

                foreach (var item1 in item.Content)
                {
                    var result_unit_null = await _syllabusController.AddNewSyllabus(mockData);
                    // Assert
                    result_unit_null.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().NotBeNull();
                    _unitServiceMock.Setup(x => x.AddNewUnitHotFix(item1, item.Day, syllabus.Id)).ReturnsAsync(unit);
                    foreach (var item2 in item1.Lessons)
                    {

                        var result_lecture_null = await _syllabusController.AddNewSyllabus(mockData);
                        // Assert
                        result_lecture_null.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().NotBeNull();
                        
                        _lectureServiceMock.Setup(x => x.AddNewLectureHotFix(item2)).ReturnsAsync(lecture);
                        _lectureServiceMock.Setup(x => x.AddNewDetailLecture(lecture, unit)).Verifiable();
                    }
                }
            }
            //Act
            var result = await _syllabusController.AddNewSyllabus(mockData);

            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task AddNewSyllabus_ShouldReturnBadResult()
        {
            // Arrange
            var mockData_1 = _fixture.Build<UpdateSyllabusModel>().Create();
            var mockData_2 = _fixture.Build<UpdateSyllabusModel>().Create();
            // Act
            var result = await _syllabusController.AddNewSyllabus(mockData_1);
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}