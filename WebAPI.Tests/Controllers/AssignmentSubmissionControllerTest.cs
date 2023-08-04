using Application.Utils;
using Application.ViewModels.GradingModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class AssignmentSubmissionControllerTest : SetupTest
    {
        private readonly AssignmentSubmissionController _controller;
        private readonly Guid idMock = Guid.NewGuid();
        private readonly IFormFile formFile;
        public AssignmentSubmissionControllerTest()
        {
            _controller = new AssignmentSubmissionController(_claimsServiceMock.Object,_assignmentSubmissionServiceMock.Object, _gradingServiceMock.Object);
            //Setup mock file using a memory stream
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
        public async Task SubmissAssignment_ShouldReturn201()
        {
            var classId=Guid.NewGuid();
            _assignmentSubmissionServiceMock.Setup(a => a.AddSubmisstion(idMock,classId, formFile)).ReturnsAsync(Guid.NewGuid());

            var actualResult = await _controller.SubmissAssignment(idMock,classId, formFile);
            actualResult.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task SubmissAssignment_ShouldReturnNoContent()
        {
            var classId = Guid.NewGuid();
            _assignmentSubmissionServiceMock.Setup(a => a.AddSubmisstion(idMock, classId, formFile)).ReturnsAsync(Guid.Empty);

            var actualResult = await _controller.SubmissAssignment(idMock,classId, formFile);

            actualResult.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteSubmission_ShouldRturnOk()
        {
            _assignmentSubmissionServiceMock.Setup(x => x.RemoveSubmisstion(idMock)).ReturnsAsync(true);

            var actualResult = await _controller.DeleteSubmission(idMock);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteSubmission_ShouldRturnBadRequest()
        {
            _assignmentSubmissionServiceMock.Setup(x => x.RemoveSubmisstion(idMock)).ReturnsAsync(false);

            var actualResult = await _controller.DeleteSubmission(idMock);

            actualResult.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DownloadSubmission_ShouldReturnOk()
        {
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var fileName2 = dirName + "\\Resources\\AssignmentSubmissions\\ctgbate.jpg";
            var fileMock = fileName2.GetFileEntity();
            _assignmentSubmissionServiceMock.Setup(x => x.DownloadSubmiss(idMock)).ReturnsAsync(fileMock);

            var actualResult = await _controller.DownLoadSubmission(idMock);

            actualResult.Should().BeOfType<FileContentResult>();
        }
        [Fact]
        public async Task DownloadSubmission_ShouldReturnBadRequest_WhenFileEntityNull()
        {
            FileEntity fileMock = null;
            _assignmentSubmissionServiceMock.Setup(x => x.DownloadSubmiss(idMock)).ReturnsAsync(fileMock);

            var actualResult = await _controller.DownLoadSubmission(idMock);

            actualResult.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task EditSubmission_ShouldReturnOk()
        {

            _assignmentSubmissionServiceMock.Setup(x => x.EditSubmisstion(idMock, formFile)).ReturnsAsync(true);

            var actualResult = await _controller.EditSubmission(idMock, formFile);

            actualResult.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task EditSubmission_ShouldReturnBadRquest()
        {
            _assignmentSubmissionServiceMock.Setup(x => x.EditSubmisstion(idMock, formFile)).ReturnsAsync(false);

            var actualResult = await _controller.EditSubmission(idMock, formFile);

            actualResult.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task GradingReview_ShouldReturnOk()
        {
            Guid submisstionId=Guid.NewGuid();
            Guid lectureIdReturn = Guid.NewGuid();
            _assignmentSubmissionServiceMock.Setup(x => x.GradingandReviewSubmission(submisstionId, 3, "Bad")).ReturnsAsync(lectureIdReturn);
            _gradingServiceMock.Setup(x => x.AddToGrading(new GradingModel(lectureIdReturn, Guid.Empty, "A", 3))).Verifiable();


            var actualResult = await _controller.GradingReview(submisstionId, 3, "A", "Bad", Guid.Empty);
            actualResult.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task GradingReview_ShouldReturnBadRequest()
        {

            Guid lectureIdReturn = Guid.NewGuid();
            _assignmentSubmissionServiceMock.Setup(x => x.GradingandReviewSubmission(idMock, 3, "Bad")).ReturnsAsync(Guid.Empty);
            var actualResult = await _controller.GradingReview(idMock, 3, "Three", "Bad", Guid.Empty);
            actualResult.Should().BeOfType<BadRequestResult>();
        }
    }
}
