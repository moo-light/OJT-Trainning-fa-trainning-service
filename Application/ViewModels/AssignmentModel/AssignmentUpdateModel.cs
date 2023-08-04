using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AssignmentModel
{
    public class AssignmentUpdateModel
    {
        public Guid AssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public IFormFile File { get; set; }
    }
}
