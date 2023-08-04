using Application.Commons;
using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.AtttendanceModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Infrastructures.Repositories
{
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AttendanceRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public AttendanceRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService, IMapper mapper) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public List<Attendance> GetAttendancesByTraineeClassID(Guid id)
        {
            var findAttendanceResult = _dbContext.Attendances.Where(x => x.TrainingClassId == id).ToList();
            return findAttendanceResult;
        }

        public List<Attendance> GetAttendancesByTraineeID(Guid id)
        {
            var finAttendaceResult = _dbContext.Attendances.Where(x => x.UserId == id).ToList();
            return finAttendaceResult;
        }


        public async Task<Pagination<Attendance>> GetAllAttendanceWithFilter(Expression<Func<Attendance, bool>> expression, int pageIndex, int pageSize)
        {

            var value = _dbSet.Include(x => x.User).Include(x => x.Application).Include(x => x.TrainingClass).AsQueryable();
            Pagination<Attendance> pagination = await ToPagination(value, expression, pageIndex, pageSize);

            return pagination.Items.IsNullOrEmpty() ? null : pagination;
        }

        public int CountAbsentedDate(Guid traineeId, Guid classId)
        {
            try
            {
                var absentedDates = _dbContext.Attendances.Include("User").Include("TrainingClass").Where(
                        x => x.User.Id == traineeId &&
                        x.TrainingClass.Id == classId &&
                        x.Status != nameof(AttendanceStatusEnums.Present)).Count();
                return absentedDates;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task<List<AttendanceMailDto>> GetAbsentAttendanceOfDay(DateTime date)
        {
            var attendances = (await _dbContext.Attendances.Include("User").Include("TrainingClass").ToListAsync())
                            .Where(x => x.Status != nameof(AttendanceStatusEnums.Present)
                                     && x.Date.Date == date.Date).ToList();
            var result = _mapper.Map<List<AttendanceMailDto>>(attendances);
            foreach (var r in result)
            {
                r.NumOfAbsented = CountAbsentedDate(r.UserId!.Value, r.TrainingClassId!.Value);
            }
            return result;
        }
    }
}
