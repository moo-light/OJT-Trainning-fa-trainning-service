using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels
{
    public class UpdateSubmissionDTO : CreateAuditSubmissionDTO
    {
        public Guid Id { get; set; }
    }
}
