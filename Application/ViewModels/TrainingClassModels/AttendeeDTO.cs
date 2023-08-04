using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public class AttendeeDTO
    {
        public string attendee { get; set; }
        public int plannedNumber { get; set; }
        public int acceptedNumber { get; set; }
        public int actualNumber { get; set; }
    }
}
