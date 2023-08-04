using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public  class TrainingClassReview
    {
        public DateTime reviewDate { get; set; }= DateTime.UtcNow;
        public string author { get; set; } = "admin02";
    }
}
