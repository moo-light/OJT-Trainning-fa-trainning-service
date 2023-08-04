using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TrainingMaterial : BaseEntity
    {
        public string? TMatName { get; set; }

        public string? TMatType { get; set; }

        public string? TMatDescription { get; set; }

        public string? TMatURL { get; set; }

        public Guid lectureID { get; set; }

        public Lecture Lecture { get; set; }

        public string BlobName { get; set; }
    }
}
