using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string SyllabusPermission { get; set; }

        public string TrainingProgramPermission { get; set; }

        public string ClassPermission { get; set; }

        public string LearningMaterial { get; set; }

        public string UserPermission { get; set; }

        public string AttendancePermission { get; set; }

        public string TrainingMaterialPermission { get; set; }

        public string ApplicationPermission { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
