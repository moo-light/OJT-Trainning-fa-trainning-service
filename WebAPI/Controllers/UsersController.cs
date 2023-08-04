using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Utils;
using WebAPI.Services;
using Application.ViewModels.TokenModels;
using Microsoft.IdentityModel.Tokens;
using Domain.Enums;
using System.Security.Claims;
using Domain.Entities;
using AutoMapper;
using Google.Apis.Auth;

namespace WebAPI.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IClaimsService _claimsService;
        private readonly IExternalAuthUtils _externalAuthUtils;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IClaimsService claimsService, IExternalAuthUtils externalAuthUtils, IMapper mapper)
        {
            _userService = userService;
            _claimsService = claimsService;
            _externalAuthUtils = externalAuthUtils;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterDTO loginObject)
        {
            var checkAdd = await _userService.RegisterAsync(loginObject);
            if (checkAdd)
            {
                return Ok("Register Successfully!");
            }
            return BadRequest("Register Failed!");
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginDTO loginObject)
        {
            var token = await _userService.LoginAsync(loginObject);
            if (token == null)
            {
                return BadRequest("Login Failed!");
            }
            return Ok(token);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var checkChanged = await _userService.ChangePasswordAsync(changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (!checkChanged.Contains("Success"))
                return BadRequest(checkChanged);

            return Ok(checkChanged);
        }
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string accessToken, string refreshtoken)
        {
            var newToken = await _userService.RefreshToken(accessToken, refreshtoken);
            if (newToken == null)
            {
                return BadRequest();
            }
            return Ok(newToken);
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> TestAuthorize()
        //{
        //    var test = _claimsService.GetCurrentUserId;
        //    return Ok(test);
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateDTO updateObject)
        {
            if(_claimsService.GetCurrentUserId.Equals(updateObject.UserId))
            {
                var result = await _userService.UpdateUserInformation(updateObject);
                if (result) return NoContent();
                else return BadRequest("Something went wrong");
            } else return BadRequest("Can not update different account");
            
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _userService.SendResetPassword(email);
            if (!result.IsNullOrEmpty())
            {
                return Ok();
            }
            else return BadRequest("Cannot found User");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetObj)
        {
            var result = await _userService.ResetPassword(resetObj);
            if (result)
            {
                return Ok(result);
            }
            else return BadRequest();
        }

        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.UserPermission), nameof(PermissionEnum.View))]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var users = await _userService.GetAllAsync();
            if (users.IsNullOrEmpty())
            {
                return NoContent();
            }
            return Ok(users);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var users = await _userService.GetUserPaginationAsync(pageIndex, pageSize);
            return Ok(users);
        }

        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.UserPermission), nameof(PermissionEnum.Modifed))]
        [HttpPut]
        public async Task<IActionResult> ChangeUserRole(UpdateRoleDTO updateDTO)
        {
            var currentUserId = _claimsService.GetCurrentUserId;
            if (currentUserId == updateDTO.UserID)
            {
                return BadRequest();
            }
            var result = await _userService.ChangeUserRole(updateDTO.UserID, updateDTO.RoleID);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
            {
                return NoContent();
            }
            return Ok(user);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPut]
        public async Task<IActionResult> DisableUserById(string userId)
        {
            var result = await _userService.DisableUserById(userId);
            if (result is false)
            {
                return BadRequest("Disable failed");
            }
            return Ok("Disable successfully!");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.LogoutAsync();
            if (result)
            {
                return NoContent();
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> LoginWithGoogle(ExternalAuthDto externalAuthDto)
        {
            var payload = await _externalAuthUtils.VerifyGoogleToken(externalAuthDto);
            if (payload == null)
            {
                return BadRequest("Invalid external authentication");
            }
            var newUser = new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                UserName = payload.Email,
                AvatarUrl = payload.Picture
            };
            var user = _userService.GetAllAsync().Result.SingleOrDefault(u => u.Email == newUser.Email);
            if (user == null)
            {
                await _userService.AddUserAsync(newUser);
            }

            var token = await _userService.LoginWithEmail(_mapper.Map<LoginWithEmailDto>(newUser));
            return Ok(token);
        }
        [Authorize]
        [HttpPost]
        [ClaimRequirement(nameof(PermissionItem.UserPermission), nameof(PermissionEnum.FullAccess))]
        public async Task<IActionResult> AddUserManually(AddUserManually addUserManually)
        {
            var resultAddUserManual = await _userService.AddUserManualAsync(addUserManually);
            if (resultAddUserManual is not null)
            {
                return Ok(resultAddUserManual);

            }
            return BadRequest("Error at AddUserManually at Controller");

        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.UserPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var import = await _userService.ImportExcel(file);
            if (!import.IsNullOrEmpty())
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(string? searchString, string? gender, int? role, string? level)
        {
            IEnumerable<SearchAndFilterUserViewModel> obj = await _userService.SearchUsersWithFilter(searchString, gender, role, level);
            if (obj != null && obj.Any())
                return Ok(obj);
            return NoContent();
        }
        [HttpGet]
        public IActionResult VerifyToken(string token)
        {
            var result = _userService.CheckToken(token);
            if (result is not null) return Ok(result);
            else return BadRequest("Invalid Token");
        }
        [HttpPost]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var result = await _userService.RefreshTokenV2(refreshToken);
            if(result is not null) 
            {
                return Ok(result);
            } else return BadRequest("Invalid RefreshToken");
        }
    }
}