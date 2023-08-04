using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetailAuditQuestion : BaseEntity
    {
        public Guid AuditQuestionId { get; set; }

        public AuditQuestion AuditQuestion { get; set; } = default!;

        public Guid AuditPlanId { get; set; }

        public AuditPlan AuditPlan { get; set; } = default!;


        public ICollection<DetailAuditSubmission> DetailAuditSubmissions { get; set; } = default!;



    }
}
