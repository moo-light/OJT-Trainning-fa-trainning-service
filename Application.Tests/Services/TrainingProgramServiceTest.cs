using System.Linq.Expressions;
using Application.Services;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Tests.Services
{
    public class TrainingProgramServiceTest : SetupTest
    {
        private readonly TrainingProgramService _trainingProgramService;
        public TrainingProgramServiceTest()
        {
            _trainingProgramService = new TrainingProgramService(_unitOfWorkMock.Object, _mapperConfig);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository).Returns(_trainingProgramRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.SyllabusRepository).Returns(_syllabusRepositoryMock.Object);
        }

        [Fact]
        public async Task GetTrainingProgramDetail_ShoudlReturnCorrectData()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create<TrainingProgram>();
            trainingProgram.IsDeleted = false;
            var syllabuses = _fixture.Build<Syllabus>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.Units).Without(x => x.User).CreateMany<Syllabus>(2).ToList();
            var trainingProgramView = _mapperConfig.Map<TrainingProgramViewModel>(trainingProgram);
            trainingProgramView.Contents = _mapperConfig.Map<ICollection<SyllabusTrainingProgramViewModel>>(syllabuses);


            _unitOfWorkMock.Setup(um => um.SyllabusRepository.GetSyllabusByTrainingProgramId(trainingProgram.Id)).ReturnsAsync(syllabuses);
            _unitOfWorkMock.Setup(um => um.TrainingProgramRepository.GetByIdAsync(trainingProgram.Id)).ReturnsAsync(trainingProgram);
            var result = await _trainingProgramService.GetTrainingProgramDetail(trainingProgram.Id);
            result.Should().BeEquivalentTo(trainingProgramView);

        }

        [Fact]
        public async Task GetTrainingProgramDetail_ShouldReturnNull()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create<TrainingProgram>();
            var trainingProgramId = trainingProgram.Id;
            _unitOfWorkMock.Setup(um => um.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(trainingProgram = null);
            var result = await _trainingProgramService.GetTrainingProgramDetail(trainingProgramId);
            result.Should().BeNull();
        }
        [Fact]
        public async void DuplicateTrainingProgram_ShouldReturnCorrectData()
        {

            //Setup
            var duplicateItem = _fixture.Build<TrainingProgram>()
                                        .Without(x => x.TrainingClasses)
                                        .Without(x => x.DetailTrainingProgramSyllabus)
                                        .Create();
            var duplicateDetailItem = _fixture.Build<DetailTrainingProgramSyllabus>()
                                              .Without(x => x.Syllabus)
                                              .Without(x => x.TrainingProgram)
                                              .With(x => x.SyllabusId, Guid.NewGuid())
                                              .CreateMany(2).ToList();
            duplicateItem.DetailTrainingProgramSyllabus = (ICollection<DetailTrainingProgramSyllabus>)duplicateDetailItem;
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<TrainingProgram, object>>[]>())).ReturnsAsync(duplicateItem);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.AddAsync(It.IsAny<TrainingProgram>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.DetailTrainingProgramSyllabusRepository.AddRangeAsync(It.IsAny<List<DetailTrainingProgramSyllabus>>())).Verifiable();
            var result = await _trainingProgramService.DuplicateTrainingProgram(duplicateItem.Id);
            result.Should().BeOfType<TrainingProgram>();
            result.ProgramName.Should().BeEquivalentTo(duplicateItem.ProgramName);

        }
        [Fact]
        public async Task DuplicateTrainingProgram_NotFound_ShouldThrowException()
        {
            var duplicateItem = _fixture.Build<TrainingProgram>()
                                        .Without(x => x.TrainingClasses)
                                        .Without(x => x.DetailTrainingProgramSyllabus)
                                        .Create();
            Guid id = duplicateItem.Id;
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(duplicateItem = null);
            Func<Task> act = async () => await _trainingProgramService.DuplicateTrainingProgram(id);
            await act.Should().ThrowAsync<Exception>().WithMessage("Not found or TrainingProgram has been deleted");

        }
        [Fact]
        public async Task CreateTrainingProgram_ShouldReturnCorrectDate()
        {
            var createTrainingProgramDTO = _fixture.Build<UpdateTrainingProgramDTO>().Without(ct => ct.SyllabusesId).Create();
            createTrainingProgramDTO.SyllabusesId = new List<Guid>();
            var syllabuses = _fixture.Build<Syllabus>().Without(x => x.Units).Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.User).CreateMany<Syllabus>(2);
            foreach (var item in syllabuses) createTrainingProgramDTO.SyllabusesId.Add(item.Id);
            foreach (var item in syllabuses) _unitOfWorkMock.Setup(um => um.SyllabusRepository.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _unitOfWorkMock.Setup(um => um.DetailTrainingProgramSyllabusRepository.AddAsync(It.IsAny<DetailTrainingProgramSyllabus>())).Verifiable();
            var trainingProgram = _mapperConfig.Map<TrainingProgram>(createTrainingProgramDTO);
            _unitOfWorkMock.Setup(um => um.TrainingProgramRepository.AddAsync(trainingProgram)).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _trainingProgramService.CreateTrainingProgram(createTrainingProgramDTO);
            actualResult.Should().NotBeNull();
        }
        [Fact]
        public async Task DuplicateTrainingProgram_SaveChangeFailed_ShouldThrowException()
        {
            var duplicateItem = _fixture.Build<TrainingProgram>()
                                        .Without(x => x.TrainingClasses)
                                        .Without(x => x.DetailTrainingProgramSyllabus)
                                        .Create();
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<TrainingProgram, object>>[]>())).ReturnsAsync(duplicateItem);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.AddAsync(It.IsAny<TrainingProgram>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(-1);

            Func<Task> result = async () => await _trainingProgramService.DuplicateTrainingProgram(duplicateItem.Id);
            await result.Should().ThrowAsync<Exception>().WithMessage("Add Training Program Failed _ Save Change Failed!");

        }
        [Fact]
        public async Task CreateTrainingProgram_ShouldReturnNull()
        {
            var createTrainingProgramDTO = _fixture.Build<UpdateTrainingProgramDTO>().Without(x => x.SyllabusesId).Create();
            var result = await _trainingProgramService.CreateTrainingProgram(createTrainingProgramDTO);
            result.Should().BeNull();
        }


        [Fact]
        public async Task UpdateTrainingProgram_ShouldReturnTrue()
        {
            var updateDTO = _fixture.Build<UpdateTrainingProgramDTO>().Without(x => x.SyllabusesId).Create();
            updateDTO.SyllabusesId = new List<Guid>();
            var detailProgramSyllabuses = _fixture.Build<DetailTrainingProgramSyllabus>().Without(x => x.Syllabus).Without(x => x.TrainingProgram).CreateMany<DetailTrainingProgramSyllabus>(1);
            var updateProgram = _mapperConfig.Map<TrainingProgram>(updateDTO);
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.Update(updateProgram)).Verifiable();
            var syllabuses = _fixture.Build<Syllabus>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.Units).Without(x => x.User).CreateMany<Syllabus>(2);
            foreach (var item in syllabuses) updateDTO.SyllabusesId.Add(item.Id);
            foreach (var item in syllabuses) _unitOfWorkMock.Setup(um => um.SyllabusRepository.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(updateProgram);
            _unitOfWorkMock.Setup(m => m.DetailTrainingProgramSyllabusRepository.FindAsync(s => s.TrainingProgramId == updateProgram.Id)).ReturnsAsync(detailProgramSyllabuses.ToList());
            _unitOfWorkMock.Setup(m => m.DetailTrainingProgramSyllabusRepository.SoftRemoveRange(It.IsAny<List<DetailTrainingProgramSyllabus>>())).Verifiable();
            _unitOfWorkMock.Setup(m => m.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _trainingProgramService.UpdateTrainingProgram(updateDTO);
            actualResult.Should().BeTrue();

        }

        [Fact]
        public async Task UpdateTrainingProgram_ProgramNull_ShouldReturnFalse()
        {
            var updateDTO = _fixture.Build<UpdateTrainingProgramDTO>().Without(x => x.SyllabusesId).Create();
            var trainingProg = _mapperConfig.Map<TrainingProgram>(updateDTO);
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.GetByIdAsync(updateDTO.Id!.Value)).ReturnsAsync(trainingProg = null);

            var actualResult = await _trainingProgramService.UpdateTrainingProgram(updateDTO);
            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteTrainingProgram_ShouldReturnTrue()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.TrainingClasses).Create();
            var detailTrainingSyllabus = _fixture.Build<DetailTrainingProgramSyllabus>().Without(x => x.TrainingProgram).Without(x => x.Syllabus).CreateMany(3);
            foreach (var detail in detailTrainingSyllabus) detail.TrainingProgramId = trainingProgram.Id;
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.GetByIdAsync(trainingProgram.Id)).ReturnsAsync(trainingProgram);
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.SoftRemove(It.IsAny<TrainingProgram>())).Verifiable();
            _unitOfWorkMock.Setup(m => m.DetailTrainingProgramSyllabusRepository.FindAsync(x => x.TrainingProgramId == trainingProgram.Id)).ReturnsAsync(detailTrainingSyllabus.ToList());
            _unitOfWorkMock.Setup(m => m.DetailTrainingProgramSyllabusRepository.SoftRemoveRange(It.IsAny<List<DetailTrainingProgramSyllabus>>())).Verifiable();
            _unitOfWorkMock.Setup(m => m.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _trainingProgramService.DeleteTrainingProgram(trainingProgram.Id);
            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTrainingProgram_ShouldReturnFalse()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var id = trainingProgram.Id;
            _unitOfWorkMock.Setup(m => m.TrainingProgramRepository.GetByIdAsync(trainingProgram.Id)).ReturnsAsync(trainingProgram = null);
            var actualResult = await _trainingProgramService.DeleteTrainingProgram(id);
            actualResult.Should().BeFalse();
        }
        [Fact]
        public async Task ViewAllTrainingProgramDTOs_ShouldReturnNull()
        {
            var result_empty = await _trainingProgramService.ViewAllTrainingProgramDTOs();
            result_empty.Should().BeNull();
        }
        [Fact]
        public async Task ViewAllTrainingProgramDTOs_ShouldReturnCorrectValue()
        {
            var mockData = _fixture.Build<TrainingProgram>()
                                   .With(x => x.IsDeleted, false)
                                   .Without(x => x.DetailTrainingProgramSyllabus)
                                   .Without(x => x.TrainingClasses)
                                   .CreateMany(3).ToList();

            foreach (var item in mockData)
            {
                item.DetailTrainingProgramSyllabus = _fixture.Build<DetailTrainingProgramSyllabus>()
                    .With(x => x.TrainingProgram, item)
                    .With(x => x.IsDeleted, false)
                    .Without(x => x.Syllabus)
                    .CreateMany(3).ToList();
                foreach (var innerItem in item.DetailTrainingProgramSyllabus.ToList())
                {
                    innerItem.Syllabus = _fixture.Build<Syllabus>()
                        .With(x => x.IsDeleted, false)
                        .Without(x => x.User)
                        .Without(x => x.Units)
                        .Without(x => x.DetailTrainingProgramSyllabus)
                        .Create();
                    innerItem.TrainingProgramId = item.Id;
                }
            }

            _trainingProgramRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(mockData);
            _trainingProgramRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                                    .ReturnsAsync((Guid id) => mockData.FirstOrDefault(tp => tp.Id == id));
            _syllabusRepositoryMock.Setup(x => x.GetSyllabusByTrainingProgramId(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) =>
                {
                    List<Syllabus> list = new();
                    foreach (var item in mockData)
                    {
                        var syllabuses = item.DetailTrainingProgramSyllabus.Where(x => x.TrainingProgramId == id).Select(x => x.Syllabus);
                        list.AddRange(syllabuses);
                    }
                    return list;
                }
                );

            var result = await _trainingProgramService.ViewAllTrainingProgramDTOs();
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.ForEach(x => x.Content.Should().HaveCount(3));
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                  .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter(null, null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithCreateByFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                  .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter(null, null, "ten");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithStatusFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter(null, "Active", null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithStatusFilterAndCreateByFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter(null, "Active", "ten");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithValidString()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter("C", null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithValidStringAndCreateByFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter("C", null, "ten");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithValidStringAndStatusFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter("C", "Active", null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchTrainingProgramWithFilter_ShouldReturnAllTrainingProgram_WithValidStringAndStatusFilterAndCreateByFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().With(u => u.Id, Guid.NewGuid()).Without(u => u.CreationDate)
                                                              .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                              .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                              .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                              .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                              .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                              .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                              .Without(u => u.Role).Without(u => u.Applications)
                                                              .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                              .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                              .With(u => u.IsDeleted, false).Without(u => u.SubmitQuizzes)
                                                              .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1).With(u => u.Gender, "Female")
                                                              .CreateMany(3).ToList();
            var mockTrainingPrograms = _fixture.Build<TrainingProgram>().Without(tp => tp.Id).Without(tp => tp.CreationDate)
                                                                        .Without(tp => tp.ModificationDate).Without(tp => tp.ModificationBy)
                                                                        .Without(tp => tp.DeletionDate).Without(tp => tp.DeleteBy)
                                                                        .Without(tp => tp.Duration).Without(tp => tp.DetailTrainingProgramSyllabus)
                                                                        .Without(tp => tp.TrainingClasses).With(tp => tp.IsDeleted, false)
                                                                        .With(tp => tp.ProgramName, "CSS").With(tp => tp.Status, "Active")
                                                                        .With(tp => tp.CreatedBy, mockUsers.First().Id)
                                                                        .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterTrainingProgramViewModel>>(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(mockTrainingPrograms);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _trainingProgramService.SearchTrainingProgramWithFilter("C", "Active", "ten");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async Task ViewAllTrainingProgramDTOs_ShouldReturnView()
        {
            var allTrainingProgramMock = _fixture.Build<TrainingProgram>()
                                                    .Without(x=>x.DetailTrainingProgramSyllabus)
                                                    .Without(x=>x.TrainingClasses)
                                                    .CreateMany(1).ToList();
            var allTrainingProgram = _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetAllAsync()).ReturnsAsync(allTrainingProgramMock);
            var mapViewTrainingProgram = _mapperConfig.Map<List<ViewAllTrainingProgramDTO>>(allTrainingProgramMock);
            
            IList<ViewAllTrainingProgramDTO> viewAllTraining= new List<ViewAllTrainingProgramDTO>();
            foreach(var a in mapViewTrainingProgram)
            {
                var resultMock= _fixture.Build<TrainingProgram>()
                                        .Without(x=>x.DetailTrainingProgramSyllabus)
                                        .Without(x=>x.TrainingClasses)
                                        .Create();
                _unitOfWorkMock.Setup(x=>x.TrainingProgramRepository.GetByIdAsync(a.Id)).ReturnsAsync(resultMock);

                var trainingProgramView=_mapperConfig.Map<ViewAllTrainingProgramDTO>(resultMock);
                var syllabusMock = _fixture.Build<Syllabus>()
                                            .Without(x=>x.Units)
                                            .Without(x=>x.DetailTrainingProgramSyllabus)
                                            .Without(x=>x.User)
                                            .CreateMany(1).ToList();
                _unitOfWorkMock.Setup(x => x.SyllabusRepository.GetSyllabusByTrainingProgramId(trainingProgramView.Id)).ReturnsAsync(syllabusMock);  
                trainingProgramView.Content = (ICollection<Syllabus>?)syllabusMock;
                viewAllTraining.Add(trainingProgramView);

                var actualResult = await _trainingProgramService.ViewAllTrainingProgramDTOs();

                actualResult.Should().BeOfType<List<ViewAllTrainingProgramDTO>>();
            }
        }
    }
}