using Application.Interfaces;
using Application.Models.ApplicationModels;
using Application.Repositories;
using Application.ViewModels.AtttendanceModels;
using Domain.Entities;
using Domain.Enums.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class ApplicationRepository : GenericRepository<Applications>, IApplicationRepository
    {
        private readonly AppDbContext _dbContext;
        public ApplicationRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }
 

        public async Task<IList<Applications>> GetAllApplicationByClassAndDateTime(Guid? classId, DateTime dateTime)
        {
            var application = await _dbContext.Applications.AsNoTracking().Where(x => x.Id == classId && DateOnly.FromDateTime(x.AbsentDateRequested) == DateOnly.FromDateTime(dateTime))
                                                           .ToListAsync();
            return application;
        }

        public async Task<Applications> GetApplicationByUserAndClassId(AttendanceDTO attendance, Guid id)
        {
            var application = await _dbContext.Applications.AsNoTracking().FirstOrDefaultAsync(x => x.TrainingClassId == id
                                                                                     && x.UserId == attendance.UserId
                                                                                     && x.AbsentDateRequested.Date == attendance.Date
                                                                                     && x.Approved
                                                                                     );
            return application;
        }

        public Expression<Func<Applications, bool>> GetFilterExpression(Guid classId, ApplicationFilterDTO filter)
        {
            return x =>
                   (string.IsNullOrEmpty(filter.Search) || x.Reason.Contains(filter.Search))
                   && (filter.Approved == x.Approved || filter.Approved == null)
                   && (filter.UserID == Guid.Empty || filter.UserID == x.UserId)
                   && (classId == Guid.Empty || classId == x.TrainingClassId)
                   && ((filter.ByDateType.Equals(nameof(ApplicationFilterByEnum.CreationDate), StringComparison.OrdinalIgnoreCase) && filter.FromDate <= x.CreationDate && x.CreationDate <= filter.ToDate)
                       || (filter.ByDateType.Equals(nameof(ApplicationFilterByEnum.RequestDate), StringComparison.OrdinalIgnoreCase) && filter.FromDate <= x.AbsentDateRequested && x.AbsentDateRequested <= filter.ToDate));
        }

    }
}
