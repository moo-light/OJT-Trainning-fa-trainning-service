using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.AssignmentModel;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _current;
        public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime current)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            _current = current;
        }


        public async Task<bool> DeleteAssignment(Guid assignmentID)
        {
            var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(assignmentID);
            if (assignment.IsDeleted == true) throw new Exception("Assignment is also deleted!");
            assignment.IsDeleted = true;
            _unitOfWork.AssignmentRepository.Update(assignment);
            var result = await _unitOfWork.SaveChangeAsync() > 0;
            return result;
        }

        public async Task<bool> UpdateAssignment(AssignmentUpdateModel assignmentUpdate)
        {
            var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(assignmentUpdate.AssignmentID);
            if (assignment == null) throw new Exception("Assignment is not existed!");
            assignment.AssignmentName = assignmentUpdate.AssignmentName;
            assignment.Description = assignmentUpdate.Description;
            var dbPath = assignmentUpdate.File.ImportFile("Assignments", assignment.Version.Value + 1, assignment.CreatedBy.Value);
            assignment.FileName = dbPath;
            assignment.Version = assignment.Version.Value + 1;
            if (assignment.DeadLine < assignmentUpdate.Deadline)
            {
                assignment.IsOverDue = false;
            }
            assignment.DeadLine = assignmentUpdate.Deadline;
            _unitOfWork.AssignmentRepository.Update(assignment);
            var result = await _unitOfWork.SaveChangeAsync() > 0;
            return result;
        }


        public async Task<List<Assignment>> GetAllAssignmentByLectureID(Guid lectureID)
        {
            var assignments = await _unitOfWork.AssignmentRepository.FindAsync(a => a.LectureID == lectureID && a.IsDeleted == false && a.IsOverDue == false, a => a.AssignmentSubmissions);
            //var result = _mapper.Map<List<AssignmentViewModel>>(assignments);
            return assignments;
        }

        public async Task CheckOverDue()
        {
            var assignmentsOverdue = await _unitOfWork.AssignmentRepository.FindAsync(x => x.DeadLine < _current.GetCurrentTime() && x.IsOverDue==false);
            assignmentsOverdue.ForEach(assignment => { assignment.IsOverDue = true; });
            _unitOfWork.AssignmentRepository.UpdateRange(assignmentsOverdue);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<Guid> CreateAssignment(AssignmentViewModel assignmentViewModel)
        {
            var check = await _unitOfWork.AssignmentRepository.FindAsync(x => x.LectureID == assignmentViewModel.LectureID && x.IsDeleted == false && x.IsOverDue == false);
            if (check.Count() > 0) throw new Exception("Assignment has already existed!");
            if (assignmentViewModel.DeadLine < DateTime.Now) throw new Exception("Deadline must higher than Datetime now");
            var dbPath = assignmentViewModel.File.ImportFile("Assignments", 1, _claimsService.GetCurrentUserId);
            if (dbPath.IsNullOrEmpty()) throw new Exception("Import File Fail");
            var assignment = _mapper.Map<Assignment>(assignmentViewModel);
            assignment.Id = Guid.NewGuid();
            assignment.FileName = dbPath;
            assignment.Version = 1;
            await _unitOfWork.AssignmentRepository.AddAsync(assignment);
            await _unitOfWork.SaveChangeAsync();
            return assignment.Id;
        }

        public async Task<FileEntity> DownLoad(Guid assignmentID)
        {
            var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(assignmentID);
            if (assignment == null) throw new Exception("Assignment is not exited!");
            var fileEntity = assignment.FileName.GetFileEntity();
            return fileEntity;
        }

 
    }
}
