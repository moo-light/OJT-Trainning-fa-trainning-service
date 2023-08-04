using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingMaterialModels
{
    public class TrainingMaterialDTO
    {
        public string blobName { get; set; } = null!;
        public string createdBy { get; set; } = null!;
        public DateTime createdOn { get; set; }
    }
}
