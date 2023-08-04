using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels
{
    public class CreateAuditDTO
    {
        public string AuditPlanName { get; set; } = default!;
        public string AuditLocation { get; set; } = default!;
        public DateTime AuditDate { get; set; }

        public Guid LectureId { get; set; }

        public ICollection<CreateAuditQuestionDTO> CreateAuditQuestionDTOS { get; set; } = default!;
    }
}
