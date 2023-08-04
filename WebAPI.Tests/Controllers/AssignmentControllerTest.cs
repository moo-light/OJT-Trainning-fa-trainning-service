using Application.Utils;
using Application.ViewModels.AssignmentModel;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class AssignmentControllerTest : SetupTest
    {
        private readonly AssignmentController _controller;
        private readonly Guid idMock = Guid.NewGuid();
        private readonly IFormFile formFile;
        public AssignmentControllerTest()
        {
            _controller = new AssignmentController(_assigmentServiceMock.Object, _recurringJobManagerMock.Object);
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        }

        [Fact]
        public async Task CreateAssignment_ShouldReturn201()
        {
            var assigment = _fixture.Build<AssignmentViewModel>().Without(x => x.File).Create();
            assigment.File = formFile;
            _assigmentServiceMock.Setup(x => x.CreateAssignment(assigment)).ReturnsAsync(Guid.NewGuid());
            var actualResult = await _controller.CreateAssignment(assigment);
            actualResult.Should().BeOfType<CreatedAtActionResult>();
        }
        [Fact]
        public async Task CreateAssignment_ShouldReturnBadRequest()
        {
            var assigment = _fixture.Build<AssignmentViewModel>().Without(x => x.File).Create();
            assigment.File = formFile;
            _assigmentServiceMock.Setup(x => x.CreateAssignment(assigment)).ReturnsAsync(Guid.Empty);
            var actualResult = await _controller.CreateAssignment(assigment);
            actualResult.Should().BeOfType<NoContentResult>();
        }
        

        [Fact]
        public async Task UpdateAssignment_ShouldReturnOk()
        {
            var updateModel = _fixture.Build<AssignmentUpdateModel>().Without(x => x.File).Create();
            updateModel.File = formFile;
            _assigmentServiceMock.Setup(x => x.UpdateAssignment(updateModel)).ReturnsAsync(true);

            var actualResult = await _controller.UpdateAssignment(updateModel);

            actualResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task UpdateAssignment_ShouldReturnBadRequest_WhenUpdateReturnFalse()
        {
            var updateModel = _fixture.Build<AssignmentUpdateModel>().Without(x => x.File).Create();
            updateModel.File = formFile;
            _assigmentServiceMock.Setup(x => x.UpdateAssignment(updateModel)).ReturnsAsync(false);

            var actualResult = await _controller.UpdateAssignment(updateModel);

            actualResult.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task DeleteAssignment_ShouldReturnOk()
        {

            _assigmentServiceMock.Setup(x => x.DeleteAssignment(idMock)).ReturnsAsync(true);

            var actualResult = await _controller.DeleteAssignment(idMock);

            actualResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteAssignment_ShouldReturnBadRequest()
        {
            _assigmentServiceMock.Setup(x => x.DeleteAssignment(idMock)).ReturnsAsync(false);

            var actualResult = await _controller.DeleteAssignment(idMock);

            actualResult.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task DownloadAssignment_ShouldReturnFile()
        {
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var fileName2 = dirName + "\\Resources\\Assignments\\ctgbate.jpg";
            var fileMock = fileName2.GetFileEntity();

            _assigmentServiceMock.Setup(x => x.DownLoad(idMock)).ReturnsAsync(fileMock);

            var actualResult = await _controller.DownloadAssignment(idMock);
            actualResult.Should().BeOfType<FileContentResult>();
        }
        [Fact]
        public async Task DownloadAssignment_ShouldReturnBadRequest_WhenFileEntityNull()
        {
            FileEntity file = null;
            _assigmentServiceMock.Setup(x => x.DownLoad(idMock)).ReturnsAsync(file);

            var actualResult = await _controller.DownloadAssignment(idMock);
            actualResult.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task ViewAssignment_ShouldReturnOk()
        {
            var assignmentbyLecture = _fixture.Build<Assignment>().Without(u => u.AssignmentSubmissions).Without(a => a.Lecture).CreateMany(1).ToList();
            _assigmentServiceMock.Setup(x => x.GetAllAssignmentByLectureID(idMock)).ReturnsAsync(assignmentbyLecture);

            var actualResult = await _controller.ViewAssignmentByLectureID(idMock);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ViewAssignment_ShouldReturnBadRequest()
        {
            var assignmentbyLecture = _fixture.Build<Assignment>().Without(u => u.AssignmentSubmissions)
                                                                  .Without(u => u.Lecture)
                                                                  .CreateMany(10)
                                                                  .ToList();
            _assigmentServiceMock.Setup(x => x.GetAllAssignmentByLectureID(idMock)).ReturnsAsync(assignmentbyLecture = null);

            var actualResult = await _controller.ViewAssignmentByLectureID(idMock);

            actualResult.Should().BeOfType<BadRequestResult>();
        }
    }
}
