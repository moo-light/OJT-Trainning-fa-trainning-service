using Application.Commons;
using Application.ViewModels.AtttendanceModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<Pagination<Attendance>> GetAllAttendanceWithFilter(Expression<Func<Attendance, bool>> expression, int pageIndex, int pageSize);
        List<Attendance> GetAttendancesByTraineeClassID(Guid id);
        List<Attendance> GetAttendancesByTraineeID(Guid id);
        int CountAbsentedDate(Guid traineeId, Guid classId);
        Task<List<AttendanceMailDto>> GetAbsentAttendanceOfDay(DateTime date);
    }

}
