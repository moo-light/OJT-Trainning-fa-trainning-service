using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class UserRepositoryTests : SetupTest
    {
        private readonly IUserRepository _userRepository;
        public UserRepositoryTests()
        {
            _userRepository = new UserRepository
             (
                _dbContext,
                _currentTimeMock.Object,
                _claimsServiceMock.Object
             );
        }

        [Fact]
        public async Task CheckEmailExistedAsync_ReturnTrue()
        {
            var userMock = _fixture.Build<User>().Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Without(x => x.SubmitQuizzes).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();

            var result = await _userRepository.CheckEmailExistedAsync(userMock.Email);

            result.Should().Be(true);

        }

        [Fact]
        public async Task CheckEmailExistedAsync_ReturnFalse()
        {
            var userMock = _fixture.Build<User>().Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Without(x => x.SubmitQuizzes).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();

            var result = await _userRepository.CheckEmailExistedAsync(_fixture.Create<string>());

            result.Should().Be(false);

        }


        [Fact]
        public async Task CheckUserNameExistedAsync_ReturnTrue()
        {
            var userMock = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Role).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();

            var result = await _userRepository.CheckUserNameExistedAsync(userMock.UserName);

            result.Should().Be(true);

        }

        [Fact]
        public async Task CheckUserNameExistedAsync_ReturnFalse()
        {
            var userMock = _fixture.Build<User>()
                 .Without(x => x.Applications)
                 .Without(x => x.Syllabuses)
                 .Without(x => x.Feedbacks)
                 .Without(x => x.Attendances)
                 .Without(x => x.DetailTrainingClassParticipate)
                 .Without(x => x.SubmitQuizzes)
                 .Without(x => x.Role).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();

            var result = await _userRepository.CheckUserNameExistedAsync(_fixture.Create<string>());

            result.Should().Be(false);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnCorrectData()
        {
            //Arrange
            var userMock = _fixture.Build<User>()
                 .Without(x => x.Applications)
                 .Without(x => x.Syllabuses)
                 .Without(x => x.Feedbacks)
                 .Without(x => x.Attendances)
                 .Without(x => x.DetailTrainingClassParticipate)
                 .Without(x => x.SubmitQuizzes)
                 .Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();
            //Act
            var result = await _userRepository.GetUserByEmailAsync(userMock.Email);

            //Assert
            result.Id.Should().Be(userMock.Id);
        }
        [Fact]
        public async Task GetUserByEmailAsync_ThrowExcpetion()
        {
            //Arrange
            var userMock = _fixture.Build<User>()
                  .Without(x => x.Applications)
                  .Without(x => x.Syllabuses)
                  .Without(x => x.Feedbacks)
                  .Without(x => x.Attendances)
                  .Without(x => x.DetailTrainingClassParticipate)
                  .Without(x => x.SubmitQuizzes)
                  .Without(x => x.Role).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();
            //Act
            Func<Task> act = async () => await _userRepository.GetUserByEmailAsync(_fixture.Create<string>());

            //Assert
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task ChangeUserPasswordAsync_ShouldReturnCorrectData()
        {
            //Arrange
            var userMock = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Role).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();
            string newPassword = "string1";

            _unitOfWorkMock.Setup(u => u.UserRepository.Update(userMock)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
            //Act
            var result = await _userRepository.ChangeUserPasswordAsync(userMock, newPassword);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserByUserNameAsync_ReturnTrue()
        {
            //Arrange
            var userMock = _fixture.Build<User>()
                  .Without(x => x.Applications)
                  .Without(x => x.Syllabuses)
                  .Without(x => x.Feedbacks)
                  .Without(x => x.Attendances)
                  .Without(x => x.DetailTrainingClassParticipate)
                  .Without(x => x.SubmitQuizzes)
                  .Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();
            //Act
            var result = await _userRepository.GetUserByUserNameAsync(userMock.UserName);

            //Assert
            result.Id.Should().Be(userMock.Id);
        }

        [Fact]
        public async Task GetUserByUserNameAsync_ThrowExcpetion_WhenUserNull()
        {
            //Arrange
            var userMock = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .Without(x => x.Role).Create();
            await _dbContext.Users.AddAsync(userMock);
            await _dbContext.SaveChangesAsync();
            //Act
            Func<Task> act = async () => await _userRepository.GetUserByUserNameAsync(_fixture.Create<string>());

            //Assert
            act.Should().ThrowAsync<Exception>();
        }

    }
}
