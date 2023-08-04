using Application.Models.ApplicationModels;
using Application.Repositories;
using Application.ViewModels.AtttendanceModels;
using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructures.Repositories
{
    public interface IApplicationRepository : IGenericRepository<Applications>
    {
        Task<IList<Applications>> GetAllApplicationByClassAndDateTime(Guid? classId, DateTime dateTime);
        Task<Applications> GetApplicationByUserAndClassId(AttendanceDTO attendance, Guid classId);
        Expression<Func<Applications, bool>> GetFilterExpression(Guid classId, ApplicationFilterDTO filter);
    }
}