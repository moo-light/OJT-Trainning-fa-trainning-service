using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    internal class CreateDetailTrainingClassDTO
    {
        public Guid UserId { get; set; }
        public Guid TrainingClassID { get; set; }
        public string StatusClassDetail { get; set; }


    }
}
