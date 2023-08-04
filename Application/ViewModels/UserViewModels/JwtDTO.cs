using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels
{
    public class JwtDTO
    {
        public string UserId { get; set; } = default!;
        public string SyllabusPermission { get; set; } = default!;
        public string TrainingProgramPermission { get;set; } = default!;
        public string TrainingMaterialPermission { get; set; } = default!;
        public string ClassPermission { get;set; } = default!;
        public string LearningMaterial { get; set; } = default!;
        public string AttendancePermission { get; set; } = default!;
        public string UserPermission { get; set; } = default!;
        public string ApplicationPermission { get; set; } = default!;
    }
}
