using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.AtttendanceModels;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Google.Apis.Logging;
using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Drawing.Text;
using System.Linq.Expressions;
using System.Threading.Channels;
using static Domain.Enums.AttendanceStatusEnums;

namespace Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly AppConfiguration _configuration;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration configuration, ICurrentTime? currentTime)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _currentTime = currentTime;
        }
        public async Task<List<Attendance>> GetAttendanceByTraineeID(Guid id)
        {
            var finResult = _unitOfWork.AttendanceRepository.GetAttendancesByTraineeID(id);
            return finResult;
        }
        public async Task<List<Attendance>> GetAttendancesByTraineeClassID(Guid id)
        {
            var findResult = _unitOfWork.AttendanceRepository.GetAttendancesByTraineeClassID(id);
            return findResult;
        }
        public async Task<Attendance> UpdateAttendanceAsync(AttendanceDTO attendanceDto, Guid classId)
        {
            await GetAndCheckClassExist(classId);

            Attendance attendance = await MapAttendance(classId, attendanceDto);
            _unitOfWork.AttendanceRepository.Update(attendance);
            return await _unitOfWork.SaveChangeAsync() > 0 ? attendance : null;
        }
        public async Task<List<Attendance>> UploadAttendanceFormAsync(List<AttendanceDTO> attendanceDtos, Guid classId, string httpMethod)
        {
            //Guid ClassId = Guid.Parse(Id);
            List<Attendance> allList = new();
            await GetAndCheckClassExist(classId);
            foreach (var attendanceDto in attendanceDtos)
            {
                Attendance attendance;

                if (attendanceDto.AttendanceId != Guid.Empty && httpMethod == "PATCH")
                {
                    attendance = await _unitOfWork.AttendanceRepository.GetByIdAsync(attendanceDto.AttendanceId.Value);
                    attendance.ThrowIfNull($"AttendanceId: {attendanceDto.AttendanceId} Is Null");
                }
                //find the attendance Application that's approved by the admin
                attendance = await MapAttendance(classId, attendanceDto);
                allList.Add(attendance);
            }
            if (httpMethod == "POST")
                await _unitOfWork.AttendanceRepository.AddRangeAsync(allList);
            else
                _unitOfWork.AttendanceRepository.UpdateRange(allList);
            return await _unitOfWork.SaveChangeAsync() > 0 ? allList : null;
        }

        public async Task<Pagination<AttendanceViewDTO>> GetAllAttendanceWithFilter(Guid classId, string search, string by, bool? containApplication, DateTime? fromDate, DateTime? toDate, int pageIndex = 0, int pageSize = 40)
        {
            by.ThrowErrorIfNotValidEnum(typeof(AttendanceStatusEnums), $"Not valid {nameof(AttendanceStatusEnums)} from {nameof(by)}");

            Expression<Func<Attendance, bool>> expression = x =>
                (classId == Guid.Empty || classId == x.TrainingClassId)    // by classId
                && (x.User != null && x.User.FullName.Contains(search))                                    // by Username
                && (containApplication == null || (x.Application != null) == containApplication) // by application                                  
                && (fromDate <= x.Date && x.Date <= toDate)  // by datetime 
                && (by == nameof(None) || by != nameof(None) && by == x.Status);       // by Status

            Pagination<Attendance> pagination = await _unitOfWork.AttendanceRepository.GetAllAttendanceWithFilter(expression, pageIndex, pageSize);

            return pagination != null ? _mapper.Map<Pagination<AttendanceViewDTO>>(pagination) ?? null : null;


        }

        public Task<List<Attendance>> GetAllAttendancesAsync()
        {
            var findResult = _unitOfWork.AttendanceRepository.GetAllAsync();
            return findResult;
        }

        public int CountAbsentedDate(Guid traineeId, Guid classId)
        {
            try
            {
                var dates = _unitOfWork.AttendanceRepository.CountAbsentedDate(traineeId, classId);
                return dates;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AttendanceMailDto>> GetAllAbsentInfoAsync()
        {
            var attendances = await _unitOfWork.AttendanceRepository.GetAbsentAttendanceOfDay(_currentTime.GetCurrentTime());
            return attendances;
        }
        public static Expression<Func<Attendance, bool>> GetFilterExpression(Guid classId, string search, string by, bool? containApplication, DateTime? fromDate, DateTime? toDate)
        {
            return x => (classId == Guid.Empty || x.TrainingClassId == classId) &&                  // Filter by classId (if provided)
                            (search == null || x.User.FullName.Contains(search)) &&                    // Filter by username (if provided)
                            (containApplication == null || x.Application != null == containApplication) &&  // Filter by application (if provided)
                            (fromDate == null || x.Date >= fromDate) &&                                 // Filter by start date (if provided)
                            (toDate == null || x.Date <= toDate) &&                                     // Filter by end date (if provided)
                            (by == nameof(None) || x.Status == by);                                     // Filter by status (if provided)
        }
        #region Private Methods


        private async Task<Attendance> MapAttendance(Guid classId, AttendanceDTO attendanceDto)
        {
            if (attendanceDto.Date == default) attendanceDto.Date = _currentTime.GetCurrentTime();

            var application = await _unitOfWork.ApplicationRepository.GetApplicationByUserAndClassId(attendanceDto, classId);
            Attendance attendance = _mapper.Map<Attendance>(attendanceDto);
            if (application is not null)
            {
                if (attendance.Status == nameof(Absent))
                    attendance.Status = nameof(AbsentPermit);
                attendance.ApplicationId = application.Id;
            }
            attendance.TrainingClassId = classId;
            return attendance;
        }
        private async Task<TrainingClass> GetAndCheckClassExist(Guid classId)
        {
            TrainingClass trainingClass = await _unitOfWork.TrainingClassRepository.GetByIdAsync(classId);
            return trainingClass.ThrowIfNull("Training Class Missing");
        }
        #endregion
    }
}
