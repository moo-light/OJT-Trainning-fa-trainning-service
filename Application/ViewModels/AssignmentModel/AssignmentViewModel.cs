using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AssignmentModel
{
    public class AssignmentViewModel
    {
        public Guid LectureID { get; set; }
        public string AssignmentName { get; set; }
        public string? Discription { get; set; }
        public DateTime DeadLine { get; set; }
        public IFormFile File { get; set; }
    }
}
