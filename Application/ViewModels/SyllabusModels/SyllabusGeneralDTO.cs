using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels
{
    public class SyllabusGeneralDTO
    {
        public string SyllabusName { get; set; }

        public string Code { get; set; } = "NPL";

        public string CourseObject { get; set; }

        public string TechRequirements { get; set; }

        public double Duration { get; set; }

        public string Level { get; set; }



        //public Guid UserId { get; set; }


    }
}
