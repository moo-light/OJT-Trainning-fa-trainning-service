using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public  class ClassTrainerDTO
    {
        public Guid trainerId { get; set; } 
        public string name { get; set; } 
        public string phone { get; set; } 
        public string email { get; set; } 
    }
}
