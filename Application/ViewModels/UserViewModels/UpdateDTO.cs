using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels
{
    public class UpdateRoleDTO
    {
        public Guid UserID { get; set; }
        public int RoleID { get; set; }
    }
    public class UpdateDTO 
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Level { get; set; }
    }
}
