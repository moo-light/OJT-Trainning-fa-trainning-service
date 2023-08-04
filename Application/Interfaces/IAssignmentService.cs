using Application.ViewModels.AssignmentModel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<bool> UpdateAssignment(AssignmentUpdateModel assignmentUpdate);
        Task<bool> DeleteAssignment(Guid assignmentID);

        Task<List<Assignment>> GetAllAssignmentByLectureID(Guid lectureID);

        Task CheckOverDue();

        Task<Guid> CreateAssignment(AssignmentViewModel assignmentViewModel);
        Task<FileEntity> DownLoad(Guid assignmentID);

    }
}
