using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAssignmentSubmisstionService
    {
        Task<Guid> AddSubmisstion(Guid assignmentID,Guid ClassId, IFormFile file);
        Task<bool> RemoveSubmisstion(Guid assignmentSubmissId);
        Task<bool> EditSubmisstion(Guid assignmentID, IFormFile file);
        Task<FileEntity> DownloadSubmiss(Guid assignmentSubmissId);
        Task<Guid> GradingandReviewSubmission(Guid assignmentSubmissId, int numberGrade, string comment);

    }
}
