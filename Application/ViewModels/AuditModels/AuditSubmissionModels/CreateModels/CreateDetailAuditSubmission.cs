using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels
{
    public class CreateDetailAuditSubmission
    {
        public Guid DetailQuesionId { get; set; }
        public string Answer { get; set; } = default!;
    }
}
