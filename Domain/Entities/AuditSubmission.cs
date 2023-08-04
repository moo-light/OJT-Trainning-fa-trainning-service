using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditSubmission : BaseEntity
    {
        public DateTime SubmissionDate { get; set; }

        public double TotalGrade { get; set; }

        public string Message { get; set; } = default!;

        public Guid AuditPlanId { get; set; }

        public AuditPlan AuditPlan { get; set; } = default!;

        public Guid UserId { get; set; }

        public ICollection<DetailAuditSubmission> DetailAuditSubmissions { get; set; } = default!;



    }
}
