using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> CheckUserNameExistedAsync(string username);
    Task<bool> CheckEmailExistedAsync(string email);
    Task<User> GetUserByEmailAsync(string email);
    Task EditRoleAsync(Guid userId, int roleId);
    Task<User> GetAuthorizedUserAsync();
    Task<bool> ChangeUserPasswordAsync(User mockData, string newPassword);
    Task<User> GetUserByUserNameAsync(string userName);
   
}
