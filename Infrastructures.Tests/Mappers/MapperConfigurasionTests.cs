using Application.Commons;
using Application.ViewModels.Location;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domains.Test;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Mappers
{
    public class MapperConfigurasionTests : SetupTest
    {
        [Fact]
        public void User_UserViewModel()
        {
            //arrange
            var user = _fixture.Build<User>()
                .OmitAutoProperties()
                .With(x => x.Id)
                .Create();

            //act
            var result = _mapperConfig.Map<UserViewModel>(user);

            //assert
            result._Id.Should().Be(user.Id.ToString());
        }

        [Fact]
        public void ExtendTrainingClassDTO2_ExtendTrainingClassDTO()
        {
            //arrange
            var extend2Mock = _fixture.Build<ExtendTrainingClassDTO2>().Create();

            //act
            var result = _mapperConfig.Map<ExtendTrainingClassDTO>(extend2Mock);

            //assert
            result.Admins.Should().BeEquivalentTo(extend2Mock.General.Admins);
            result.Trainers.Should().BeEquivalentTo(extend2Mock.General.Trainers);
            result.TimeFrame.Should().BeEquivalentTo(extend2Mock.TimeFrame);
            result.Attendees.Should().BeEquivalentTo(extend2Mock.Attendees);
            result.Fsu.Should().BeEquivalentTo(extend2Mock.General.Fsu);
            result.ReviewDate.Should().Be(extend2Mock.General.Review!.Date);
            result.ReviewAuthor.Should().Be(extend2Mock.General.Review.Author);
            result.ApproveDate.Should().Be(extend2Mock.General.Approve!.Date);
            result.ApproveAuthor.Should().Be(extend2Mock.General.Approve.Author);
        }

        [Fact]
        public void ExtendTrainingClassDTO2_ExtendTrainingClassDTO_ReverseMap()
        {
            //arrange
            var extendMock = _fixture.Build<ExtendTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<ExtendTrainingClassDTO2>(extendMock);

            //assert
            extendMock.Admins.Should().BeEquivalentTo(result.General.Admins);
            extendMock.Trainers.Should().BeEquivalentTo(result.General.Trainers);
            extendMock.TimeFrame.Should().BeEquivalentTo(result.TimeFrame);
            extendMock.Attendees.Should().BeEquivalentTo(result.Attendees);
            extendMock.Fsu.Should().BeEquivalentTo(result.General.Fsu);
            extendMock.ReviewDate.Should().Be(result.General.Review!.Date);
            extendMock.ReviewAuthor.Should().Be(result.General.Review.Author);
            extendMock.ApproveDate.Should().Be(result.General.Approve!.Date);
            extendMock.ApproveAuthor.Should().Be(result.General.Approve.Author);
        }

        [Fact]
        public void GeneralDTO_ExtendTrainingClassDTO()
        {
            //arrange
            var generalDTOMock = _fixture.Build<GeneralDTO>()
                .Without(x => x.ClassTime)
                .Without(x => x.Location)
                .Create();

            //act
            var result = _mapperConfig.Map<ExtendTrainingClassDTO>(generalDTOMock);

            //assert
            result.Admins.Should().BeEquivalentTo(generalDTOMock.Admins);
            result.Trainers.Should().BeEquivalentTo(generalDTOMock.Trainers);
            result.Fsu.Should().BeEquivalentTo(generalDTOMock.Fsu);
            result.ReviewDate.Should().Be(generalDTOMock.Review!.Date);
            result.ReviewAuthor.Should().Be(generalDTOMock.Review.Author);
            result.ApproveDate.Should().Be(generalDTOMock.Approve!.Date);
            result.ApproveAuthor.Should().Be(generalDTOMock.Approve.Author);
        }

        [Fact]
        public void GeneralDTO_ExtendTrainingClassDTO_ReverseMap()
        {
            //arrange
            var result = _fixture.Build<ExtendTrainingClassDTO>()
                .Create();

            //act
            var generalDTOMock = _mapperConfig.Map<GeneralDTO>(result);

            //assert
            result.Admins.Should().BeEquivalentTo(generalDTOMock.Admins);
            result.Trainers.Should().BeEquivalentTo(generalDTOMock.Trainers);
            result.Fsu.Should().BeEquivalentTo(generalDTOMock.Fsu);
            result.ReviewDate.Should().Be(generalDTOMock.Review!.Date);
            result.ReviewAuthor.Should().Be(generalDTOMock.Review.Author);
            result.ApproveDate.Should().Be(generalDTOMock.Approve!.Date);
            result.ApproveAuthor.Should().Be(generalDTOMock.Approve.Author);
        }

        [Fact]
        public void ReviewDTO_ExtendTrainingClassDTO()
        {
            //arrange
            var reviewDTOMock = _fixture.Build<ReviewDTO>().Create();

            //act
            var result = _mapperConfig.Map<ExtendTrainingClassDTO>(reviewDTOMock);
            var reverseResult = _mapperConfig.Map<ReviewDTO>(result);

            //assert
            result.ReviewDate.Should().Be(reviewDTOMock.Date);
            result.ReviewAuthor.Should().Be(reviewDTOMock.Author);
            result.ReviewDate.Should().Be(reverseResult.Date);
            result.ReviewAuthor.Should().Be(reverseResult.Author);
        }

        [Fact]
        public void ApproveDTO_ExtendTrainingClassDTO()
        {
            //arrange
            var approveDTOMock = _fixture.Build<ApproveDTO>().Create();

            //act
            var result = _mapperConfig.Map<ExtendTrainingClassDTO>(approveDTOMock);
            var reverseResult = _mapperConfig.Map<ApproveDTO>(result);

            //assert
            result.ApproveDate.Should().Be(approveDTOMock.Date);
            result.ApproveAuthor.Should().Be(approveDTOMock.Author);
            result.ApproveDate.Should().Be(reverseResult.Date);
            result.ApproveAuthor.Should().Be(reverseResult.Author);
        }

        [Fact]
        public void CreateTrainingClassDTO_TrainingClass()
        {
            //arrange
            var createDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<TrainingClass>(createDTOMock);

            //assert
            result.Name.Should().Be(createDTOMock.Name);
            result.Code.Should().Be(createDTOMock.Code);
            result.StatusClassDetail.Should().Be(createDTOMock.StatusClassDetail);
            result.LocationID.Should().Be(createDTOMock.LocationID);
            result.Location!.LocationName.Should().Be(createDTOMock.LocationName);
            result.TrainingProgramId.Should().Be(createDTOMock.TrainingProgramId);
            result.StartTime.Should().Be(createDTOMock.StartTime);
            result.EndTime.Should().Be(createDTOMock.EndTime);
            result.TrainingClassAdmins.Should().BeEquivalentTo(createDTOMock.Admins);
            result.TrainingClassTrainers.Should().BeEquivalentTo(createDTOMock.Trainers);
            result.TrainingClassAttendee.Should().BeEquivalentTo(createDTOMock.Attendees);
            result.Fsu.Should().BeEquivalentTo(createDTOMock.Fsu);
            result.ReviewDate.Should().Be(createDTOMock.ReviewDate);
            result.ReviewAuthor.Should().Be(createDTOMock.ReviewAuthor);
            result.ApproveDate.Should().Be(createDTOMock.ApproveDate);
            result.ApproveAuthor.Should().Be(createDTOMock.ApproveAuthor);
            result.TrainingClassTimeFrame.StartDate.Should().Be(createDTOMock.TimeFrame.StartDate);
            result.TrainingClassTimeFrame.EndDate.Should().Be(createDTOMock.TimeFrame.EndDate);
            result.TrainingClassTimeFrame.HighlightedDates!
                .Select(x => x.HighlightedDate).ToList()
                .Should().BeEquivalentTo(createDTOMock.TimeFrame.HighlightedDates);
            result.Branch.Should().Be(createDTOMock.Branch);
            result.Attendee.Should().Be(createDTOMock.Attendee);
        }

        [Fact]
        public void CreateTrainingClassDTO_TrainingClass_ReverseMap()
        {
            //arrange
            var createDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();
            var trainingClassMock = _mapperConfig.Map<TrainingClass>(createDTOMock);

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO>(trainingClassMock);

            //assert
            trainingClassMock.Name.Should().Be(result.Name);
            trainingClassMock.Code.Should().Be(result.Code);
            trainingClassMock.StatusClassDetail.Should().Be(result.StatusClassDetail);
            trainingClassMock.LocationID.Should().Be(result.LocationID);
            trainingClassMock.Location!.LocationName.Should().Be(result.LocationName);
            trainingClassMock.TrainingProgramId.Should().Be(result.TrainingProgramId);
            trainingClassMock.StartTime.Should().Be(result.StartTime);
            trainingClassMock.EndTime.Should().Be(result.EndTime);
            trainingClassMock.TrainingClassAdmins.Should().BeEquivalentTo(result.Admins);
            trainingClassMock.TrainingClassTrainers.Should().BeEquivalentTo(result.Trainers);
            trainingClassMock.TrainingClassAttendee.Should().BeEquivalentTo(result.Attendees);
            trainingClassMock.Fsu.Should().BeEquivalentTo(result.Fsu);
            trainingClassMock.ReviewDate.Should().Be(result.ReviewDate);
            trainingClassMock.ReviewAuthor.Should().Be(result.ReviewAuthor);
            trainingClassMock.ApproveDate.Should().Be(result.ApproveDate);
            trainingClassMock.ApproveAuthor.Should().Be(result.ApproveAuthor);
            trainingClassMock.TrainingClassTimeFrame.StartDate.Should().Be(result.TimeFrame.StartDate);
            trainingClassMock.TrainingClassTimeFrame.EndDate.Should().Be(result.TimeFrame.EndDate);
            trainingClassMock.TrainingClassTimeFrame.HighlightedDates!
                .Select(x => x.HighlightedDate).ToList()
                .Should().BeEquivalentTo(result.TimeFrame.HighlightedDates);
            trainingClassMock.Branch.Should().Be(result.Branch);
            trainingClassMock.Attendee.Should().Be(result.Attendee);
        }

        [Fact]
        public void CreateTrainingClassDTO_Location()
        {
            //arrange
            var createDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<Location>(createDTOMock);
            var reverseResult = _mapperConfig.Map<CreateTrainingClassDTO>(result);

            //assert
            result.LocationName.Should().Be(createDTOMock.LocationName);
            result.LocationName.Should().Be(reverseResult.LocationName);
        }

        [Fact]
        public void CreateTrainingClassDTO2_CreateTrainingClassDTO()
        {
            //arrange
            var createDTO2Mock = _fixture.Build<CreateTrainingClassDTO2>().Create();

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO>(createDTO2Mock);

            //assert
            result.Name.Should().Be(createDTO2Mock.ClassName);
            result.Code.Should().Be(createDTO2Mock.ClassCode);
            result.StatusClassDetail.Should().Be(createDTO2Mock.ClassStatus);
            result.LocationName.Should().Be(createDTO2Mock.General.Location);
            result.TrainingProgramId.Should().Be(createDTO2Mock.TrainingPrograms.TrainingProgramId);
            result.StartTime.Should().Be(createDTO2Mock.General.ClassTime.StartTime);
            result.EndTime.Should().Be(createDTO2Mock.General.ClassTime.EndTime);
            result.Admins.Should().BeEquivalentTo(createDTO2Mock.General.Admins);
            result.Trainers.Should().BeEquivalentTo(createDTO2Mock.General.Trainers);
            result.TimeFrame.Should().BeEquivalentTo(createDTO2Mock.TimeFrame);
            result.Attendees.Should().BeEquivalentTo(createDTO2Mock.Attendees);
            result.Fsu.Should().BeEquivalentTo(createDTO2Mock.General.Fsu);
            result.ReviewDate.Should().Be(createDTO2Mock.General.Review!.Date);
            result.ReviewAuthor.Should().Be(createDTO2Mock.General.Review.Author);
            result.ApproveDate.Should().Be(createDTO2Mock.General.Approve!.Date);
            result.ApproveAuthor.Should().Be(createDTO2Mock.General.Approve.Author);
        }

        [Fact]
        public void CreateTrainingClassDTO2_CreateTrainingClassDTO_ReverseMap()
        {
            //arrange
            var createTrainingClassDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO2>(createTrainingClassDTOMock);

            //assert
            createTrainingClassDTOMock.Name.Should().Be(result.ClassName);
            createTrainingClassDTOMock.Code.Should().Be(result.ClassCode);
            createTrainingClassDTOMock.StatusClassDetail.Should().Be(result.ClassStatus);
            createTrainingClassDTOMock.LocationName.Should().Be(result.General.Location);
            createTrainingClassDTOMock.TrainingProgramId.Should().Be(result.TrainingPrograms.TrainingProgramId);
            createTrainingClassDTOMock.StartTime.Should().Be(result.General.ClassTime.StartTime);
            createTrainingClassDTOMock.EndTime.Should().Be(result.General.ClassTime.EndTime);
            createTrainingClassDTOMock.Admins.Should().BeEquivalentTo(result.General.Admins);
            createTrainingClassDTOMock.Trainers.Should().BeEquivalentTo(result.General.Trainers);
            createTrainingClassDTOMock.TimeFrame.Should().BeEquivalentTo(result.TimeFrame);
            createTrainingClassDTOMock.Attendees.Should().BeEquivalentTo(result.Attendees);
            createTrainingClassDTOMock.Fsu.Should().BeEquivalentTo(result.General.Fsu);
            createTrainingClassDTOMock.ReviewDate.Should().Be(result.General.Review!.Date);
            createTrainingClassDTOMock.ReviewAuthor.Should().Be(result.General.Review.Author);
            createTrainingClassDTOMock.ApproveDate.Should().Be(result.General.Approve!.Date);
            createTrainingClassDTOMock.ApproveAuthor.Should().Be(result.General.Approve.Author);
        }

        [Fact]
        public void TrainingProgramDTO_CreateTrainingClassDTO()
        {
            //arrange
            var trainingProgramDTOMock = _fixture.Build<TrainingProgramDTO>().Create();

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO>(trainingProgramDTOMock);
            var reverseResult = _mapperConfig.Map<TrainingProgramDTO>(result);

            //assert
            result.TrainingProgramId.Should().Be(trainingProgramDTOMock.TrainingProgramId);
            result.TrainingProgramId.Should().Be(reverseResult.TrainingProgramId);
        }

        [Fact]
        public void ClassTimeDTO_CreateTrainingClassDTO()
        {
            //arrange
            var classTimeDTOMock = _fixture.Build<ClassTimeDTO>().Create();

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO>(classTimeDTOMock);
            var reverseReult = _mapperConfig.Map<ClassTimeDTO>(result);

            //assert
            result.StartTime.Should().Be(classTimeDTOMock.StartTime);
            result.EndTime.Should().Be(classTimeDTOMock.EndTime);
            result.StartTime.Should().Be(reverseReult.StartTime);
            result.EndTime.Should().Be(reverseReult.EndTime);
        }

        [Fact]
        public void GeneralDTO_CreateTrainingClassDTO()
        {
            //arrange
            var generalDTOMock = _fixture.Build<GeneralDTO>()
                .OmitAutoProperties()
                .With(x => x.ClassTime)
                .With(x => x.Location)
                .Create();

            //act
            var result = _mapperConfig.Map<CreateTrainingClassDTO>(generalDTOMock);
            var reverseResult = _mapperConfig.Map<GeneralDTO>(result);

            //assert
            result.StartTime.Should().Be(generalDTOMock.ClassTime.StartTime);
            result.EndTime.Should().Be(generalDTOMock.ClassTime.EndTime);
            result.LocationName.Should().Be(generalDTOMock.Location);
            result.StartTime.Should().Be(reverseResult.ClassTime.StartTime);
            result.EndTime.Should().Be(reverseResult.ClassTime.EndTime);
            result.LocationName.Should().Be(reverseResult.Location);
        }

        [Fact]
        public void UpdateTrainingClassDTO_TrainingClass()
        {
            //arrange
            var updateDTOMock = _fixture.Build<UpdateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<TrainingClass>(updateDTOMock);

            //assert
            result.Name.Should().Be(updateDTOMock.Name);
            result.Code.Should().Be(updateDTOMock.Code);
            result.StatusClassDetail.Should().Be(updateDTOMock.StatusClassDetail);
            result.LocationID.Should().Be(updateDTOMock.LocationID);
            result.Location!.LocationName.Should().Be(updateDTOMock.LocationName);
            result.TrainingProgramId.Should().Be(updateDTOMock.TrainingProgramId);
            result.StartTime.Should().Be(updateDTOMock.StartTime);
            result.EndTime.Should().Be(updateDTOMock.EndTime);
            result.TrainingClassAdmins.Should().BeEquivalentTo(updateDTOMock.Admins);
            result.TrainingClassTrainers.Should().BeEquivalentTo(updateDTOMock.Trainers);
            result.TrainingClassAttendee.Should().BeEquivalentTo(updateDTOMock.Attendees);
            result.Fsu.Should().BeEquivalentTo(updateDTOMock.Fsu);
            result.ReviewDate.Should().Be(updateDTOMock.ReviewDate);
            result.ReviewAuthor.Should().Be(updateDTOMock.ReviewAuthor);
            result.ApproveDate.Should().Be(updateDTOMock.ApproveDate);
            result.ApproveAuthor.Should().Be(updateDTOMock.ApproveAuthor);
            result.TrainingClassTimeFrame.StartDate.Should().Be(updateDTOMock.TimeFrame.StartDate);
            result.TrainingClassTimeFrame.EndDate.Should().Be(updateDTOMock.TimeFrame.EndDate);
            result.TrainingClassTimeFrame.HighlightedDates!
                .Select(x => x.HighlightedDate).ToList()
                .Should().BeEquivalentTo(updateDTOMock.TimeFrame.HighlightedDates);
            result.Branch.Should().Be(updateDTOMock.Branch);
            result.Attendee.Should().Be(updateDTOMock.Attendee);
        }

        [Fact]
        public void UpdateTrainingClassDTO_TrainingClass_ReverseMap()
        {
            //arrange
            var createDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();
            var trainingClassMock = _mapperConfig.Map<TrainingClass>(createDTOMock);

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO>(trainingClassMock);

            //assert
            trainingClassMock.Name.Should().Be(result.Name);
            trainingClassMock.Code.Should().Be(result.Code);
            trainingClassMock.StatusClassDetail.Should().Be(result.StatusClassDetail);
            trainingClassMock.LocationID.Should().Be(result.LocationID);
            trainingClassMock.Location!.LocationName.Should().Be(result.LocationName);
            trainingClassMock.TrainingProgramId.Should().Be(result.TrainingProgramId);
            trainingClassMock.StartTime.Should().Be(result.StartTime);
            trainingClassMock.EndTime.Should().Be(result.EndTime);
            trainingClassMock.TrainingClassAdmins.Should().BeEquivalentTo(result.Admins);
            trainingClassMock.TrainingClassTrainers.Should().BeEquivalentTo(result.Trainers);
            trainingClassMock.TrainingClassAttendee.Should().BeEquivalentTo(result.Attendees);
            trainingClassMock.Fsu.Should().BeEquivalentTo(result.Fsu);
            trainingClassMock.ReviewDate.Should().Be(result.ReviewDate);
            trainingClassMock.ReviewAuthor.Should().Be(result.ReviewAuthor);
            trainingClassMock.ApproveDate.Should().Be(result.ApproveDate);
            trainingClassMock.ApproveAuthor.Should().Be(result.ApproveAuthor);
            trainingClassMock.TrainingClassTimeFrame.StartDate.Should().Be(result.TimeFrame.StartDate);
            trainingClassMock.TrainingClassTimeFrame.EndDate.Should().Be(result.TimeFrame.EndDate);
            trainingClassMock.TrainingClassTimeFrame.HighlightedDates!
                .Select(x => x.HighlightedDate).ToList()
                .Should().BeEquivalentTo(result.TimeFrame.HighlightedDates);
            trainingClassMock.Branch.Should().Be(result.Branch);
            trainingClassMock.Attendee.Should().Be(result.Attendee);
        }

        [Fact]
        public void UpdateTrainingClassDTO_Location()
        {
            //arrange
            var createDTOMock = _fixture.Build<UpdateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<Location>(createDTOMock);
            var reverseResult = _mapperConfig.Map<UpdateTrainingClassDTO>(result);

            //assert
            result.LocationName.Should().Be(createDTOMock.LocationName)
                .And.Be(reverseResult.LocationName);
        }

        [Fact]
        public void UpdateTrainingClassDTO2_UpdateTrainingClassDTO()
        {
            //arrange
            var createDTO2Mock = _fixture.Build<UpdateTrainingClassDTO2>().Create();

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO>(createDTO2Mock);

            //assert
            result.Name.Should().Be(createDTO2Mock.ClassName);
            result.Code.Should().Be(createDTO2Mock.ClassCode);
            result.StatusClassDetail.Should().Be(createDTO2Mock.ClassStatus);
            result.TrainingProgramId.Should().Be(createDTO2Mock.TrainingPrograms.TrainingProgramId);
            result.TimeFrame.Should().BeEquivalentTo(createDTO2Mock.TimeFrame);
            result.Attendees.Should().BeEquivalentTo(createDTO2Mock.Attendees);
            result.LocationName.Should().Be(createDTO2Mock.General.Location);
            result.Admins.Should().BeEquivalentTo(createDTO2Mock.General.Admins);
            result.Trainers.Should().BeEquivalentTo(createDTO2Mock.General.Trainers);
            result.Fsu.Should().BeEquivalentTo(createDTO2Mock.General.Fsu);
            result.StartTime.Should().Be(createDTO2Mock.General.ClassTime.StartTime);
            result.EndTime.Should().Be(createDTO2Mock.General.ClassTime.EndTime);
            result.ReviewDate.Should().Be(createDTO2Mock.General.Review!.Date);
            result.ReviewAuthor.Should().Be(createDTO2Mock.General.Review.Author);
            result.ApproveDate.Should().Be(createDTO2Mock.General.Approve!.Date);
            result.ApproveAuthor.Should().Be(createDTO2Mock.General.Approve.Author);
        }

        [Fact]
        public void UpdateTrainingClassDTO2_UpdateTrainingClassDTO_ReverseMap()
        {
            //arrange
            var createDTOMock = _fixture.Build<UpdateTrainingClassDTO>().Create();

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO2>(createDTOMock);

            //assert
            createDTOMock.Name.Should().Be(result.ClassName);
            createDTOMock.Code.Should().Be(result.ClassCode);
            createDTOMock.StatusClassDetail.Should().Be(result.ClassStatus);
            createDTOMock.TrainingProgramId.Should().Be(result.TrainingPrograms.TrainingProgramId);
            createDTOMock.TimeFrame.Should().BeEquivalentTo(result.TimeFrame);
            createDTOMock.Attendees.Should().BeEquivalentTo(result.Attendees);
            createDTOMock.LocationName.Should().Be(result.General.Location);
            createDTOMock.Admins.Should().BeEquivalentTo(result.General.Admins);
            createDTOMock.Trainers.Should().BeEquivalentTo(result.General.Trainers);
            createDTOMock.Fsu.Should().BeEquivalentTo(result.General.Fsu);
            createDTOMock.StartTime.Should().Be(result.General.ClassTime.StartTime);
            createDTOMock.EndTime.Should().Be(result.General.ClassTime.EndTime);
            createDTOMock.ReviewDate.Should().Be(result.General.Review!.Date);
            createDTOMock.ReviewAuthor.Should().Be(result.General.Review.Author);
            createDTOMock.ApproveDate.Should().Be(result.General.Approve!.Date);
            createDTOMock.ApproveAuthor.Should().Be(result.General.Approve.Author);
        }
        [Fact]
        public void TrainingProgramDTO_UpdateTrainingClassDTO()
        {
            //arrange
            var trainingProgramDTOMock = _fixture.Build<TrainingProgramDTO>().Create();

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO>(trainingProgramDTOMock);
            var reverseResult = _mapperConfig.Map<TrainingProgramDTO>(result);

            //assert
            result.TrainingProgramId.Should().Be(trainingProgramDTOMock.TrainingProgramId)
                .And.Be(reverseResult.TrainingProgramId);
        }

        [Fact]
        public void ClassTimeDTO_UpdateTrainingClassDTO()
        {
            //arrange
            var classTimeDTOMock = _fixture.Build<ClassTimeDTO>().Create();

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO>(classTimeDTOMock);
            var reverseResult = _mapperConfig.Map<ClassTimeDTO>(result);

            //assert
            result.StartTime.Should().Be(classTimeDTOMock.StartTime)
                .And.Be(reverseResult.StartTime);
            result.EndTime.Should().Be(classTimeDTOMock.EndTime)
                .And.Be(reverseResult.EndTime);
        }

        [Fact]
        public void GeneralDTO_UpdateTrainingClassDTO()
        {
            //arrange
            var generalDTOMock = _fixture.Build<GeneralDTO>()
                .OmitAutoProperties()
                .With(x => x.ClassTime)
                .With(x => x.Location)
                .Create();

            //act
            var result = _mapperConfig.Map<UpdateTrainingClassDTO>(generalDTOMock);
            var reverseResult = _mapperConfig.Map<GeneralDTO>(result);
            //assert
            result.StartTime.Should().Be(generalDTOMock.ClassTime.StartTime)
                .And.Be(reverseResult.ClassTime.StartTime);
            result.EndTime.Should().Be(generalDTOMock.ClassTime.EndTime)
                .And.Be(reverseResult.ClassTime.EndTime);
            result.LocationName.Should().Be(generalDTOMock.Location)
                .And.Be(reverseResult.Location);
        }

        [Fact]
        public void TrainingClass_TrainingClassViewModel()
        {
            //arrange
            var createDTOMock = _fixture.Build<CreateTrainingClassDTO>().Create();
            var trainingClassMock = _mapperConfig.Map<TrainingClass>(createDTOMock);

            //act
            var result = _mapperConfig.Map<TrainingClassViewModel>(trainingClassMock);

            //assert
            result._Id.Should().BeEquivalentTo(trainingClassMock.Id.ToString());
            result.StartTime.Should().Be(trainingClassMock.StartTime);
            result.EndTime.Should().Be(trainingClassMock.EndTime);
            result.LocationID.Should().Be(trainingClassMock.LocationID.ToString());
            Assert.Equivalent(result.Admins, trainingClassMock.TrainingClassAdmins);
            Assert.Equivalent(result.Trainers, trainingClassMock.TrainingClassTrainers);
            Assert.Equivalent(result.Attendees, trainingClassMock.TrainingClassAttendee);
            result.LocationName.Should().Be(trainingClassMock.Location!.LocationName);
            result.TimeFrame.StartDate.Should().Be(trainingClassMock.TrainingClassTimeFrame.StartDate);
            result.TimeFrame.EndDate.Should().Be(trainingClassMock.TrainingClassTimeFrame.EndDate);
            result.TimeFrame.HighlightedDates
                .Should()
                .BeEquivalentTo(trainingClassMock.TrainingClassTimeFrame.HighlightedDates!
                .Select(x => x.HighlightedDate).ToList());
        }

        [Fact]
        public void Location_TrainingClassViewModel()
        {
            //arrange
            var locationMock = _fixture.Build<Location>()
                .OmitAutoProperties()
                .With(x => x.LocationName)
                .Create();

            //act
            var result = _mapperConfig.Map<TrainingClassViewModel>(locationMock);

            //assert
            result.LocationName.Should().Be(locationMock.LocationName);
        }

        [Fact]
        public void AdminsDTO_TrainingClassAdmin()
        {
            //arrange
            var adminDTOMock = _fixture.Build<AdminsDTO>().Create();
            var trainingClassAdminMock = _fixture.Build<TrainingClassAdmin>()
                .With(x => x.TrainingClass, new TrainingClass()).Create();
            //act
            var result = _mapperConfig.Map<TrainingClassAdmin>(adminDTOMock);
            var reverseResult = _mapperConfig.Map<AdminsDTO>(trainingClassAdminMock);

            //assert
            result.AdminId.Should().Be(adminDTOMock.AdminId);
            reverseResult.AdminId.Should().Be(trainingClassAdminMock.AdminId);
        }

        [Fact]
        public void TrainerDTO_TrainingClassTrainer()
        {
            //arrange
            var trainerDTOMock = _fixture.Build<TrainerDTO>().Create();
            var trainingClassTrainerMock = _fixture.Build<TrainingClassTrainer>()
                .With(x => x.TrainingClass, new TrainingClass()).Create();

            //act
            var result = _mapperConfig.Map<TrainingClassTrainer>(trainerDTOMock);
            var reverseResult = _mapperConfig.Map<TrainerDTO>(trainingClassTrainerMock);

            //assert
            result.TrainerId.Should().Be(trainerDTOMock.TrainerId);
            reverseResult.TrainerId.Should().Be(trainingClassTrainerMock.TrainerId);
        }

        [Fact]
        public void TimeFrameDTO_TrainingClassTimeFrame()
        {
            //arrange
            var timeframeDTOMock = _fixture.Build<TimeFrameDTO>().Create();
            var trainingClassTimeFrameMock = _fixture.Build<TrainingClassTimeFrame>()
                .With(x => x.TrainingClass, new TrainingClass())
                .Create();
            //act
            var result = _mapperConfig.Map<TrainingClassTimeFrame>(timeframeDTOMock);
            var reverseResult = _mapperConfig.Map<TimeFrameDTO>(trainingClassTimeFrameMock);
            //assert
            result.StartDate.Should().Be(timeframeDTOMock.StartDate);
            result.EndDate.Should().Be(timeframeDTOMock.EndDate);
            result.HighlightedDates!.Select(x => x.HighlightedDate).ToList()
                .Should()
                .BeEquivalentTo(timeframeDTOMock.HighlightedDates);
            reverseResult.StartDate.Should().Be(trainingClassTimeFrameMock.StartDate);
            reverseResult.EndDate.Should().Be(trainingClassTimeFrameMock.EndDate);
            reverseResult.HighlightedDates
                .Should()
                .BeEquivalentTo(
                trainingClassTimeFrameMock.HighlightedDates!
                .Select(x => x.HighlightedDate)
                .ToList()
                );
        }

        [Fact]
        public void DateTime_HighlightedDates()
        {
            //arrange
            var dateMock = new DateTime();

            //act
            var result = _mapperConfig.Map<HighlightedDates>(dateMock);

            //assert
            result.HighlightedDate.Should().Be(dateMock);
        }

        [Fact]
        public void HighlightedDates_DateTime()
        {
            //arrange
            var highlightDatesMock = _fixture.Build<HighlightedDates>()
                .OmitAutoProperties()
                .With(x => x.HighlightedDate)
                .With(x => x.TrainingClassTimeFrame, new TrainingClassTimeFrame())
                .Create();

            //act
            var result = _mapperConfig.Map<DateTime>(highlightDatesMock);

            //assert
            result.Should().Be(highlightDatesMock.HighlightedDate);
        }

        [Fact]
        public void AttendeesDTO_TrainingClassAttendees()
        {
            //arrange
            var attendeesDTOMock = _fixture.Build<AttendeesDTO>().Create();
            var trainingClassAttendeesMock = _fixture.Build<TrainingClassAttendees>()
                .With(x => x.TrainingClass, new TrainingClass())
                .Create();
            //act
            var result = _mapperConfig.Map<TrainingClassAttendees>(attendeesDTOMock);
            var reverseResult = _mapperConfig.Map<AttendeesDTO>(trainingClassAttendeesMock);

            //assert
            Assert.Equivalent(attendeesDTOMock, result);
            Assert.Equivalent(reverseResult, trainingClassAttendeesMock);
        }

        [Fact]
        public void CreateLocationDTO_Location()
        {
            //arrange 
            var createLocationMock = _fixture.Build<CreateLocationDTO>().Create();

            //act
            var result = _mapperConfig.Map<Location>(createLocationMock);

            //assert
            Assert.Equivalent(createLocationMock, result);
        }

        [Fact]
        public void Location_LocationDTO()
        {
            //arrange
            var locationMock = _fixture.Build<Location>()
                .OmitAutoProperties()
                .With(x => x.LocationName)
                .Create();

            //act
            var result = _mapperConfig.Map<LocationDTO>(locationMock);

            //assert
            result.LocationName.Should().Be(locationMock.LocationName);
            result._Id.Should().Be(locationMock.Id.ToString());
        }
    }
}
