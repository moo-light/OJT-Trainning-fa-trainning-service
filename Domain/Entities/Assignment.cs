using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Assignment : FileEntity
    {
        public string? AssignmentName { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// One assignmet must belong to one
        /// </summary>
        public Guid LectureID { get; set; }
        public virtual Lecture Lecture { get; set; }


        public DateTime DeadLine { get; set; }
        public bool IsOverDue { get; set; }


        //Set Relation to Assignment Submisstion
        public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }



    }
}
