using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class AdminsDTO
    {
        [JsonPropertyName("admin_id")]
        public Guid AdminId { get; set; }
    }
}

