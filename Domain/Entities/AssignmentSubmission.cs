using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AssignmentSubmission : FileEntity
    {
        public double? Grade { get; set; }

        public string? Comment { get; set; }

        //Set Relation to Assignment - Submisstion for what
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }


    }
}
