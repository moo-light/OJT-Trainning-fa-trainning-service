using Application.Commons;
using Application.Utils;
using Application.ViewModels.TokenModels;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers;

public class UserControllerTest : SetupTest
{
    private readonly UsersController _userController;
    private readonly Mock<IExternalAuthUtils> _externalAuthUtilsMock;

    public UserControllerTest()
    {
        _externalAuthUtilsMock = new Mock<IExternalAuthUtils>();
        _userController = new UsersController(_userServiceMock.Object,
                                              _claimsServiceMock.Object,
                                              _externalAuthUtilsMock.Object,
                                              _mapperMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ReturnOKObjectResult()
    {
        //Arrange
        var registerDTO = _fixture.Create<RegisterDTO>();
        _userServiceMock.Setup(u => u.RegisterAsync(registerDTO)).Returns(Task.FromResult(true));
        //Act

        var result = await _userController.RegisterAsync(registerDTO);
        //Assert
        result.Should().BeOfType<OkObjectResult>();

    }
    [Fact]
    public async Task RegisterAsync_ReturnBadRequestObjectResult()
    {
        //Arrange
        var registerDTO = _fixture.Create<RegisterDTO>();
        _userServiceMock.Setup(u => u.RegisterAsync(registerDTO)).Returns(Task.FromResult(false));
        //Act

        var result = await _userController.RegisterAsync(registerDTO);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();

    }

    [Fact]
    public async Task LoginAsync_ReturnOKObjectResult()
    {
        //Arrange
        var loginDTo = _fixture.Create<LoginDTO>();
        var token = _fixture.Create<Token>();
        _userServiceMock.Setup(u => u.LoginAsync(loginDTo)).ReturnsAsync(token);
        //Act

        var result = await _userController.LoginAsync(loginDTo);
        //Assert
        result.Should().BeOfType<OkObjectResult>();

    }

    [Fact]
    public async Task LoginAsync_ReturnBadRequestResult()
    {
        //Arrange
        var loginDTo = _fixture.Create<LoginDTO>();
        var token = _fixture.Create<Token>();
        _userServiceMock.Setup(u => u.LoginAsync(loginDTo)).ReturnsAsync(token = null);
        //Act

        var result = await _userController.LoginAsync(loginDTo);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();

    }

    [Fact]
    public async Task RefreshToken_ReturnOKObjectResult()
    {
        //Arrange
        var token = _fixture.Create<Token>();
        _userServiceMock.Setup(u => u.RefreshToken(token.AccessToken, token.RefreshToken)).ReturnsAsync(token);
        //Act

        var result = await _userController.RefreshToken(token.AccessToken, token.RefreshToken);
        //Assert
        result.Should().BeOfType<OkObjectResult>();

    }
    [Fact]
    public async Task RefreshToken_ReturnBadRequestResult()
    {
        //Arrange
        var token = _fixture.Create<Token>();
        Token newToken = null;
        _userServiceMock.Setup(u => u.RefreshToken(token.AccessToken, token.RefreshToken)).ReturnsAsync(newToken);
        //Act

        var result = await _userController.RefreshToken(token.AccessToken, token.RefreshToken);
        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateUser_ReturnNoContentResult()
    {
        //Arrange
        var updateDTO = _fixture.Create<UpdateDTO>();
        _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(updateDTO.UserId);
        _userServiceMock.Setup(u => u.UpdateUserInformation(updateDTO)).ReturnsAsync(true);
        //Act

        var result = await _userController.UpdateUser(updateDTO);
        //Assert
        result.Should().BeOfType<NoContentResult>();

    }

    [Fact]
    public async Task UpdateUser_ReturnBadRequestResult()
    {
        //Arrange
        var updateDTO = _fixture.Create<UpdateDTO>();
        _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(updateDTO.UserId);
        _userServiceMock.Setup(u => u.UpdateUserInformation(updateDTO)).ReturnsAsync(false);
        //Act

        var result = await _userController.UpdateUser(updateDTO);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();

    }
    [Fact]
    public async Task ForgotPassword_ReturnNoContentResult()
    {
        //Arrange
        var email = _fixture.Create<string>();
        _userServiceMock.Setup(u => u.SendResetPassword(email)).ReturnsAsync(email);
        //Act
        var result = await _userController.ForgotPassword(email);
        //Assert
        result.Should().BeOfType<OkResult>();
    }
    [Fact]
    public async Task ForgotPassword_ReturnBadRequestResult()
    {
        //Arrange
        var email = _fixture.Create<string>();
        //Act
        var result = await _userController.ForgotPassword(email);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();

    }
    [Fact]
    public async Task ResetPassword_ReturnNoContentResult()
    {
        //Arrange
        var email = _fixture.Create<ResetPasswordDTO>();
        _userServiceMock.Setup(u => u.ResetPassword(email)).ReturnsAsync(true);
        //Act
        var result = await _userController.ResetPassword(email);
        //Assert
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(true);
    }
    [Fact]
    public async Task ResetPassword_ReturnBadRequestResult()
    {
        //Arrange
        var email = _fixture.Create<ResetPasswordDTO>();
        //Act
        var result = await _userController.ResetPassword(email);
        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    [Fact]
    public async Task GetAllUserAsync_ShouldReturnCorrectData()
    {
        //arrange
        var mockUsers = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).Without(x => x.Role).CreateMany(2).ToList();

        _userServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(mockUsers);
        //act
        var result = await _userController.GetAllUserAsync() as OkObjectResult;
        //assert
        _userServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<List<UserViewModel>>(result.Value);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(mockUsers, result.Value);
    }
    [Fact]
    public async Task GetAllUserAsync_ShouldReturnNoContent_WhenIsNullOrEmpty()
    {
        //act
        var result = await _userController.GetAllUserAsync() as NoContentResult;
        //assert
        _userServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }
    [Fact]
    public async Task GetUserPaginationAsync_ShouldReturnCorrectDataWithDefaultParameter()
    {
        //arrange
        var mock = _fixture.Build<Pagination<UserViewModel>>().Without(x => x.Items).Create();

        _userServiceMock.Setup(
            x => x.GetUserPaginationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mock);

        //act
        var result = await _userController.GetUserPaginationAsync() as OkObjectResult;

        //assert
        _userServiceMock.Verify(x => x.GetUserPaginationAsync(
            It.Is<int>(x => x.Equals(0)),
            It.Is<int>(x => x.Equals(10))
            ), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<Pagination<UserViewModel>>(result.Value);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(mock, result.Value);
    }
    [Fact]
    public async Task GetUserPaginationAsync_ShouldReturnCorrectDataWithParameter()
    {
        //arrange
        var mock = _fixture.Build<Pagination<UserViewModel>>().Without(x => x.Items).Create();

        _userServiceMock.Setup(
            x => x.GetUserPaginationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(mock);

        //act
        var result = await _userController.GetUserPaginationAsync(1, 100) as OkObjectResult;

        //assert
        _userServiceMock.Verify(x => x.GetUserPaginationAsync(
            It.Is<int>(x => x.Equals(1)),
            It.Is<int>(x => x.Equals(100))
            ), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<Pagination<UserViewModel>>(result.Value);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(mock, result.Value);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnCorrectData()
    {
        //arrange
        var mockUser = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).Without(x => x.Role).Create();

        _userServiceMock.Setup(
            x => x.GetUserByIdAsync(mockUser._Id)).ReturnsAsync(mockUser);
        //act
        var result = await _userController.GetUserByIdAsync(mockUser._Id) as OkObjectResult;

        //assert
        _userServiceMock.Verify(
            x => x.GetUserByIdAsync(
                It.IsAny<string>()), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<UserViewModel>(result.Value);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(mockUser, result.Value);
    }
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNoContent_WhenPassNonExistId()
    {
        //arrange
        var mockUser = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).Without(x => x.Role).Create();

        _userServiceMock.Setup(
            x => x.GetUserByIdAsync(mockUser._Id)).ReturnsAsync(mockUser);
        //act
        var result = await _userController.GetUserByIdAsync(It.IsAny<string>()) as NoContentResult;

        //assert
        _userServiceMock.Verify(
            x => x.GetUserByIdAsync(
                mockUser._Id), Times.Never);
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }
    [Fact]
    public async Task DisableUserById_ShouldReturnOk()
    {
        //arrange
        var mock = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).Without(x => x.Role).Create();

        _userServiceMock.Setup(
            x => x.DisableUserById(mock._Id)).ReturnsAsync(true);

        //act
        var result = await _userController.DisableUserById(mock._Id) as OkObjectResult;

        //assert
        _userServiceMock.Verify(x => x.DisableUserById(It.Is<string>(x => x.Equals(mock._Id))), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }
    [Fact]
    public async Task DisableUserById_ShouldReturnBadRequest()
    {
        //arrange
        var mock = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).Without(x => x.Role).Create();

        _userServiceMock.Setup(
            x => x.DisableUserById(It.IsAny<string>())).ReturnsAsync(false);

        //act
        var result = await _userController.DisableUserById(It.IsAny<string>()) as BadRequestObjectResult;

        //assert
        _userServiceMock.Verify(x => x.DisableUserById(It.IsAny<string>()), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task ChangeUserRole_ShouldReturnNoContent_WhenServiceReturnTrue()
    {
        var updateDTO = _fixture.Build<UpdateRoleDTO>().Create();
        _userServiceMock.Setup(us => us.ChangeUserRole(updateDTO.UserID, updateDTO.RoleID)).ReturnsAsync(true);
        var result = await _userController.ChangeUserRole(updateDTO);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ChangeUserRole_ShouldReturnBadRequest_WhenServiceReturnFalse()
    {
        var updateDTO = _fixture.Build<UpdateRoleDTO>().Create();
        _userServiceMock.Setup(us => us.ChangeUserRole(updateDTO.UserID, updateDTO.RoleID)).ReturnsAsync(false);
        var result = await _userController.ChangeUserRole(updateDTO);
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public async Task ChangeUserRole_ShouldReturnBadRequest_WhenSuperAdminChangeOwnRole()
    {
        var admin = _fixture.Build<User>().Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Without(x => x.SubmitQuizzes).Create();
        admin.RoleId = 1;

        _claimsServiceMock.Setup(ad => ad.GetCurrentUserId).Returns(admin.Id);
        var updateDTO = _fixture.Build<UpdateRoleDTO>().Create();
        updateDTO.RoleID = 1;
        updateDTO.UserID = admin.Id;
        var result = await _userController.ChangeUserRole(updateDTO);
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public async Task LoginWithGoogle_ShouldReturnBadRequest()
    {
        //Arrange
        ExternalAuthDto externalAuthDto = _fixture.Build<ExternalAuthDto>().Create();

        //Act
        var result = await _userController.LoginWithGoogle(externalAuthDto);

        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task LoginWithGoogle_ShouldReturnOk()
    {
        // Arrange
        ExternalAuthDto externalAuthDto = _fixture.Build<ExternalAuthDto>().Create();
        var payload = _fixture.Build<GoogleJsonWebSignature.Payload>().Create();
        var token = _fixture.Create<Token>();
        var users = _fixture.Build<UserViewModel>().Without(x => x.Syllabuses).CreateMany(0).ToList();
        // Setup
        _externalAuthUtilsMock.Setup(eau => eau.VerifyGoogleToken(It.IsAny<ExternalAuthDto>())).ReturnsAsync(payload);
        _userServiceMock.Setup(us => us.GetAllAsync()).ReturnsAsync(users);
        _userServiceMock.Setup(us => us.AddUserAsync(It.IsAny<User>()));
        _userServiceMock.Setup(us => us.LoginWithEmail(It.IsAny<LoginWithEmailDto>())).ReturnsAsync(token);
        _userServiceMock.Setup(us => us.AddUserAsync(It.IsAny<User>())).Callback((User user) => users.Add(_mapperConfig.Map<UserViewModel>(user)));
        // Act
        var result = await _userController.LoginWithGoogle(externalAuthDto);
        var result_user = await _userController.LoginWithGoogle(externalAuthDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(token);
        result_user.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(token);
    }
    [Fact]
    public async Task Logout_ReturnNoContentResult()
    {
        _userServiceMock.Setup(u => u.LogoutAsync()).ReturnsAsync(true);

        //Act
        var result = await _userController.Logout();
        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Logout_ReturnUnAuthorizeResult()
    {
        _userServiceMock.Setup(u => u.LogoutAsync()).ReturnsAsync(false);

        //Act
        var result = await _userController.Logout();
        //Assert
        result.Should().BeOfType<UnauthorizedResult>();
    }
    [Fact]
    public async Task AddUserManually_ShouldReturnUser()
    {
        var userMockData = _fixture.Build<AddUserManually>().Create();
        var user = _mapperConfig.Map<User>(userMockData);
        _userServiceMock.Setup(x => x.AddUserManualAsync(userMockData)).ReturnsAsync(user);
        var resulltController = _userController.AddUserManually(userMockData);
        Assert.NotNull(resulltController);
        //Assert.IsType<OkObjectResult>(resulltController);
    }
    [Fact]
    public async Task AddUserManually_ShouldNoUserReturn()
    {
        var userMockData = _fixture.Build<AddUserManually>().Create();
        var user = _mapperConfig.Map<User>(userMockData);
        _userServiceMock.Setup(x => x.AddUserManualAsync(userMockData));

        var resultController = await _userController.AddUserManually(userMockData);
        BadRequestObjectResult actual = resultController as BadRequestObjectResult;
        Assert.NotNull(actual);
    }
    [Fact]
    public async Task ImportExcel_ReturnNoContentResult()
    {
        //Arrange
        var fileMock = new Mock<IFormFile>();
        var users = _fixture.Build<User>().OmitAutoProperties().CreateMany(10).ToList();
        _userServiceMock.Setup(u => u.ImportExcel(fileMock.Object)).ReturnsAsync(users);
        //Act
        var result = await _userController.ImportExcel(fileMock.Object);
        //Assert
        result.Should().BeOfType<OkResult>();
    }
    [Fact]
    public async Task ImportExcel_ReturnBadRequestResult()
    {
        //Arrange
        var fileMock = new Mock<IFormFile>();
        //Act
        var result = await _userController.ImportExcel(fileMock.Object);
        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    [Fact]
    public async Task SearchUserWithFilter_ShouldReturnCorrectData()
    {
        //arrange
        var mockUsers = _fixture.Build<SearchAndFilterUserViewModel>().With(u => u.Type, "Admin")
                                                                    .Without(u => u.Id)
                                                                    .Without(u => u.Email)
                                                                    .Without(u => u.DateOfBirth)
                                                                    .CreateMany(3).ToList();
        _userServiceMock.Setup(x => x.SearchUsersWithFilter(mockUsers[1].Fullname, mockUsers[1].Gender, 1, mockUsers[1].Level)).ReturnsAsync(mockUsers);
        //act
        var result = await _userController.Search(mockUsers[1].Fullname, mockUsers[1].Gender, 1, mockUsers[1].Level) as OkObjectResult;
        //assert
        _userServiceMock.Verify(x => x.SearchUsersWithFilter(mockUsers[1].Fullname, mockUsers[1].Gender, 1, mockUsers[1].Level), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<List<SearchAndFilterUserViewModel>>(result.Value);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(mockUsers, result.Value);
    }

    [Fact]
    public async Task SearchUserWithFilter_ShouldReturnNoContent_WhenIsNullOrEmpty()
    {
        //act
        var result = await _userController.Search("", "", 69, "") as NoContentResult;
        //assert
        _userServiceMock.Verify(x => x.SearchUsersWithFilter("", "", 69, ""), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }
    [Fact]
    public void VerifyToken_ReturnNoContentResult()
    {
        //Arrange
        var token = _fixture.Create<string>();
        var jwtDto = _fixture.Build<JwtDTO>().Create();
        _userServiceMock.Setup(u => u.CheckToken(token)).Returns(jwtDto);
        //Act
        var result = _userController.VerifyToken(token);
        //Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    [Fact]
    public void VerifyToken_ReturnBadRequestResult()
    {
        //Arrange
        var token = _fixture.Create<string>();
        //Act
        var result = _userController.VerifyToken(token);
        //Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    [Fact]
    public async Task Refresh_ShouldReturnOk()
    {
        var token = _fixture.Build<Token>()
                            .Create();
        _userServiceMock.Setup(x => x.RefreshTokenV2(token.RefreshToken)).ReturnsAsync(token);

        // Assert 
        var result = await _userController.Refresh(token.RefreshToken);
        result.Should().BeAssignableTo<OkObjectResult>();
        var actualResult = result as OkObjectResult;
        actualResult!.Value.Should().BeEquivalentTo(token);
    }

    [Fact]
    public async Task Refresh_ShouldReturnBadRequest()
    {
        var token = _fixture.Build<Token>()
                            .Create();
        var refreshToken = token.RefreshToken;
        _userServiceMock.Setup(x => x.RefreshTokenV2(refreshToken))!.ReturnsAsync(token = null);

        var result = await _userController.Refresh(refreshToken);
        result.Should().BeAssignableTo<BadRequestObjectResult>();
        var actualResult = result as BadRequestObjectResult;
        actualResult!.Value.Should().BeEquivalentTo("Invalid RefreshToken");
    }
}
