
using System.Data;
using Application.Commons;
using Application.Filter;
using Application.Filter.UserFilter;
using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.TokenModels;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.UserViewModels;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentTime _currentTime;
    private readonly AppConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly ISendMailHelper _sendMailHelper;
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, AppConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentTime = currentTime;
        _configuration = configuration;
    }

    public async Task<User> AddUserManualAsync(AddUserManually addUserManually)
    {
        addUserManually.Pass = addUserManually.Pass.Hash();
        var mapUserwithAddUserManual = _mapper.Map<User>(addUserManually);


        await _unitOfWork.UserRepository.AddAsync(mapUserwithAddUserManual);


        if (await _unitOfWork.SaveChangeAsync() > 0)
        {
            return await _unitOfWork.UserRepository.GetUserByEmailAsync(addUserManually.Email);
        };
        return null;
    }

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, AppConfiguration configuration, IConfiguration config, IMemoryCache memoryCache, ISendMailHelper sendMailHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentTime = currentTime;
        _configuration = configuration;
        _memoryCache = memoryCache;
        _sendMailHelper = sendMailHelper;
    }


    public async Task<bool> ChangeUserRole(Guid userId, int roleId)
    {
        var updateUser = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (updateUser != null)
        {
            updateUser.RoleId = roleId;
            _unitOfWork.UserRepository.Update(updateUser);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }
        return false;

    }

    public async Task<string> ChangePasswordAsync(string oldPassword, string newPassword)
    {

        var user = await _unitOfWork.UserRepository.GetAuthorizedUserAsync();
        if (user == null) throw new Exception("User Not Exist");

        if (oldPassword.CheckPassword(user.PasswordHash!) == false)
            throw new Exception("Old password are wrong");

        var result = await _unitOfWork.UserRepository.ChangeUserPasswordAsync(user, newPassword);
        if (result)
            return "Update Success!";
        return "Update Failed";
    }


    public async Task<Token> LoginAsync(LoginDTO userDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(userDto.UserName);
        if (userDto.Password.CheckPassword(user.PasswordHash!) == false)
        {
            throw new Exception("Password is not incorrect!!");
        }
        var refreshToken = RefreshTokenString.GetRefreshToken();
        var accessToken = user.GenerateJsonWebToken(_configuration.JWTSecretKey, _currentTime.GetCurrentTime());
        var expireRefreshTokenTime = DateTime.Now.AddHours(24);

        user.RefreshToken = refreshToken;
        user.ExpireTokenTime = expireRefreshTokenTime;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangeAsync();
        return new Token
        {
            UserName = user.UserName,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    }
    public JwtDTO CheckToken(string accessToken)
    {
        if (accessToken.IsNullOrEmpty())
        {
            throw new Exception("Invalid Token");
        }
        else
        {
            var principal = accessToken.VerifyToken(_configuration.JWTSecretKey);
            if (principal is not null) return principal;
            else throw new Exception("Invalid Token");
        }
    }

    public async Task<Token> RefreshTokenV2(string refreshToken)
    {
        _ = _memoryCache.TryGetValue(refreshToken, out Guid? userId);
        if (userId is not null)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId.Value, x => x.Role);
            if (user is not null)
            {

                var newAccessToken = user.GenerateJsonWebToken(_configuration.JWTSecretKey, _currentTime.GetCurrentTime());
                var newRefreshToken = RefreshTokenString.GetRefreshToken();
                user.RefreshToken = newRefreshToken;
                _unitOfWork.UserRepository.Update(user);
                _memoryCache.Set(newRefreshToken, user.Id, DateTimeOffset.Now.AddDays(1));
                await _unitOfWork.SaveChangeAsync();
                return new Token
                {
                    UserName = user.UserName,
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            else throw new Exception("Can not found any user with that Id");
        }
        else throw new Exception("Not exist any userId with that RefreshToken");


    }
    public async Task<Token> RefreshToken(string accessToken, string refreshToken)
    {
        if (accessToken.IsNullOrEmpty() || refreshToken.IsNullOrEmpty())
        {
            return null;
        }
        var principal = accessToken.GetPrincipalFromExpiredToken(_configuration.JWTSecretKey);

        var id = principal?.FindFirstValue("userID");
        _ = Guid.TryParse(id, out Guid userID);
        var userLogin = await _unitOfWork.UserRepository.GetByIdAsync(userID, x => x.Role);
        if (userLogin == null || userLogin.RefreshToken != refreshToken || userLogin.ExpireTokenTime <= DateTime.Now)
        {
            return null;
        }

        var newAccessToken = userLogin.GenerateJsonWebToken(_configuration.JWTSecretKey, _currentTime.GetCurrentTime());
        var newRefreshToken = RefreshTokenString.GetRefreshToken();

        userLogin.RefreshToken = newRefreshToken;
        userLogin.ExpireTokenTime = DateTime.Now.AddDays(1);
        _unitOfWork.UserRepository.Update(userLogin);
        await _unitOfWork.SaveChangeAsync();

        return new Token
        {
            UserName = userLogin.UserName,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> RegisterAsync(RegisterDTO userDto)
    {
        var isExisted = await _unitOfWork.UserRepository.FindAsync(u => u.UserName == userDto.UserName || u.Email == userDto.Email);

        if (isExisted.Count() > 0)
        {
            throw new Exception("Account exited please try again");
        }

        var newUser = new User
        {
            UserName = userDto.UserName,
            PasswordHash = userDto.Password.Hash(),
            Email = userDto.Email, // lay email lam username luon
        };
        await _unitOfWork.UserRepository.AddAsync(newUser);
        return await _unitOfWork.SaveChangeAsync() > 0;
    }

    public async Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO)
    {
        string email;
        _ = _memoryCache.TryGetValue(resetPasswordDTO.Code, out email);


        if (email != null)
        {
            if (resetPasswordDTO.NewPassword.Equals(resetPasswordDTO.ConfirmPassword))
            {

                var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
                if (user != null)
                {
                    resetPasswordDTO.NewPassword = resetPasswordDTO.NewPassword.Hash();
                    _ = _mapper.Map(resetPasswordDTO, user, typeof(ResetPasswordDTO), typeof(User));
                    _unitOfWork.UserRepository.Update(user);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                throw new Exception("Password and Confirm Password is not match");
            }
        }
        throw new Exception("Not exsited Code");



    }


    // <summarry>
    // Method send Confirmation Code Through Email 
    // </summary>
    public async Task<string> SendResetPassword(string email)
    {
        var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
        if (user != null)
        {
            var key = StringUtils.RandomString(6);
            //Get project's directory and fetch ForgotPasswordTemplate content from EmailTemplates
            string exePath = Environment.CurrentDirectory.ToString();
            if (exePath.Contains(@"\bin\Debug\net7.0"))
                exePath = exePath.Remove(exePath.Length - (@"\bin\Debug\net7.0").Length);
            string FilePath = exePath + @"\EmailTemplates\ForgotPasswordTemplate.html";
            StreamReader streamreader = new StreamReader(FilePath);
            string MailText = streamreader.ReadToEnd();
            streamreader.Close();
            //Replace [resetpasswordkey] = key
            MailText = MailText.Replace("[resetpasswordkey]", key);
            //Replace [emailaddress] = email
            MailText = MailText.Replace("[emailaddress]", email);
            var result = await _sendMailHelper.SendMailAsync(email, "ResetPassword", MailText);
            if (!result) return string.Empty;

            _memoryCache.Set(key, email, DateTimeOffset.Now.AddMinutes(10));
            return key;


        }
        else
        {
            throw new Exception("User not available");
        }
    }

    public async Task<bool> UpdateUserInformation(UpdateDTO updateUser)
    {
        if (updateUser != null)
        {

            User user = (await _unitOfWork.UserRepository.GetByIdAsync(updateUser.UserId))!;
            _ = _mapper.Map(updateUser, user, typeof(UpdateDTO), typeof(User));

            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.SaveChangeAsync() > 0) return true;
            else return false;
        }
        throw new Exception("User can not be null");



    }
    /// <summary>
    /// Return collection of item with parameter
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async Task<Pagination<UserViewModel>> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var users = await _unitOfWork.UserRepository.ToPagination(pageIndex, pageSize);
        var result = _mapper.Map<Pagination<UserViewModel>>(users);
        return result;
    }
    public async Task<List<UserViewModel>> GetAllAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();
        var result = _mapper.Map<List<UserViewModel>>(users);
        return result;
    }
    /// <summary>
    /// Find and return UserViewModel 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserViewModel> GetUserByIdAsync(string id)
    {
        var userId = _mapper.Map<Guid>(id);
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        var result = _mapper.Map<UserViewModel>(user);
        return result;
    }

    public async Task<bool> DisableUserById(string userId)
    {
        var result = false;
        var user = await _unitOfWork.UserRepository.GetByIdAsync(_mapper.Map<Guid>(userId));
        if (user != null)
        {
            _unitOfWork.UserRepository.SoftRemove(user);
            var success = await _unitOfWork.SaveChangeAsync();
            if (success == 1)
            {
                result = true;
            }
        }
        return result;
    }

    public async Task<bool> LogoutAsync()
    {
        var user = await _unitOfWork.UserRepository.GetAuthorizedUserAsync();
        if (user == null)
        {
            return false;
        }
        user.RefreshToken = null;
        user.ExpireTokenTime = null;
        _unitOfWork.UserRepository.Update(user);
        var result = await _unitOfWork.SaveChangeAsync() > 0;
        if (result == false)
        {
            throw new Exception("SaveChange Fail!");
        }
        return true;
    }
    public async Task AddUserAsync(User user)
    {
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task<Token> LoginWithEmail(LoginWithEmailDto loginDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.Email);
        if (user != null)
        {
            var refreshToken = RefreshTokenString.GetRefreshToken();
            var accessToken = user.GenerateJsonWebToken(_configuration.JWTSecretKey, _currentTime.GetCurrentTime());
            var expireRefreshTokenTime = DateTime.Now.AddHours(24);

            user.RefreshToken = refreshToken;
            user.ExpireTokenTime = expireRefreshTokenTime;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            return new Token
            {
                UserName = user.UserName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        return null;
    }
    public async Task<List<User>> ImportExcel(IFormFile file)
    {
        // chỉ nhận các file extension của excel
        var supportedTypes = new[] { "xls", "xlsx" };
        var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
        if (!supportedTypes.Contains(fileExt))
        {
            throw new Exception("You can only upload Excel files (.xls / .xlsx");
        }

        // license của EPPlus
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        try
        {
            var list = new List<User>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowcount; row++)
                    {
                        try
                        {
                            list.Add(new User
                            {
                                UserName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                PasswordHash = worksheet.Cells[row, 3].Value.ToString().Hash(),
                                FullName = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                Email = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                DateOfBirth = DateTime.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                Gender = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                //LoginDate = DateTime.Parse(worksheet.Cells[row, 8].Value.ToString().Trim()),
                                CreationDate = DateTime.Parse(worksheet.Cells[row, 8].Value.ToString().Trim()),
                                //CreatedBy = Guid.Parse(worksheet.Cells[row,10].Value.ToString().Trim()),
                                //ModificationDate = DateTime.Parse(worksheet.Cells[row, 11].Value.ToString().Trim()),
                                //ModificationBy = Guid.Parse(worksheet.Cells[row,12].Value.ToString().Trim()),
                                //DeletionDate = DateTime.Parse(worksheet.Cells[row, 13].Value.ToString().Trim()),
                                //DeleteBy = Guid.Parse(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                //IsDeleted = bool.Parse(worksheet.Cells[row, 15].Value.ToString().Trim()),
                                RoleId = int.Parse(worksheet.Cells[row, 9].Value.ToString().Trim()),
                                //AvatarUrl = worksheet.Cells[row, 10].Value.ToString().Trim(),
                            });
                        }
                        catch (NullReferenceException)
                        {
                            throw new Exception("Excel file is missing some data");
                        }
                    }
                    await _unitOfWork.UserRepository.AddRangeAsync(list);
                    await _unitOfWork.SaveChangeAsync();
                }
                return list;
            }
        }
        catch (Exception)
        {
            throw new Exception("Excel file has invalid data");
        }
    }
    public async Task<List<SearchAndFilterUserViewModel>> SearchUsersWithFilter(string? searchString, string? gender, int? role, string? level)
    {
        //init filter
        ICriterias<User> genderCriteria = new GenderCriteria(gender);
        ICriterias<User> roleCriteria = new RoleCriteria(role);
        ICriterias<User> levelCriteria = new LevelCriteria(level);
        ICriterias<User> andCriteria = new AndUserCriteria(genderCriteria, roleCriteria, levelCriteria);
        //search user with filter
        if (searchString != null)
        {
            var listUser = await _unitOfWork.UserRepository.FindAsync(u => u.FullName.Contains(searchString));
            var result = _mapper.Map<List<SearchAndFilterUserViewModel>>(andCriteria.MeetCriteria(listUser));
            return LinkingRoleName(listUser, result);
        }
        //using filter only (filter from all users)
        else if (!gender.IsNullOrEmpty() || role != null || !level.IsNullOrEmpty())
        {
            var listUser = await _unitOfWork.UserRepository.GetAllAsync();
            var result = _mapper.Map<List<SearchAndFilterUserViewModel>>(andCriteria.MeetCriteria(listUser));
            return LinkingRoleName(listUser, result);
        }
        //return all user if all input are null
        else
        {
            var listUser = await _unitOfWork.UserRepository.GetAllAsync();
            var result = _mapper.Map<List<SearchAndFilterUserViewModel>>(listUser);
            return LinkingRoleName(listUser, result);
        }
    }
    private protected List<SearchAndFilterUserViewModel> LinkingRoleName(List<User> listUser, List<SearchAndFilterUserViewModel> listUserFiltered)
    {
        foreach (var user in listUser)
        {
            foreach (var uf in listUserFiltered)
                if (uf.Id.Equals(user.Id))
                    uf.Type = ((RoleEnums)user.RoleId).ToString();
        }
        return listUserFiltered;
    }
}
