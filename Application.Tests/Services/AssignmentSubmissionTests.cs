using Application.Interfaces;
using Application.Services;
using Application.Utils;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Azure;
using Moq;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Services;

namespace Application.Tests.Services
{
    public class AssignmentSubmissionTests : SetupTest
    {
        private readonly IAssignmentSubmisstionService _assignmentSubmissionService;
        private readonly Mock<IFormFile> file;
        private readonly Guid id = Guid.NewGuid();
        public AssignmentSubmissionTests()
        {
            _assignmentSubmissionService = new AssignmentSubmissionService(_unitOfWorkMock.Object, _claimsServiceMock.Object);
            //Setup mock file using a memory stream
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var filePath = dirName + "\\Resources\\AssignmentSubmissions\\ctgbate.jpg";
            file = new Mock<IFormFile>();
            var sourceImg = File.OpenRead(filePath);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sourceImg);
            writer.Flush();
            stream.Position = 0;
            var fileName = "ctgbate.jpg";
            file.Setup(f => f.OpenReadStream()).Returns(stream);
            file.Setup(f => f.FileName).Returns(fileName);
            file.Setup(f => f.Length).Returns(stream.Length);
            var contentType = "image/jpeg";
            string val = "form-data; name=";

            val += "\"";
            val += "files";
            val += "\"";
            val += "; filename=";
            val += "\"";
            val += "ctgbate.jpg";
            val += "\"";
            file.Setup(f => f.ContentType).Returns(contentType);
            file.Setup(f => f.ContentDisposition).Returns(val);
        }

        /*[Fact]
        public async Task AddSubmission_ThorwExcpetion_WhenNullAssign()
        {
            var assignment = new List<Assignment>();//null assignment return false
            _unitOfWorkMock.Setup(x => x.AssignmentRepository
            .FindAsync(a =>
                a.Id == id &&
                a.IsOverDue == false &&
                a.IsDeleted == false
                )).ReturnsAsync(assignment);
            Func<Task> act = async () => await _assignmentSubmissionService.AddSubmisstion(id, file.Object);
            await act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task AddSubmission_ThrowException_WhenNullSubmission()
        {
            var fakeID = Guid.Empty;
            var assignment = _fixture.Build<Assignment>().Without(x => x.AssignmentSubmissions).Without(a => a.Lecture).CreateMany(2).ToList();
            var submissions = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).CreateMany(1).ToList();

            _unitOfWorkMock.Setup(x => x.AssignmentRepository
            .FindAsync(a =>
                a.Id == fakeID &&
                a.IsOverDue == false &&
                a.IsDeleted == false
                )).ReturnsAsync(assignment);
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeID);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository
            .FindAsync(a =>
                 a.AssignmentId == fakeID &&
                 a.IsDeleted == false &&
                 a.CreatedBy == fakeID
                 )).ReturnsAsync(submissions);

            Func<Task> act = async () => await _assignmentSubmissionService.AddSubmisstion(fakeID, file.Object);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AddSubmission_ShouldTrue()
        {
            var fakeID = Guid.Empty;
            var assignmentList = _fixture.Build<Assignment>().Without(x => x.AssignmentSubmissions).Without(a => a.Lecture).CreateMany(2).ToList();
            var submissions = new List<AssignmentSubmission>();
            var assignmenent = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            _unitOfWorkMock.Setup(x => x.AssignmentRepository
            .FindAsync(a =>
                a.Id == fakeID &&
                a.IsOverDue == false &&
                a.IsDeleted == false
                )).ReturnsAsync(assignmentList);
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeID);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository
            .FindAsync(a =>
                 a.AssignmentId == fakeID &&
                 a.IsDeleted == false &&
                 a.CreatedBy == fakeID
                 )).ReturnsAsync(submissions);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.AddAsync(assignmenent)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            var result = await _assignmentSubmissionService.AddSubmisstion(fakeID, file.Object);
            result.Should().BeTrue();
        }
        */
/*        [Fact]
        public async Task AddSubmission_ShouldFasle_WhenSaveChangesFalse()
        {
            var fakeID = Guid.Empty;
            var assignmentList = _fixture.Build<Assignment>().Without(x => x.AssignmentSubmissions).Without(a => a.Lecture).CreateMany(2).ToList();
            var submissions = new List<AssignmentSubmission>();
            var assignmenent = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            _unitOfWorkMock.Setup(x => x.AssignmentRepository
            .FindAsync(a =>
                a.Id == fakeID &&
                a.IsOverDue == false &&
                a.IsDeleted == false
                )).ReturnsAsync(assignmentList);
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeID);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository
            .FindAsync(a =>
                 a.AssignmentId == fakeID &&
                 a.IsDeleted == false &&
                 a.CreatedBy == fakeID
                 )).ReturnsAsync(submissions);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.AddAsync(assignmenent)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);
            var result = await _assignmentSubmissionService.AddSubmisstion(fakeID, file.Object);
            result.Should().BeFalse();
        }*/
        
