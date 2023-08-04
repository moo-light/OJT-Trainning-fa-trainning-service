using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public partial class TrainingClassViewModel : ExtendTrainingClassDTO
    {
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public DateTime? DeletionDate { get; set; }
        public string Branch { get; set; }

        public string _Id { get; set; }
        public string LocationID { get; set; } = default!;
        public string LocationName { get; set; } = default!;
    }
}
