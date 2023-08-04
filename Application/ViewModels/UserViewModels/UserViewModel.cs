using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.UserViewModels
{
    public class UserViewModel
    {
        public string _Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Level { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public Role Role { get; set; }
        public DateTime LoginDate { get; set; }

        public ICollection<Syllabus> Syllabuses { get; set; }
    }
}
