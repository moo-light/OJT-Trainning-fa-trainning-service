using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels
{
    public class CreateAuditSubmissionDTO
    {
        public DateTime SubmissionDate { get; set; }
        public double TotalGrade { get; set; }
        public string Message { get; set; } = default!;
        public Guid AuditPlanId { get; set; }
        public Guid UserId { get; set; }
        public Guid ClassId { get; set;}
        public ICollection<CreateDetailAuditSubmission> AuditSubmissions { get; set; } = default!;

    }
}
