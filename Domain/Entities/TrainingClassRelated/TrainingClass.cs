using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Training class
    /// </summary>
    public partial class TrainingClass : BaseEntity
    {
        public string Name { get; set; } = null!;

        public DateTime StartTime { get; set; } = default!;

        public DateTime EndTime { get; set; }=default!;

        public string Code { get; set; }

        public double Duration { get; set; }


        public string? Attendee { get; set; }

        public string? Branch { get; set; }

        public Guid? LocationID { get; set; }

        public Location? Location { get; set; }

        public ICollection<DetailTrainingClassParticipate> TrainingClassParticipates { get; set; } = default!;

        public ICollection<Attendance> Attendances { get; set; } = default!;

        public ICollection<Feedback> Feedbacks { get; set; } = default!;

        public string StatusClassDetail { get; set; } = default!;

        public Guid TrainingProgramId { get; set; }

        public TrainingProgram TrainingProgram { get; set; } = default!;

        public ICollection<Applications> Applications { get; set; }

        public ICollection<ClassSchedule> ClassSchedules { get; set; }

    }
    public partial class TrainingClass
    {
        public string? Fsu { get; set; } = default!;
        public TrainingClassAttendees TrainingClassAttendee { get; set; } = default!;
        public TrainingClassTimeFrame TrainingClassTimeFrame { get; set; } = default!;
        public ICollection<TrainingClassTrainer> TrainingClassTrainers { get; set; }
        public ICollection<TrainingClassAdmin> TrainingClassAdmins { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ReviewAuthor { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? ApproveAuthor { get; set; }
    }
}
