using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels.ViewModels
{
    public class AuditPlanViewModel
    {
        public Guid Id { get; set; }
        public string AuditPlanName { get; set; } = default!;
        public string AuditLocation { get; set; } = default!;
        public DateTime AuditDate { get; set; }
        public IEnumerable<AuditQuestionViewModel> AuditQuestions { get; set; } = default!;
    }
}