        [Fact]
        public async Task AddSubmission_ThrowException_WhenCheckJoinClassisZero()
        {
            Guid fakeUserId = Guid.NewGuid();
            Guid fakeClassId = Guid.NewGuid();
            _claimsServiceMock.Setup(x=>x.GetCurrentUserId).Returns(fakeUserId);
            //list user join class
            List<DetailTrainingClassParticipate> detailTrainingClassParticipates = new List<DetailTrainingClassParticipate>();
            _unitOfWorkMock.Setup(x=>x.DetailTrainingClassParticipate.FindAsync(p=>p.TrainingClassID==fakeClassId && p.UserId==fakeUserId)).ReturnsAsync(detailTrainingClassParticipates);

            Func<Task> act = async () => await _assignmentSubmissionService.AddSubmisstion(id, fakeClassId, file.Object);
            await act.Should().ThrowAsync<Exception>().WithMessage("User not join in class!");
        }
        [Fact]
        public async Task AddSubmission_ThrowException_WhenIsOverDueisTrue()
        {
            Guid fakeUserId = Guid.NewGuid();
            Guid fakeClassId = Guid.NewGuid();
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeUserId);
            //list user join class
            DetailTrainingClassParticipate detailTrainingClassParticipate = new DetailTrainingClassParticipate
            {
                UserId= fakeUserId,
                TrainingClassID=fakeClassId,
            };
            List<DetailTrainingClassParticipate> detailTrainingClassParticipates = new List<DetailTrainingClassParticipate> { detailTrainingClassParticipate };
           
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.FindAsync(p => p.TrainingClassID == fakeClassId && p.UserId == fakeUserId)).ReturnsAsync(detailTrainingClassParticipates);

            var assignmentList = _fixture.Build<Assignment>()
                .Without(x=>x.AssignmentSubmissions)
                .Without(x => x.Lecture)
                .CreateMany(1)
                .ToList();
            assignmentList.First().IsOverDue= true;
            _unitOfWorkMock.Setup(x => x.AssignmentRepository.FindAsync(It.IsAny<Expression<Func<Assignment, bool>>>())).ReturnsAsync(assignmentList);


            Func<Task> act = async () => await _assignmentSubmissionService.AddSubmisstion(id, fakeClassId, file.Object);
            await act.Should().ThrowAsync<Exception>().WithMessage("Assignment is overdue!");
        }
        [Fact]
        public async Task AddSubmission_ThrowException_WhenExistedSubmission()
        {
            Guid fakeUserId = Guid.NewGuid();
            Guid fakeClassId = Guid.NewGuid();
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeUserId);
            //list user join class
            DetailTrainingClassParticipate detailTrainingClassParticipate = new DetailTrainingClassParticipate
            {
                UserId = fakeUserId,
                TrainingClassID = fakeClassId,
            };
            List<DetailTrainingClassParticipate> detailTrainingClassParticipates = new List<DetailTrainingClassParticipate> { detailTrainingClassParticipate };

            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.FindAsync(p => p.TrainingClassID == fakeClassId && p.UserId == fakeUserId)).ReturnsAsync(detailTrainingClassParticipates);

            List<Assignment> assignmentList = new List<Assignment>();
            _unitOfWorkMock.Setup(x => x.AssignmentRepository.FindAsync(It.IsAny<Expression<Func<Assignment, bool>>>())).ReturnsAsync(assignmentList);

            var submissionExisted = _fixture.Build<AssignmentSubmission>()
                                            .Without(x=>x.Assignment)
                                            .CreateMany(1).ToList();
            submissionExisted.First().AssignmentId = id;
            submissionExisted.First().IsDeleted = false;
            submissionExisted.First().CreatedBy = fakeUserId;

            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.FindAsync(It.IsAny<Expression<Func<AssignmentSubmission, bool>>>())).ReturnsAsync(submissionExisted);
            Func<Task> act = async () => await _assignmentSubmissionService.AddSubmisstion(id, fakeClassId, file.Object);
            await act.Should().ThrowAsync<Exception>().WithMessage("Submission has already existed!");
        }
        [Fact]
        public async Task AddSubmission_ShouldReturnSubmissionId()
        {
            Guid fakeUserId = Guid.NewGuid();
            Guid fakeClassId = Guid.NewGuid();
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(fakeUserId);
            //list user join class
            DetailTrainingClassParticipate detailTrainingClassParticipate = new DetailTrainingClassParticipate
            {
                UserId = fakeUserId,
                TrainingClassID = fakeClassId,
            };
            List<DetailTrainingClassParticipate> detailTrainingClassParticipates = new List<DetailTrainingClassParticipate> { detailTrainingClassParticipate };

            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.FindAsync(p => p.TrainingClassID == fakeClassId && p.UserId == fakeUserId)).ReturnsAsync(detailTrainingClassParticipates);

            List<Assignment> assignmentList = new List<Assignment>();
            _unitOfWorkMock.Setup(x => x.AssignmentRepository.FindAsync(It.IsAny<Expression<Func<Assignment, bool>>>())).ReturnsAsync(assignmentList);

            //var submissionExisted = _fixture.Build<AssignmentSubmission>()
            //                                .Without(x => x.Assignment)
            //                                .CreateMany(1).ToList();
            List<AssignmentSubmission> submissionExisted = new List<AssignmentSubmission>();

            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.FindAsync(It.IsAny<Expression<Func<AssignmentSubmission, bool>>>())).ReturnsAsync(submissionExisted);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.AddAsync(new AssignmentSubmission())).Verifiable();
            var actualResult= await _assignmentSubmissionService.AddSubmisstion(id, fakeClassId, file.Object);
            actualResult.Should().NotBe(Guid.Empty);
        }
        [Fact]
        public async Task DownloadSubmission_ReturnFile()
        {
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var fileName = dirName + "\\Resources\\AssignmentSubmissions\\1_00000000-0000-0000-0000-000000000000_ctgbate.jpg";
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            assignmentSubmis.IsDeleted = false;
            assignmentSubmis.FileName = fileName;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(assignmentSubmis.Id)).ReturnsAsync(assignmentSubmis);
            var actualResult = await _assignmentSubmissionService.DownloadSubmiss(assignmentSubmis.Id);
            actualResult.Should().BeOfType<FileEntity>();
        }


        [Fact]
        public async Task DownloadSubmission_ThrowException_WhenSubmissionNull()
        {
            AssignmentSubmission assignmentSubmis = null;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);
            //_dbContext.Add(assignment);
            Func<Task> act = async () => await _assignmentSubmissionService.DownloadSubmiss(id);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task DownloadSubmission_ThrowException_WhenSubmissionIsDeleted()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            assignmentSubmis.IsDeleted = true;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);

            Func<Task> act = async () => await _assignmentSubmissionService.DownloadSubmiss(id);
            await act.Should().ThrowAsync<Exception>();
        }


        [Fact]
        public async Task RemoveSubmission_ShouldRetunTrue()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.IsDeleted).Without(x => x.Assignment).Create();
            assignmentSubmis.IsDeleted = false;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);

            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.Update(assignmentSubmis)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _assignmentSubmissionService.RemoveSubmisstion(id);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task RemoveSubmission_ThrowException_WhenSubmissionIsDeleted()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.IsDeleted).Without(x => x.Assignment).Create();
            assignmentSubmis.IsDeleted = true;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);

            Func<Task> act = async () => await _assignmentSubmissionService.RemoveSubmisstion(id);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task RemoveSubmission_ThrowException_WhenSubmissNull()
        {
            AssignmentSubmission assignmentSubmis = null;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);

            Func<Task> act = async () => await _assignmentSubmissionService.RemoveSubmisstion(id);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task RemoveSubmission_ShouldRetunFalse_WhenSaveChangeFalse()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.IsDeleted).Without(x => x.Assignment).Create();
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id)).ReturnsAsync(assignmentSubmis);
            assignmentSubmis.IsDeleted = false;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.Update(assignmentSubmis)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);
            var actualResult = await _assignmentSubmissionService.RemoveSubmisstion(id);

            actualResult.Should().BeFalse();
        }
        [Fact]
        public async Task EditSubmission_ShouldReturnTrue()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            var assignment = new Assignment { IsOverDue = false };
            assignmentSubmis.Assignment = assignment;
            assignmentSubmis.Version = 0;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id, s => s.Assignment)).ReturnsAsync(assignmentSubmis);
            //assignmentSubmis.Assignment.SetFile(formFile);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.Update(assignmentSubmis)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _assignmentSubmissionService.EditSubmisstion(id, file.Object);

            actualResult.Should().BeTrue();

        }

        [Fact]
        public async Task EditSubmission_ShouldReturnFalse()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            var assignment = new Assignment { IsOverDue = false };
            assignmentSubmis.Assignment = assignment;
            assignmentSubmis.Version = 0;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id, s => s.Assignment)).ReturnsAsync(assignmentSubmis);
            //assignmentSubmis.Assignment.SetFile(formFile);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.Update(assignmentSubmis)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            var actualResult = await _assignmentSubmissionService.EditSubmisstion(id, file.Object);

            actualResult.Should().BeFalse();

        }

        [Fact]
        public async Task GradingandReviewSubmission_ShouldReturnLectureId()
        {
            var assignment = new Assignment();
            var assignmentSubmission = _fixture.Build<AssignmentSubmission>()
                .Without(x => x.Assignment)
                .Create();
            assignmentSubmission.Assignment = assignment;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(It.IsAny<Guid>(), s => s.Assignment)).ReturnsAsync(assignmentSubmission);
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.Update(assignmentSubmission)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();
            var actualResult = await _assignmentSubmissionService.GradingandReviewSubmission(id, 8, "Good");
            _unitOfWorkMock.Verify(x => x.AssignmentSubmissionRepository.Update(assignmentSubmission), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
            actualResult.Should().Be(assignmentSubmission.Assignment.LectureID);
        }

        [Fact]
        public async Task GradingandReviewSubmission_ShouldThrowException_WhenSubmissionNull()
        {
            AssignmentSubmission assignmentSubnmission = null;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(assignmentSubnmission);
            Func<Task> actualResult = async () => await _assignmentSubmissionService.GradingandReviewSubmission(Guid.Empty, 1, "Bad");
            await actualResult.Should().ThrowAsync<Exception>().WithMessage("Submission Assignment is not exist!");
        }

        [Fact]
        public async Task EditSubmission_ThrowException_WhenAssignmentIsOverdue()
        {
            var assignmentSubmis = _fixture.Build<AssignmentSubmission>().Without(x => x.Assignment).Create();
            var assignment = new Assignment { IsOverDue = true };
            assignmentSubmis.Assignment = assignment;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id, s => s.Assignment)).ReturnsAsync(assignmentSubmis);

            Func<Task> act = async () => await _assignmentSubmissionService.EditSubmisstion(id, file.Object);

            await act.Should().ThrowAsync<Exception>();

        }

        [Fact]
        public async Task EditSubmission_ShouldReturnFalse_WhenSubmissionNotExist()
        {
            AssignmentSubmission assignmentSubmis = null;
            _unitOfWorkMock.Setup(x => x.AssignmentSubmissionRepository.GetByIdAsync(id, s => s.Assignment)).ReturnsAsync(assignmentSubmis);

            Func<Task> act = async () => await _assignmentSubmissionService.EditSubmisstion(id, file.Object);

            await act.Should().ThrowAsync<Exception>();

        }
    }
}
