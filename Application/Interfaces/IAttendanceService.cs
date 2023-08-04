using Application.Commons;
using Application.ViewModels.AtttendanceModels;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IAttendanceService
    {
        public Task<List<Attendance>> GetAttendancesByTraineeClassID(Guid id);
        public Task<List<Attendance>> GetAttendanceByTraineeID(Guid id);

        Task<List<Attendance>> UploadAttendanceFormAsync(List<AttendanceDTO> attendanceDtos,
                                                         Guid ClassId,
                                                         string httpMethod);
        Task<Attendance> UpdateAttendanceAsync(AttendanceDTO attendanceDto, Guid classId);
        Task<Pagination<AttendanceViewDTO>> GetAllAttendanceWithFilter(Guid classId, string search, string by, bool? containApplication, DateTime? fromDate, DateTime? toDate, int pageIndex = 0, int pageSize = 40);
        public Task<List<Attendance>> GetAllAttendancesAsync();

        int CountAbsentedDate(Guid traineeId, Guid classId);
        public Task<List<AttendanceMailDto>> GetAllAbsentInfoAsync();
    }
}
