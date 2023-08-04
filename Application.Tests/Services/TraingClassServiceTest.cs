using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using FluentAssertions.Common;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Moq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class TraingClassServiceTest : SetupTest
    {
        private readonly ITrainingClassService _trainingClassService;

        public TraingClassServiceTest()
        {
            _trainingClassService = new TrainingClassService(_unitOfWorkMock.Object, _mapperConfig);
        }
        [Fact]
        public async Task SoftRemoveTrainingClass_ShouldReturnTrue_WhenSaveSucceed()
        {
            //arrange
            var mockTrainingClass = _fixture.Build<TrainingClass>().Without(x => x.Applications).Without(x => x.Attendances).Without(x => x.Feedbacks).Without(x => x.TrainingProgram).Without(x => x.TrainingClassParticipates).Without(x => x.Location).Create();
            var mockId = mockTrainingClass.Id;
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.SoftRemove(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(mockId)).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.SoftRemoveTrainingClassAsync(mockId.ToString());

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository
                .SoftRemove(It.Is<TrainingClass>(
                    x => x.Equals(_mapperConfig.Map<TrainingClass>(mockTrainingClass)))
                ), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
            result.Should().BeTrue();
        }
        [Fact]
        public async Task SoftRemoveTrainingClass_ShouldReturnFalse_WhenSaveFail()
        {
            //arrange
            var mockTrainingClass = _fixture.Build<TrainingClass>().
                Without(x => x.Applications).
                Without(x => x.Attendances).Without(x => x.Feedbacks).
                Without(x => x.TrainingProgram).
                Without(x => x.TrainingClassParticipates).
                Without(x => x.Location)
                .With(x => x.StatusClassDetail)
                .With(x => x.Branch)
                .With(x => x.Attendee)
                .Create();
            var mockId = mockTrainingClass.Id;
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.SoftRemove(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(mockId)).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            //act
            var result = await _trainingClassService.SoftRemoveTrainingClassAsync(mockId.ToString());

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository
                .SoftRemove(It.Is<TrainingClass>(
                    x => x.Equals(_mapperConfig.Map<TrainingClass>(mockTrainingClass)))
                ), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CreateTrainingClass_ShouldReturnCorrectData_WhenSavedSucceed()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                Without(x => x.TrainingClasses).
                Without(x => x.DetailTrainingClassesParticipate)
                .Create();

            var mockAdmin = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Role)
                .With(x => x.RoleId, (int)RoleEnums.Admin).Create();

            var mockTrainingClassAdmins = _fixture.Build<AdminsDTO>()
                .With(x => x.AdminId, mockAdmin.Id)
                .CreateMany(1).ToList();

            var mockTrainer = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Role)
                .With(x => x.RoleId, (int)RoleEnums.Trainer).Create();

            var mockTrainingClassTrainers = _fixture.Build<TrainerDTO>()
                .With(x => x.TrainerId, mockTrainer.Id)
                .CreateMany(1).ToList();

            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            DateTime mockStartTime = new();
            DateTime mockEndTime = mockStartTime.AddHours(2);
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>().With(x => x.LocationID, mockLocation.Id).
                With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.StatusClassDetail)
                .With(x => x.Attendees)
                .With(x => x.TimeFrame)
                .With(x => x.LocationName, mockLocation.LocationName)
                .With(x => x.Admins, mockTrainingClassAdmins)
                .With(x => x.Trainers, mockTrainingClassTrainers)
                .With(x => x.StartTime, mockStartTime)
                .With(x => x.EndTime, mockEndTime)
                .Create();

            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockAdmin.Id)).ReturnsAsync(mockAdmin);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockTrainer.Id)).ReturnsAsync(mockTrainer);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x == mockAdmin.Id)), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x == mockTrainer.Id)), Times.Once);
            _unitOfWorkMock.Verify(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTrainingClass_ShouldThroWException_WhenInCorrectAdminId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                Without(x => x.TrainingClasses).
                Without(x => x.DetailTrainingClassesParticipate)
                .Create();

            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>().With(x => x.LocationID, mockLocation.Id).
                With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.StatusClassDetail)
                .With(x => x.Attendees)
                .With(x => x.TimeFrame)
                .With(x => x.LocationName, mockLocation.LocationName)
                .With(x => x.Admins)
                .Without(x => x.Trainers)
                .Create();

            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = async () => await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result!.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task CreateTrainingClass_ShouldThroWException_WhenInCorrectTrainerId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                Without(x => x.TrainingClasses).
                Without(x => x.DetailTrainingClassesParticipate)
                .Create();

            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>().With(x => x.LocationID, mockLocation.Id).
                With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.StatusClassDetail)
                .With(x => x.Attendees)
                .With(x => x.TimeFrame)
                .With(x => x.LocationName, mockLocation.LocationName)
                .Without(x => x.Admins)
                .With(x => x.Trainers)
                .Create();

            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = async () => await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result!.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task CreateTrainingClass_SaveChangeShouldBeAtLeastOnce_WhenLocationNameDoesNotExist()
        {
            //arrange
            Location mockLocation = null;
            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>()
                .With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.StatusClassDetail)
                .With(x => x.Attendees)
                .With(x => x.TimeFrame)
                .With(x => x.LocationName)
                .Without(x => x.Admins)
                .Without(x => x.Trainers)
                .Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateTrainingClass_ShouldReturnNull_WhenSavedFail()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                Without(x => x.TrainingClasses).
                Without(x => x.DetailTrainingClassesParticipate)
                .Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>()
                .With(x => x.LocationID, mockLocation.Id)
                .With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .Without(x => x.Attendees)
                .Without(x => x.TimeFrame)
                .Without(x => x.Admins)
                .Without(x => x.Trainers)
                .Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            //act
            var result = await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository
            .AddAsync(It.IsAny<TrainingClass>()), Times.Once);

            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);

            result.Should().BeNull();
        }
        [Fact]
        public async Task CreateTrainingClass_ShouldThrowException_WhenWrongLocationId()
        {
            //arrange
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>()
                .Without(x => x.Attendees)
                .Without(x => x.TimeFrame).Create();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            //act
            var result = async () => await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Never);

            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);

            await result.Should().ThrowAsync<Exception>("Invalid location Id");
        }
        [Fact]
        public async Task CreateTrainingClass_ShouldThrowException_WhenWrongProgramId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingClassesParticipate).Create();
            TrainingProgram mockTrainingProgram = null!;
            var mockCreate = _fixture.Build<CreateTrainingClassDTO>().With(x => x.LocationID, mockLocation.Id).Without(x => x.Attendees).Without(x => x.TimeFrame).Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            //act
            var result = async () => await _trainingClassService.CreateTrainingClassAsync(mockCreate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.AddAsync(It.IsAny<TrainingClass>()), Times.Never);

            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);

            await result.Should().ThrowAsync<Exception>("Invalid training program Id");
        }
        [Fact]
        public async Task UpdateTrainingClass_ShouldReturnTrue_WhenSaveSucceed()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>()
                .OmitAutoProperties()
                .With(x => x.LocationName)
                .Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>()
                .OmitAutoProperties()
                .Create();
            var mockAdmin = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id)
                .With(x => x.RoleId, (int)RoleEnums.Admin)
                .Create();
            var mockAdminDTO = _fixture.Build<AdminsDTO>()
                .With(x => x.AdminId, mockAdmin.Id)
                .CreateMany(1).ToList();
            var mockTrainer = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id)
                .With(x => x.RoleId, (int)RoleEnums.Trainer)
                .Create();
            var mockTrainerDTO = _fixture.Build<TrainerDTO>()
                .With(x => x.TrainerId, mockTrainer.Id)
                .CreateMany(1).ToList();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .OmitAutoProperties()
                .With(x => x.LocationName, mockLocation.LocationName)
                .With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Admins, mockAdminDTO)
                .With(x => x.Trainers, mockTrainerDTO)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockAdmin.Id)).ReturnsAsync(mockAdmin);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockTrainer.Id)).ReturnsAsync(mockTrainer);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.UpdateTrainingClassAsync(mockTrainingClass.Id.ToString(), mockUpdate);

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository
                .Update(It.IsAny<TrainingClass>()
                ), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
            result.Should().BeTrue();
        }
        [Fact]
        public async Task UpdateTrainingClass_ShouldThroWException_WhenInCorrectAdminId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                OmitAutoProperties()
                .Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>()
                .OmitAutoProperties().Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .OmitAutoProperties()
                .With(x => x.Admins)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .Create();
            User mockUser = null!;
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);

            //act
            var result = async () => await _trainingClassService.UpdateTrainingClassAsync(It.IsAny<Guid>().ToString(), mockUpdate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result!.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task UpdateTrainingClass_ShouldThroWException_WhenInCorrectTrainerId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().
                OmitAutoProperties()
                .Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>()
                .OmitAutoProperties().Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .OmitAutoProperties()
                .With(x => x.Trainers)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .Create();
            User mockUser = null!;
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);

            //act
            var result = async () => await _trainingClassService.UpdateTrainingClassAsync(It.IsAny<Guid>().ToString(), mockUpdate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result!.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task UpdateTrainingClass_WhenLocationNameIsNew()
        {
            //arrange
            Location mockLocation = null!;
            var mockTrainingProgram = _fixture.Build<TrainingProgram>()
                .OmitAutoProperties().Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .OmitAutoProperties()
                .With(x => x.LocationName)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .Create();
            User mockUser = null!;
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.UpdateTrainingClassAsync(mockTrainingClass.Id.ToString(), mockUpdate);

            //assert
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.AtLeastOnce);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateTrainingClass_ShouldReturnFalse_WhenSaveFail()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingClassesParticipate).Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .With(x => x.LocationID, mockLocation.Id)
                .With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .Without(x => x.Admins)
                .Without(x => x.Trainers)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Applications)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.TrainingClassAdmins)
                .Without(x => x.TrainingClassTrainers)
                .Without(x => x.Location).Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            var expected = _mapperConfig.Map<UpdateTrainingClassDTO, TrainingClass>(mockUpdate, mockTrainingClass);
            expected.Location = mockLocation;
            expected.TrainingProgram = mockTrainingProgram;
            //act
            var result = await _trainingClassService.UpdateTrainingClassAsync(mockTrainingClass.Id.ToString(), mockUpdate);

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository
                .Update(It.Is<TrainingClass>(
                    x => x.Equals(expected))
                ), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once());
            result.Should().BeFalse();
        }
        [Fact]
        public async Task UpdateTrainingClass_ShouldThrowException_WhenWrongLocationId()
        {
            //arrange
            var mockLocation = new Location();
            var mockTrainingProgram = new TrainingProgram();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>().With(x => x.TrainingProgramId, mockTrainingProgram.Id).Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Applications)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.TrainingClassAdmins)
                .Without(x => x.TrainingClassTrainers)
                .Without(x => x.Location).Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockLocation.Id)))).ReturnsAsync(mockLocation);
            //act
            var result = async () => await _trainingClassService.UpdateTrainingClassAsync(mockTrainingClass.Id.ToString(), mockUpdate);

            //assert
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result.Should().ThrowAsync<NullReferenceException>();
        }
        [Fact]
        public async Task UpdateTrainingClass_ShouldThrowException_WhenWrongTrainingProgramId()
        {
            //arrange
            var mockLocation = _fixture.Build<Location>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingClassesParticipate).Create();
            var mockTrainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.TrainingClasses).Without(x => x.DetailTrainingProgramSyllabus).Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>().With(x => x.LocationID, mockLocation.Id).Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>().Without(x => x.TrainingClassParticipates).Without(x => x.TrainingProgram).Without(x => x.Applications).Without(x => x.Attendances).Without(x => x.Feedbacks).Without(x => x.Location).Create();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingProgram.Id)))).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var expected = _mapperConfig.Map<UpdateTrainingClassDTO, TrainingClass>(mockUpdate, mockTrainingClass);
            expected.Location = mockLocation;
            expected.TrainingProgram = mockTrainingProgram;
            //act
            var result = async () => await _trainingClassService.UpdateTrainingClassAsync(mockTrainingClass.Id.ToString(), mockUpdate);

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository
                .Update(It.Is<TrainingClass>(
                    x => x.Equals(expected))
                ), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await result.Should().ThrowAsync<NullReferenceException>();
        }
        [Fact]
        public async Task UpdateTrainingClass_SaveChangeShouldBeAtLeastOnce_WhenLocationNameDoesNotExist()
        {
            //arrange
            Location mockLocation = null;
            var mockTrainingProgram = _fixture.Build<TrainingProgram>()
                .Without(x => x.TrainingClasses)
                .Without(x => x.DetailTrainingProgramSyllabus)
                .Create();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>()
                .With(x => x.TrainingProgramId, mockTrainingProgram.Id)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.StatusClassDetail)
                .With(x => x.Attendees)
                .With(x => x.TimeFrame)
                .With(x => x.LocationName)
                .Without(x => x.Admins)
                .Without(x => x.Trainers)
                .Create();
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Applications)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.TrainingClassAdmins)
                .Without(x => x.TrainingClassTrainers)
                .Without(x => x.Location).Create();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.LocationRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(mockLocation);
            _unitOfWorkMock.Setup(x => x.TrainingProgramRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingProgram);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            //act
            var result = await _trainingClassService.UpdateTrainingClassAsync(Guid.NewGuid().ToString(), mockUpdate);

            //Assert
            _unitOfWorkMock.Verify(x => x.TrainingClassRepository.Update(It.IsAny<TrainingClass>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.AtLeastOnce);
        }
        [Fact]
        public async void GetTrainingClassByIdAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockId = Guid.NewGuid();
            TrainingClass mockTrainingClass = new();
            _unitOfWorkMock.Setup(
                x => x.TrainingClassRepository.GetByIdAsync(
                    mockId)).ReturnsAsync(mockTrainingClass);

            //act
            var result = await _trainingClassService.GetTrainingClassByIdAsync(mockId.ToString());

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository.GetByIdAsync(
                    It.Is<Guid>(e => e.Equals(mockId))), Times.Once);
            result.Should().Be(mockTrainingClass);
        }
        [Fact]
        public async Task GetTrainingClassByIdAsync_ShouldThrowException_WhenIdIsIncorrect()
        {
            //arrange
            var mockId = Guid.NewGuid();
            TrainingClass mockTrainingClass = new();
            _unitOfWorkMock.Setup(
                x => x.TrainingClassRepository.GetByIdAsync(
                    It.Is<Guid>(e => e.Equals(mockId)))).ReturnsAsync(mockTrainingClass);

            //act
            var result = async () => await _trainingClassService.GetTrainingClassByIdAsync(Guid.NewGuid().ToString());

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository.GetByIdAsync(
                    It.Is<Guid>(e => e.Equals(mockId))), Times.Never);
            await result.Should().ThrowAsync<NullReferenceException>("Incorrect Id");
        }
        [Fact]
        public async Task GetTrainingClassByIdAsync_ShouldThrowException_WhenGetMappingError()
        {
            //arrange
            var mockId = Guid.NewGuid();
            TrainingClass mockTrainingClass = new();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockTrainingClass);

            //act
            var result = async () => await _trainingClassService.GetTrainingClassByIdAsync("abc123");

            //assert
            _unitOfWorkMock.Verify(
                x => x.TrainingClassRepository.GetByIdAsync(
                    It.Is<Guid>(e => e.Equals(mockId))), Times.Never);
            await result.Should().ThrowAsync<AutoMapperMappingException>("Id is not a guid");
        }
        [Fact]
        public async Task GetDetailTrainingClass_ShouldReturnData_WhenClasExsited()
        {
            //arrange
            var trainingClass = _fixture.Build<TrainingClass>()
                                        .With(x => x.Id)
                                        .With(x => x.Name)
                                        .With(x => x.Code)
                                        .With(x => x.Branch)
                                        .With(x => x.CreatedBy)
                                        .With(x => x.CreationDate)
                                        .With(x => x.StatusClassDetail)
                                        .With(x => x.LocationID)
                                        .With(x=>x.StartTime)
                                        .With(x=>x.EndTime)
                                        .With(x=>x.ModificationDate)
                                        .Without(x => x.TrainingClassParticipates)
                                        .Without(x => x.TrainingProgram)
                                        .Without(x => x.ClassSchedules)
                                        .Without(x => x.Feedbacks)
                                        .Without(x => x.Applications)
                                        .Without(x => x.Location)
                                        .Without(x => x.Attendances)
                                        .Create<TrainingClass>();
            var trainingProgram = _fixture.Build<TrainingProgram>()
                                      .With(x => x.Id)
                                      .Without(x => x.DetailTrainingProgramSyllabus)
                                      .Without(x => x.TrainingClasses)
                                      .Create<TrainingProgram>();
            var programViewAll = _fixture.Build<TrainingProgramViewForTrainingClassDetail>()
                                        .With(x => x.programId, trainingProgram.Id)
                                        .With(x => x.programName, trainingProgram.ProgramName).Create();
            var lastEditDTO = _fixture.Build<LastEditDTO>()
                                     .With(x => x.modificationBy, "lmao")
                                     .With(x => x.modificationDate, trainingClass.ModificationDate)
                                     .Create();
            var syllabus = _fixture.Build<Syllabus>()
                                 .Without(s => s.Units)
                                 .Without(s => s.DetailTrainingProgramSyllabus)
                                 .Without(s=>s.User)
                                 .With(s => s.Id)
                                 .With(s => s.SyllabusName)
                                 .With(s => s.Duration)
                                 .With(s => s.Status)
                                 .CreateMany(3).ToList();
            var syllabusViewAll = _mapperConfig.Map<List<SyllabusViewForTrainingClassDetail>>(syllabus);
            var userMock = _fixture.Build<User>()
                                  .Without(x => x.Syllabuses)
                                  .Without(x => x.Applications)
                                  .Without(x => x.SubmitQuizzes)
                                  .Without(x => x.DetailTrainingClassParticipate)
                                  .Without(x => x.Attendances)
                                  .Without(x => x.Feedbacks)
                                  .With(x => x.Id)
                                  .Create<User>();
            var classDuration = _fixture.Build<DurationView>()
                                    .With(x => x.TotalHours, trainingClass.Duration)
                                    .Create();

            var allClass = _fixture.Build<TrainingClassFilterDTO>()
                           .With(x => x.ClassID, trainingClass.Id)
                           .With(x => x.Name, trainingClass.Name)
                           .With(x => x.Code, trainingClass.Code)
                           .With(x => x.StartDate, trainingClass.StartTime)
                           .With(x => x.EndDate, trainingClass.EndTime)
                           .With(x => x.CreationDate, trainingClass.CreationDate)
                           .With(x => x.LocationName)
                           .With(x => x.Branch, trainingClass.Branch)
                           .With(x => x.Attendee, trainingClass.Attendee)
                           .With(x => x.ClassDuration, classDuration)
                           .With(x => x.LastEditDTO, lastEditDTO)
                           .Create();
            var trainer = _fixture.Build<ClassTrainerDTO>()
                                .With(t => t.trainerId, userMock.Id)
                                .CreateMany(2).ToList();
            var admin = _fixture.Build<ClassAdminDTO>()
                               .With(ad => ad.adminID, userMock.Id)
                               .CreateMany(2).ToList();
            var attend = _fixture.Build<AttendeeDTO>()
                               .With(a => a.attendee, allClass.Attendee)
                               .With(a => a.plannedNumber, 100)
                               .With(a => a.acceptedNumber, 50)
                               .With(a => a.actualNumber, 40)
                               .Create();
            var created = _fixture.Build<CreatedByDTO>()
                                 .With(c => c.userName, allClass.CreatedBy)
                                 .With(c => c.creationDate, allClass.CreationDate)
                                 .Create();
            var dateOrder = _fixture.Build<DateOrderForViewDetail>()
                                  .With(date => date.current, 10)
                                  .With(date => date.total, 10 * 3)
                                  .Create();
            var classDate = _fixture.Build<ClassDateDTO>()
                                  .With(x => x.StartDate, allClass.StartDate)
                                  .With(x => x.EndDate, allClass.EndDate)
                                  .Create();

            var general = _fixture.Build<GeneralTrainingClassDTO>()
                                 .With(x => x.class_date, classDate).Create();
            var finalTrainingClass = _fixture.Build<FinalTrainingClassDTO>()
                                           .With(x => x.classId, trainingClass.Id)
                                           .With(x => x.className, trainingClass.Name)
                                           .With(x => x.classCode, trainingClass.Code)
                                           .With(x => x.classStatus, trainingClass.StatusClassDetail)

                                           .With(x => x.classDuration, classDuration)
                                           .With(x => x.attendeesDetail, attend)
                                           .With(x => x.syllabuses, syllabusViewAll)
                                           .With(x => x.trainingPrograms, programViewAll)
                                           .With(x => x.admin, admin)
                                           .With(x => x.trainer, trainer)
                                           .With(x => x.created, created)
                                           .With(x => x.general, general)
                                           .Create();
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingProgramByClassID(trainingClass.Id)).Returns(programViewAll);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassForViewDetailById(trainingClass.Id)).Returns(allClass);
            _unitOfWorkMock.Setup(x => x.DetailTrainingProgramSyllabusRepository.GetDetailByClassID(programViewAll.programId)).Returns(syllabusViewAll);
             _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipateRepository.GetDetailTrainingClassParticipatesByClassIDAsync(trainingClass.Id)).ReturnsAsync(trainer);
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipateRepository.GetAdminInClasssByClassIDAsync(trainingClass.Id)).ReturnsAsync(admin);
            //act
            var result = await _trainingClassService.GetFinalTrainingClassesAsync(trainingClass.Id);
            //assert
            result.Should().BeOfType<FinalTrainingClassDTO>();
            result.Should().NotBeNull();
            result.trainingPrograms.Should().BeEquivalentTo(programViewAll);
            result.admin.Should().BeEquivalentTo(admin);
            result.trainer.Should().BeEquivalentTo(trainer);
            result.fsu.Should().BeEquivalentTo(allClass.Branch);
            result.classCode.Should().BeEquivalentTo(allClass.Code);
            result.className.Should().BeEquivalentTo(allClass.Name);
            result.syllabuses.Should().BeEquivalentTo(syllabusViewAll);

        }
        [Fact]
        public async Task DuplicateClassAsync_Should_Return_True_When_Class_Is_Duplicated()
        {
            // Arrange
            /*            var classId = Guid.NewGuid();*/
            var trainingClass = _fixture.Build<TrainingClass>()
                                        .With(x => x.Id)
                                        .With(x => x.Name)
                                        .With(x => x.Code)
                                        .With(x => x.Branch)
                                        .With(x => x.CreatedBy)
                                        .With(x => x.CreationDate)
                                        .With(x => x.StatusClassDetail)
                                        .With(x => x.LocationID)
                                        .Without(x => x.TrainingClassParticipates)
                                        .Without(x => x.TrainingProgram)
                                        .Without(x => x.ClassSchedules)
                                        .Without(x => x.Feedbacks)
                                        .Without(x => x.Applications)
                                        .Without(x => x.Location)
                                        .Without(x => x.Attendances)
                                        .Create<TrainingClass>();

            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddAsync(trainingClass));
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetByIdAsync(trainingClass.Id)).ReturnsAsync(trainingClass);
            // Act
            var result = await _trainingClassService.DuplicateClassAsync(trainingClass.Id);

            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task SearchClassByName_ShouldReturnNull_WhenSerachValueIsEmpty()
        {
            //arrange
            var trainingClass = _fixture.Build<TrainingClass>()
                                       .With(x => x.Id)
                                       .With(x => x.Name)
                                       .With(x => x.Code)
                                       .With(x => x.Branch)
                                       .With(x => x.CreatedBy)
                                       .With(x => x.CreationDate)
                                       .With(x => x.StatusClassDetail)
                                       .With(x => x.LocationID)
                                       .Without(x => x.TrainingClassParticipates)
                                       .Without(x => x.TrainingProgram)
                                       .Without(x => x.ClassSchedules)
                                       .Without(x => x.Feedbacks)
                                       .Without(x => x.Applications)
                                       .Without(x => x.Location)
                                       .Without(x => x.Attendances)
                                       .CreateMany(3).ToList();
            var viewAllClass = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(trainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddRangeAsync(trainingClass));
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            foreach( TrainingClass training in trainingClass)
            {
                _unitOfWorkMock.Setup(x => x.TrainingClassRepository.SearchClassByName(training.Name)).Returns(viewAllClass = null);
            }
            List<TrainingClassViewAllDTO> result=new List<TrainingClassViewAllDTO>();
            //Act 
           foreach(TrainingClass trainingClass1 in trainingClass)
            {
                 result = await _trainingClassService.SearchClassByNameAsync(trainingClass1.Name);
            }
          
            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task SearchClassByName_ShouldReturnData_WhenSerachValueIsNotEmpty()
        {
            //arrange
            var trainingClass = _fixture.Build<TrainingClass>()
                                       .With(x => x.Id)
                                       .With(x => x.Name)
                                       .With(x => x.Code)
                                       .With(x => x.Branch)
                                       .With(x => x.CreatedBy)
                                       .With(x => x.CreationDate)
                                       .With(x => x.StatusClassDetail)
                                       .With(x => x.LocationID)
                                       .Without(x => x.TrainingClassParticipates)
                                       .Without(x => x.TrainingProgram)
                                       .Without(x => x.ClassSchedules)
                                       .Without(x => x.Feedbacks)
                                       .Without(x => x.Applications)
                                       .Without(x => x.Location)
                                       .Without(x => x.Attendances)
                                       .CreateMany(3).ToList();
            var viewAllClass = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(trainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.AddRangeAsync(trainingClass));
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            foreach (TrainingClass training in trainingClass)
            {
                _unitOfWorkMock.Setup(x => x.TrainingClassRepository.SearchClassByName(training.Name)).Returns(viewAllClass);
            }
            List<TrainingClassViewAllDTO> result = new List<TrainingClassViewAllDTO>();
            //Act 
            foreach (TrainingClass trainingClass1 in trainingClass)
            {
                result = await _trainingClassService.SearchClassByNameAsync(trainingClass1.Name);
            }

            // Assert
            result.Should().BeEquivalentTo<TrainingClassViewAllDTO>(viewAllClass) ;
        }

        [Fact]
        public async Task FilterTrainingClass_ShouldReturnData_WhenNull()
        {
            //arrange

            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                          .Without(x => x.TrainingClassParticipates)
                                          .Without(x => x.TrainingProgram)
                                          .Without(x => x.Location)
                                          .Without(x => x.TrainingProgramId)
                                          .Without(x => x.LocationID)
                                          .Without(x => x.Attendances)
                                          .Without(x => x.Applications)
                                          .Without(x => x.Feedbacks)
                                          .Without(x => x.ClassSchedules)
                                          .With(x => x.Attendee, "Intern")
                                          .With(x => x.StatusClassDetail, "Online")
                                          .With(x => x.Branch, "FHM")
                                          .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, null, null, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);

        }
     
        [Fact]
        public async Task FilterTrainingClass_ShouldReturnData_WhenBranchNameIsNotNull()
        {

            //arrange
            var filterMock = new TrainingClassFilterModel
            {
                branchName = "FHM"
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                          .Without(x => x.TrainingClassParticipates)
                                          .Without(x => x.TrainingProgram)
                                          .Without(x => x.Location)
                                          .Without(x => x.TrainingProgramId)
                                          .Without(x => x.LocationID)
                                          .Without(x => x.Attendances)
                                          .Without(x => x.Applications)
                                          .Without(x => x.Feedbacks)
                                          .Without(x => x.ClassSchedules)
                                          .With(x => x.Attendee, "Intern")
                                          .With(x => x.StatusClassDetail, "Online")
                                          .With(x => x.Branch, "FHM")
                                          .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, filterMock.branchName, null, null, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }


        [Fact]
        public async Task FilterTrainingClass_ShouldReturnData_WhenFirstDateIsNotNull()
        {

            //arrange
            var filterMock = new TrainingClassFilterModel
            {
                date1 = DateTime.Parse("2023-02-22")
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                          .Without(x => x.TrainingClassParticipates)
                                          .Without(x => x.TrainingProgram)
                                          .Without(x => x.Location)
                                          .Without(x => x.TrainingProgramId)
                                          .Without(x => x.LocationID)
                                          .Without(x => x.Attendances)
                                          .Without(x => x.Applications)
                                          .Without(x => x.Feedbacks)
                                          .Without(x => x.ClassSchedules)
                                          .With(x => x.StartTime, DateTime.Parse("2023-02-28"))
                                          .With(x => x.EndTime, DateTime.Parse("2023-03-31"))
                                          .With(x => x.Attendee, "Intern")
                                          .With(x => x.StatusClassDetail, "Online")
                                          .With(x => x.Branch, "FHM")
                                          .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, filterMock.date1, null, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
        [Fact]
        public async Task FilterTrainingClass_ShouldReturnData_WhenSecondDateIsNotNull()
        {

            //arrange
            var filterMock = new TrainingClassFilterModel
            {
                date2 = DateTime.Parse("2023-04-01")
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                          .Without(x => x.TrainingClassParticipates)
                                          .Without(x => x.TrainingProgram)
                                          .Without(x => x.Location)
                                          .Without(x => x.TrainingProgramId)
                                          .Without(x => x.LocationID)
                                          .Without(x => x.Attendances)
                                          .Without(x => x.Applications)
                                          .Without(x => x.Feedbacks)
                                          .Without(x => x.ClassSchedules)
                                          .With(x => x.StartTime, DateTime.Parse("2023-02-28"))
                                          .With(x => x.EndTime, DateTime.Parse("2023-03-31"))
                                          .With(x => x.Attendee, "Intern")
                                          .With(x => x.StatusClassDetail, "Online")
                                          .With(x => x.Branch, "FHM")
                                          .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, null, filterMock.date2, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
        [Fact]
        public async Task FilterTrainingClass_ShouldReturnData_WhenFirstAndSecondDateIsNotNull()
        {

            //arrange
            var filterMock = new TrainingClassFilterModel
            {
                date1 = DateTime.Parse("2023-02-22"),
                date2 = DateTime.Parse("2023-04-02")
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                          .Without(x => x.TrainingClassParticipates)
                                          .Without(x => x.TrainingProgram)
                                          .Without(x => x.Location)
                                          .Without(x => x.TrainingProgramId)
                                          .Without(x => x.LocationID)
                                          .Without(x => x.Attendances)
                                          .Without(x => x.Applications)
                                          .Without(x => x.Feedbacks)
                                          .Without(x => x.ClassSchedules)
                                          .With(x => x.StartTime, DateTime.Parse("2023-02-28"))
                                          .With(x => x.EndTime, DateTime.Parse("2023-03-31"))
                                          .With(x => x.Attendee, "Intern")
                                          .With(x => x.StatusClassDetail, "Online")
                                          .With(x => x.Branch, "FHM")
                                          .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, filterMock.date1, filterMock.date2, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
        [Fact]
        public async Task FilterClass_ShouldReturnData_WhenClassStatusIsNotNull()
        {
            string[] classStatus = { "Active" };
            //arrange
            var filterMock = new TrainingClassFilterModel()
            {
                classStatus = classStatus,
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                         .Without(x => x.TrainingClassParticipates)
                                         .Without(x => x.TrainingProgram)
                                         .Without(x => x.Location)
                                         .Without(x => x.TrainingProgramId)
                                         .Without(x => x.LocationID)
                                         .Without(x => x.Attendances)
                                         .Without(x => x.Applications)
                                         .Without(x => x.Feedbacks)
                                         .Without(x => x.ClassSchedules)
                                         .With(x => x.StartTime, DateTime.UtcNow.AddDays(-30))
                                         .With(x => x.EndTime, DateTime.UtcNow.AddDays(3))
                                         .With(x => x.Attendee, "Intern")
                                         .With(x => x.StatusClassDetail, "Active")
                                         .With(x => x.Branch, "FHM")
                                         .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, null, null, filterMock.classStatus, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
        [Fact]
        public async Task FilterClass_ShouldReturnData_WhenLocationNameIsNotNull()
        {
            string[] locationName = { "Ftown2", "Ftown1" };
            //arrange
            var filterMock = new TrainingClassFilterModel()
            {
                locationName = locationName,
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                         .Without(x => x.TrainingClassParticipates)
                                         .Without(x => x.TrainingProgram)
                                         .Without(x => x.Location)
                                         .Without(x => x.TrainingProgramId)
                                         .Without(x => x.LocationID)
                                         .Without(x => x.Attendances)
                                         .Without(x => x.Applications)
                                         .Without(x => x.Feedbacks)
                                         .Without(x => x.ClassSchedules)
                                         .With(x => x.StartTime, DateTime.UtcNow.AddDays(-30))
                                         .With(x => x.EndTime, DateTime.UtcNow.AddDays(3))
                                         .With(x => x.Attendee, "Intern")
                                         .With(x => x.StatusClassDetail, "Active")
                                         .With(x => x.Branch, "FHM")
                                         .CreateMany(3).ToList();
            var filterMockList = _fixture.Build<TrainingClassFilterDTO>()
                                         .With(x => x.LocationName, "Ftown2")
                                         .CreateMany(3).ToList();
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            foreach (var item in filterMockData)
            {
                foreach (var filterItem in filterMockList)
                {
                    item.LocationName = filterItem.LocationName;

                }
            }
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(filterMock.locationName, null, null, null, null, null, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
        [Fact]
        public async Task FilterClass_ShouldReturnData_WhenAttendeeIsNotNull()
        {
            string[] attendee = { "Intern", "Online fee-fresher", "Fresher", "Openning" };
            //arrange
            var filterMock = new TrainingClassFilterModel()
            {
                attendInClass = attendee,
            };
            var mockTrainingClass = _fixture.Build<TrainingClass>()
                                         .Without(x => x.TrainingClassParticipates)
                                         .Without(x => x.TrainingProgram)
                                         .Without(x => x.Location)
                                         .Without(x => x.TrainingProgramId)
                                         .Without(x => x.LocationID)
                                         .Without(x => x.Attendances)
                                         .Without(x => x.Applications)
                                         .Without(x => x.Feedbacks)
                                         .Without(x => x.ClassSchedules)
                                         .With(x => x.StartTime, DateTime.UtcNow.AddDays(-30))
                                         .With(x => x.EndTime, DateTime.UtcNow.AddDays(3))
                                         .With(x => x.Attendee, "Intern")
                                         .With(x => x.StatusClassDetail, "Active")
                                         .With(x => x.Branch, "FHM")
                                         .CreateMany(3).ToList();
            var dataFilter1 = _fixture.Build<TrainingClassFilterDTO>()
                                         .With(x => x.LocationName, "Ftown2")
                                         .With(x => x.Attendee, "Intern")
                                         .Create();
            var dataFilter2 = _fixture.Build<TrainingClassFilterDTO>()
                                        .With(x => x.LocationName, "Ftown2")
                                        .With(x => x.Attendee, "Online fee-fresher")
                                        .Create();
            var dataFilter3 = _fixture.Build<TrainingClassFilterDTO>()
                                      .With(x => x.LocationName, "Ftown2")
                                      .With(x => x.Attendee, "Fresher")
                                      .Create();
            var filterMockList = _fixture.Build<TrainingClassFilterDTO>()
                                       .CreateMany(3).ToList();
            filterMockList.Add(dataFilter1);
            filterMockList.Add(dataFilter2);
            filterMockList.Add(dataFilter3);
            var filterMockData = _mapperConfig.Map<List<TrainingClassFilterDTO>>(mockTrainingClass);
            foreach (var item in filterMockData)
            {
                foreach (var filterItem in filterMockList)
                {
                    item.LocationName = filterItem.LocationName;
                }
            }
            _unitOfWorkMock.Setup(x => x.TrainingClassRepository.GetTrainingClassesForFilter()).Returns(filterMockData);
            var filterResult = _mapperConfig.Map<List<TrainingClassViewAllDTO>>(filterMockData);
            //act
            var result = await _trainingClassService.FilterLocation(null, null, null, null, null, filterMock.attendInClass, null);
            //assert
            result.Should().BeEquivalentTo(filterResult);
        }
    
       

        [Fact]
        public async Task CheckTrainingClassAdminsIdAsync_ShouldReturnTrue()
        {
            //arrange
            var mockTrainingClassAdmin = _fixture.Build<TrainingClassAdmin>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)RoleEnums.Admin)
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassAdminsIdAsync(mockTrainingClassAdmin);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassAdmin.AdminId))), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckTrainingClassAdminsIdAsync_ShouldReturnFalse_WhenAdminIdIsWrong()
        {
            //arrange
            var mockTrainingClassAdmin = _fixture.Build<TrainingClassAdmin>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)RoleEnums.Admin)
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockUser.Id)))).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassAdminsIdAsync(mockTrainingClassAdmin);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassAdmin.AdminId))), Times.Once);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckTrainingClassAdminsIdAsync_ShouldReturnFalse_WhenRoleIdIsWrong()
        {
            //arrange
            var mockTrainingClassAdmin = _fixture.Build<TrainingClassAdmin>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)It.Is<RoleEnums>(x => !x.Equals(RoleEnums.Admin)))
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassAdminsIdAsync(mockTrainingClassAdmin);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassAdmin.AdminId))), Times.Once);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckTrainingClassTrainersIdAsync_ShouldReturnTrue()
        {
            //arrange
            var mockTrainingClassTrainer = _fixture.Build<TrainingClassTrainer>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)RoleEnums.Trainer)
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassTrainersIdAsync(mockTrainingClassTrainer);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassTrainer.TrainerId))), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckTrainingClassTrainersIdAsync_ShouldReturnFalse_WhenAdminIdIsWrong()
        {
            //arrange
            var mockTrainingClassTrainer = _fixture.Build<TrainingClassTrainer>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)RoleEnums.Trainer)
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockUser.Id)))).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassTrainersIdAsync(mockTrainingClassTrainer);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassTrainer.TrainerId))), Times.Once);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CheckTrainingClassTrainersIdAsync_ShouldReturnFalse_WhenRoleIdIsWrong()
        {
            //arrange
            var mockTrainingClassTrainer = _fixture.Build<TrainingClassTrainer>()
                .Without(x => x.TrainingClass)
                .Create();
            var mockUser = _fixture.Build<User>()
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .With(x => x.RoleId, (int)It.Is<RoleEnums>(x => !x.Equals(RoleEnums.Trainer)))
                .Create();
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mockUser);

            //act
            var result = await _trainingClassService.CheckTrainingClassTrainersIdAsync(mockTrainingClassTrainer);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(It.Is<Guid>(x => x.Equals(mockTrainingClassTrainer.TrainerId))), Times.Once);
            result.Should().BeFalse();
        }
    }
}
