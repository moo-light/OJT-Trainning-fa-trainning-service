using Application.Interfaces;
using Application.Repositories;
using Application.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext,
        ICurrentTime timeService,
        IClaimsService claimsService)
        : base(dbContext,
              timeService,
              claimsService)
    {
        _dbContext = dbContext;
    }

    public async Task EditRoleAsync(Guid userId, int roleId)
    {
        var userAccount = await GetByIdAsync(userId);
        userAccount!.RoleId = roleId;
    }

    public Task<bool> CheckEmailExistedAsync(string email)
    => _dbContext.Users.AnyAsync(u => u.Email == email);

    public Task<bool> CheckUserNameExistedAsync(string username)
    => _dbContext.Users.AnyAsync(u => u.UserName == username);



    public async Task<User> GetAuthorizedUserAsync()
    {
        Guid id = _claimsService.GetCurrentUserId;
        var user = await GetByIdAsync(id);
        return user;
    }


    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(e => e.Email == email);
        if (user == null)
        {
            throw new Exception("UserName is not exist!");
        }
        return user;
    }

    public async Task<User> GetUserByUserNameANdPaswordHashAsync(string username, string passwordHash)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username
                                                                && u.PasswordHash == passwordHash);
        if (user == null)
        {
            throw new Exception("UserName & password is not correct");
        }
        return user;
    }

    public async Task<bool> ChangeUserPasswordAsync(User user, string newPassword)
    {
        user.PasswordHash = newPassword.Hash();
        Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(e => e.UserName == userName);
        if (user == null)
        {
            throw new Exception("UserName is not exist!");
        }
        return user;
    }
}
