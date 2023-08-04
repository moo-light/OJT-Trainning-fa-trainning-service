using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public partial class CreateTrainingClassDTO2 : ExtendTrainingClassDTO2
    {
        [JsonPropertyName("class_name")]
        public string ClassName { get; set; } = default!;
        [JsonPropertyName("class_code")]
        public string ClassCode { get; set; }
        [JsonPropertyName("class_status")]
        public string ClassStatus { get; set; }
    }
}

