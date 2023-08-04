using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels
{
    public class AuditSubmissionViewModel
    {
        public Guid Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Message { get; set; } = default!;
        public double TotalGrade { get; set; } = default!;

        public IEnumerable<DetailAuditSubmissionViewModel> DetailAuditSubmisisonViewModel { get; set; } = default!;
    }
}
