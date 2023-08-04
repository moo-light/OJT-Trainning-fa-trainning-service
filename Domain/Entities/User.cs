using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = null!;

        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }
        public string? Level { get; set; }
        //RefreshToken
        public string? RefreshToken { get; set; }

        public DateTime? ExpireTokenTime { get; set; }

        public DateTime LoginDate { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        public ICollection<Applications>? Applications { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }

        public ICollection<Syllabus> Syllabuses { get; set; }

        public ICollection<DetailTrainingClassParticipate> DetailTrainingClassParticipate { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; } = default!;

        public ICollection<SubmitQuiz> SubmitQuizzes { get; set; }
    }
}
