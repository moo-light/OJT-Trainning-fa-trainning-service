using Application.Commons;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Application.ViewModels.TokenModels;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.Tests.Services
{
    public class UserServiceTest : SetupTest
    {
        private readonly IUserService _userService;
        public UserServiceTest()
        {
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            _userService = new UserService(_unitOfWorkMock.Object,
                _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
                _mockConfig.Object, cache, _sendMailMock.Object);
        }

        [Fact]
        public async Task SendResetPasswordTest_ShouldReturnString()
        {
            // Setup
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(mockUser.Email)).ReturnsAsync(mockUser);
            _sendMailMock.Setup(sm => sm.SendMailAsync(mockUser.Email, "ResetPassword", It.IsAny<string>())).ReturnsAsync(true);
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            // Act
            IUserService newUserService = new UserService(_unitOfWorkMock.Object,
                _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
                _mockConfig.Object, cache, _sendMailMock.Object);

            var result = await newUserService.SendResetPassword(mockUser.Email);
            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task SendResetPassword_ShouldThrowException()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
           .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            User mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _unitOfWorkMock.Setup(um => um.UserRepository.GetUserByEmailAsync(mockUser.Email)).ReturnsAsync(mockUser = null);
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            // Act
            IUserService newUserService = new UserService(_unitOfWorkMock.Object, _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object, _mockConfig.Object, cache, _sendMailMock.Object);

            Func<Task> act = async () => await newUserService.SendResetPassword(mockUser.Email);
            await act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task AddUserManual_ShouldReturnTrue()
        {
            //Arrange
            var mockData = _fixture.Build<AddUserManually>().Create();
            mockData.Pass = mockData.Pass.Hash();
            var user = _mapperConfig.Map<User>(mockData);
            _unitOfWorkMock.Setup(x => x.UserRepository.AddAsync(user)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            //Act
            var result = await _userService.AddUserManualAsync(mockData);
            //Assert
            result.Email.Should().Be(mockData.Email);

        }

        [Fact]
        public async Task AddUserManual_ShouldReturnNull()
        {
            var mockData = _fixture.Build<AddUserManually>().Create();
            var user = _mapperConfig.Map<User>(mockData);
            _unitOfWorkMock.Setup(x => x.UserRepository.AddAsync(user)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);
            var result = await _userService.AddUserManualAsync(mockData);
            result.Should().BeNull();
        }

        [Fact]
        public async Task SendResetPassword_ShouldReturnEmptyString()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(mockUser.Email)).ReturnsAsync(mockUser);
            _sendMailMock.Setup(sm => sm.SendMailAsync(mockUser.Email, "ResetPassword", It.IsAny<string>())).ReturnsAsync(false);
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            IUserService newUserService = new UserService(_unitOfWorkMock.Object,
                _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
                _mockConfig.Object, cache, _sendMailMock.Object);

            var result = await newUserService.SendResetPassword(mockUser.Email);
            // Assert
            result.Should().BeNullOrEmpty();
        }
        [Fact]
        public async Task ResetPassword_ShouldReturnTrue()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
           .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            // Setup
            var cache = new MemoryCache(new MemoryCacheOptions());
            var ResetPassDTO = _fixture.Build<ResetPasswordDTO>().Create();
            var user = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            string email = "MockEMail@gmail.com";
            cache.Set(ResetPassDTO.Code, email);
            ResetPassDTO.ConfirmPassword = ResetPassDTO.NewPassword;
            _unitOfWorkMock.Setup(um => um.UserRepository.GetUserByEmailAsync(email)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(um => um.UserRepository.Update(user)).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            IUserService newUserService = new UserService(_unitOfWorkMock.Object,
              _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
              _mockConfig.Object, cache, _sendMailMock.Object);
            var result = await newUserService.ResetPassword(ResetPassDTO);
            result.Should().BeTrue();
        }




        [Fact]
        public async Task ResetPassword_ShouldReturnFalse()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
          .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var cache = new MemoryCache(new MemoryCacheOptions());
            var ResetPassDTO = _fixture.Build<ResetPasswordDTO>().Create();
            var user = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            string email = "MockEMail@gmail.com";
            cache.Set("ABSDSE", email);
            Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
            IUserService newUserService = new UserService(_unitOfWorkMock.Object,
            _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
            _mockConfig.Object, cache, _sendMailMock.Object);

            Func<Task> act = async () => await newUserService.ResetPassword(ResetPassDTO);
            await act.Should().ThrowAsync<Exception>();
        }
        /*        [Fact]
                public async Task ResetPassword_ConfirmFalse_ShouldThrowException()
                {
                    _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                   .ForEach(b => _fixture.Behaviors.Remove(b));
                    _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
                    // Setup
                    var cache = new MemoryCache(new MemoryCacheOptions());
                    var ResetPassDTO = _fixture.Build<ResetPasswordDTO>().Create();
                    var user = _fixture.Build<User>().Create();
                    string email = "MockEMail@gmail.com";
                    cache.Set(ResetPassDTO.Code, email);
                    Mock<IConfiguration> _mockConfig = new Mock<IConfiguration>();
                    IUserService newUserService = new UserService(_unitOfWorkMock.Object,
                      _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object,
                      _mockConfig.Object, cache, _sendMailMock.Object);
                    var result = await newUserService.ResetPassword(ResetPassDTO);
                    Func<Task> act = async () => await newUserService.ResetPassword(ResetPassDTO);
                    act.Should().ThrowAsync<Exception>();

                }*/


        [Fact]
        public async Task UpdateUser_ShouldReturnTrue()
        {
            var mockData = _fixture.Create<UpdateDTO>();
            _unitOfWorkMock.Setup(um => um.UserRepository.Update(It.IsAny<User>())).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            var result = await _userService.UpdateUserInformation(mockData);
            Assert.True(result);

        }


        [Fact]
        public async Task UpdateUser_ShouldReturnFalse()
        {
            var mockData = _fixture.Create<UpdateDTO>();
            _unitOfWorkMock.Setup(um => um.UserRepository.Update(It.IsAny<User>())).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(-1);
            var result = await _userService.UpdateUserInformation(mockData);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateUser_ShouldThrowException()
        {
            UpdateDTO mockData = null;
            //Get Return Exception
            Func<Task> act = async () => await _userService.UpdateUserInformation(mockData);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task Register_ShouldReturnUser()
        {
            var listUser = new List<User>();
            var mockData = _fixture.Build<RegisterDTO>().Create();
            _unitOfWorkMock.Setup(um => um.UserRepository.FindAsync(u => u.UserName == mockData.UserName || u.Email == mockData.Email)).ReturnsAsync(listUser);
            var addMockData = _mapperConfig.Map<User>(mockData);
            _unitOfWorkMock.Setup(um => um.UserRepository.AddAsync(addMockData)).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            var result = await _userService.RegisterAsync(mockData);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Register_ShouldThrowException()
        {
            var listUser = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks).CreateMany(1).ToList();
            var mockData = _fixture.Build<RegisterDTO>().Create();
            _unitOfWorkMock.Setup(um => um.UserRepository.FindAsync(u => u.UserName == mockData.UserName || u.Email == mockData.Email)).ReturnsAsync(listUser);
            Func<Task> act =  () =>  _userService.RegisterAsync(mockData);
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task LoginAsync_ReturnCorrectData()
        {
            //Arange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var loginDTO = new LoginDTO { UserName = mockUser.UserName, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            mockUser.ExpireTokenTime = DateTime.UtcNow.AddDays(1);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUserNameAsync(mockUser.UserName)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
            _appConfigurationMock.SetupAllProperties();
            _appConfigurationMock.Object.JWTSecretKey = "ChanquaChanquaChanquaChanqua";
            //Act
            var result = await _userService.LoginAsync(loginDTO);
            //Assert

            result.Should().NotBeNull();
        }
        [Fact]
        public async Task LoginAsync_ThrowException_WhenPasswordIncorrect()
        {
            //Arange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var loginDTO = new LoginDTO { UserName = mockUser.UserName, Password = _fixture.Create<string>() };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            mockUser.ExpireTokenTime = DateTime.UtcNow.AddDays(1);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(mockUser.Email)).ReturnsAsync(mockUser);

            //Act
            Func<Task> act =  () =>  _userService.LoginAsync(loginDTO);
            //Assert

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task RefreshToken_ReturnCorrectData()
        {
            //Arange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var loginDTO = new LoginDTO { UserName = mockUser.UserName, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            mockUser.ExpireTokenTime = DateTime.UtcNow.AddDays(1);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUserNameAsync(mockUser.UserName)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
            _appConfigurationMock.SetupAllProperties();
            _appConfigurationMock.Object.JWTSecretKey = "ChanquaChanquaChanquaChanqua";
            var token = await _userService.LoginAsync(loginDTO);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetByIdAsync(mockUser.Id, x => x.Role)).ReturnsAsync(mockUser);
            //Assert

            var result = await _userService.RefreshToken(token.AccessToken, token.RefreshToken);

            //Assert
            result.Should().NotBeNull();
        }
        [Fact]
        public async Task RefreshToken_ReturnNull_WhenAccessTokenOrRefreshTokenIsNull()
        {

            //Assert

            var result = await _userService.RefreshToken(string.Empty, string.Empty);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshToken_ReturnNull_WhenGetPrincipalThowException()
        {
            //Arrange
            var token = _fixture.Create<Token>();
            //Assert

            Func<Task> act = async () => await _userService.RefreshToken(token.AccessToken, token.RefreshToken);

            //Assert
            await act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task RefreshToken_ReturnNull_WhenRefreshTokenInCorrect()
        {
            // Arange
            var tokenRefresh = _fixture.Create<string>();
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var loginDTO = new LoginDTO { UserName = mockUser.UserName, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            mockUser.ExpireTokenTime = DateTime.UtcNow.AddDays(1);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUserNameAsync(mockUser.UserName)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
            _appConfigurationMock.SetupAllProperties();
            _appConfigurationMock.Object.JWTSecretKey = "ChanquaChanquaChanquaChanqua";
            var token = await _userService.LoginAsync(loginDTO);
            //Assert

            var result = await _userService.RefreshToken(token.AccessToken, tokenRefresh);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RefreshToken_ReturnNull_WhenExpireTimeInCorrect()
        {
            // Arange
            var tokenRefresh = _fixture.Create<string>();
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var loginDTO = new LoginDTO { UserName = mockUser.UserName, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            mockUser.ExpireTokenTime = DateTime.Now;
            _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByUserNameAsync(mockUser.UserName)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);
            _appConfigurationMock.SetupAllProperties();
            _appConfigurationMock.Object.JWTSecretKey = "ChanquaChanquaChanquaChanqua";
            var token = await _userService.LoginAsync(loginDTO);
            //Assert

            var result = await _userService.RefreshToken(token.AccessToken, token.RefreshToken);

            //Assert
            result.Should().BeNull();
        }
        [Fact]
        public async void GetAllAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockUsers = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).CreateMany().ToList();
            var expected = _mapperConfig.Map<List<UserViewModel>>(mockUsers);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);

            //act
            var result = await _userService.GetAllAsync();

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetAllAsync(), Times.Once);
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void GetUserPaginationAsync_ShouldReturnCorrectData_WhenPassTheParameter()
        {
            //arrange
            var mockData = new Pagination<User>
            {
                Items = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).CreateMany().ToList(),
                PageIndex = 0,
                PageSize = 100,
                TotalItemsCount = 100,
            };
            var expected = _mapperConfig.Map<Pagination<UserViewModel>>(mockData);

            _unitOfWorkMock.Setup(x => x.UserRepository.ToPagination(0, 10)).ReturnsAsync(mockData);

            //act
            var result = await _userService.GetUserPaginationAsync(0, 10);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.ToPagination(0, 10), Times.Once());
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void DisableUser_ShouldReturnTrue()
        {
            //arrange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var userViewModel = _mapperConfig.Map<UserViewModel>(mockUser);
            var userId = userViewModel._Id;
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockUser.Id)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(x => x.UserRepository.SoftRemove(It.IsAny<User>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            //act
            var result = await _userService.DisableUserById(userId);
            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(mockUser.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.SoftRemove(mockUser), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
            Assert.IsType<bool>(result);
            result.Should().BeTrue();
        }
        [Fact]
        public async void DisableUser_ShouldReturnFalse_WhenSaveChangesFailed()
        {
            //arrange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var userViewModel = _mapperConfig.Map<UserViewModel>(mockUser);
            var userId = userViewModel._Id;
            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockUser.Id)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(x => x.UserRepository.SoftRemove(It.IsAny<User>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(-1);

            //act
            var result = await _userService.DisableUserById(userId);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(mockUser.Id), Times.Once);
            _unitOfWorkMock.Verify(x => x.UserRepository.SoftRemove(mockUser), Times.AtMostOnce);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.AtMostOnce);
            Assert.IsType<bool>(result);
            result.Should().BeFalse();
        }
        [Fact]
        public async void DisableUser_ShouldThrowException_WhenUsingNonexistedId()
        {
            //arrange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var userViewModel = _mapperConfig.Map<UserViewModel>(mockUser);
            var userId = "";
            _unitOfWorkMock.Setup(x => x.UserRepository.SoftRemove(It.IsAny<User>())).Verifiable();

            //act
            Func<Task> act = async () => await _userService.DisableUserById(userId);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.SoftRemove(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Never);
            await act.Should().ThrowAsync<AutoMapperMappingException>();
        }
        [Fact]
        public async void GetUserByIdAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var expected = _mapperConfig.Map<UserViewModel>(mockUser);

            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockUser.Id)).ReturnsAsync(mockUser);

            //act
            var result = await _userService.GetUserByIdAsync(expected._Id);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(mockUser.Id), Times.Once());
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task UserService_ChangePasswordAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            mockData.PasswordHash = "string".Hash();

            var newPassword = "string1";
            _unitOfWorkMock.Setup(service => service.UserRepository.GetAuthorizedUserAsync()).ReturnsAsync(mockData);

            _unitOfWorkMock.Setup(s => s.UserRepository.ChangeUserPasswordAsync(mockData, newPassword)).ReturnsAsync(true);
            var result = await _userService.ChangePasswordAsync("string", newPassword);

            result.Should().Be("Update Success!");

        }
        [Fact]
        public async void GetUserByIdAsync_ShouldThrowException_WhenPassWrongFormat()
        {
            //arrange
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            var incorrectFormatId = "abc123";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetByIdAsync(mockUser.Id)).ReturnsAsync(mockUser);

            //act
            Func<Task> act = async () => await _userService.GetUserByIdAsync(incorrectFormatId);

            //assert
            _unitOfWorkMock.Verify(x => x.UserRepository.GetByIdAsync(mockUser.Id), Times.Never);
            await act.Should().ThrowAsync<AutoMapperMappingException>();
        }

        [Fact]
        public async void LogoutAsync_ReturnTrue()
        {
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAuthorizedUserAsync()).ReturnsAsync(mockUser);

            mockUser.RefreshToken = null;
            mockUser.ExpireTokenTime = null;
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(1);

            //Act
            var result = await _userService.LogoutAsync();
            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void LogoutAsync_ReturnFalse_WhenUserIsNull()
        {
            User mockUser = null;
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAuthorizedUserAsync()).ReturnsAsync(mockUser);

            //Act
            var result = await _userService.LogoutAsync();
            //Assert
            result.Should().BeFalse();
        }
        [Fact]
        public async void LogoutAsync_ThrowExcpetion_WhenSaveChangeFalse()
        {
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAuthorizedUserAsync()).ReturnsAsync(mockUser);

            mockUser.RefreshToken = null;
            mockUser.ExpireTokenTime = null;
            _unitOfWorkMock.Setup(u => u.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(u => u.SaveChangeAsync()).ReturnsAsync(0);

            //Act
            Func<Task> act = async () => await _userService.LogoutAsync();
            //Assert
            await act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnAllUser()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter(null, null, null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFilter_WithGenderFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter(null, "Female", null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFilter_WithRoleFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter(null, null, 1, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFilter_WithLevelFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAllAsync()).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter(null, null, null, "SiuCapVjpPr0");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFullname_WithValidString()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.FindAsync
                (u =>
                    u.FullName!.Contains("Gicungdc")
                )).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter("Gicungdc", null, null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFullnameAndFilter_WithValidStringAndGenderFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.FindAsync
                (u =>
                    u.FullName!.Contains("ten")
                )).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter("ten", "Female", null, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFullnameAndFilter_WithValidStringAndRoleAndGenderAndLevelFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.FindAsync
                (u =>
                    u.FullName!.Contains("ten")
                )).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter("ten", "Female", 1, "SiuCapVjpPr0");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFullnameAndFilter_WithValidStringAndRoleFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.FindAsync
                (u =>
                    u.FullName!.Contains("ten")
                )).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter("ten", null, 1, null);
            //Assert
            result.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public async void SearchUserWithFilter_ShouldReturnListOfMatchUserFullnameAndFilter_WithValidStringAndLevelFilter()
        {
            //Arrange
            var mockUsers = _fixture.Build<User>().Without(u => u.Id).Without(u => u.SubmitQuizzes)
                                                  .Without(u => u.Id).Without(u => u.CreationDate)
                                                  .Without(u => u.CreatedBy).Without(u => u.ModificationDate)
                                                  .Without(u => u.ModificationBy).Without(u => u.DeletionDate)
                                                  .Without(u => u.DeleteBy).Without(u => u.UserName)
                                                  .Without(u => u.PasswordHash).Without(u => u.Email).Without(u => u.DateOfBirth)
                                                  .Without(u => u.AvatarUrl).Without(u => u.RefreshToken)
                                                  .Without(u => u.ExpireTokenTime).Without(u => u.LoginDate)
                                                  .Without(u => u.Role).Without(u => u.Applications)
                                                  .Without(u => u.Attendances).Without(u => u.Syllabuses)
                                                  .Without(u => u.DetailTrainingClassParticipate).Without(u => u.Feedbacks)
                                                  .With(u => u.IsDeleted, false)
                                                  .With(u => u.FullName, "tenngdung").With(u => u.RoleId, 1)
                                                  .With(u => u.Gender, "Female").With(u => u.Level, "SiuCapVjpPr0")
                                                  .CreateMany(3).ToList();
            var expected = _mapperConfig.Map<List<SearchAndFilterUserViewModel>>(mockUsers);
            foreach (var user in mockUsers)
            {
                foreach (var uf in expected)
                    if (uf.Id.Equals(user.Id))
                        uf.Type = ((RoleEnums)user.RoleId).ToString();
            }
            _unitOfWorkMock.Setup(x => x.UserRepository.FindAsync
                (u =>
                    u.FullName!.Contains("ten")
                )).ReturnsAsync(mockUsers);
            //Act
            var result = await _userService.SearchUsersWithFilter("ten", null, null, "SiuCapVjpPr0");
            //Assert
            result.Should().BeEquivalentTo(expected);
        }

    }
}