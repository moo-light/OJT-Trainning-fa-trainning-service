using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditPlan : BaseEntity
    {
        public string AuditPlanName { get; set; } = default!;

        public string AuditLocation { get; set; } = default!;

        public DateTime AuditDate { get; set; }

        public Guid LectureId { get; set; }

        public Lecture Lecture { get; set; } = default!;

        public ICollection<DetailAuditQuestion> DetailAuditQuestions { get; set; } = default!;

        public ICollection<AuditSubmission> AuditSubmissions { get; set; } = default!;


    }
}
