using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetailAuditSubmission : BaseEntity
    {
        public Guid AuditSubmissionId { get; set; }

        public AuditSubmission AuditSubmission { get; set; } = default!;

        public Guid DetailAuditQuestionId { get; set; }

        public DetailAuditQuestion DetailAuditQuestion { get; set; } = default!;

        public string Answer { get; set; } = default!;
    }
}
