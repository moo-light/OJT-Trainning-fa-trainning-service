using Application.ViewModels.ApplicationViewModels;
using Application.ViewModels.UserViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AtttendanceModels
{
    public class AttendanceViewDTO
    {
        public Guid? Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string DateOfBirth { get; set; }
        public Guid? ApplicationId { get; set; }
        public string ApplicationReason { get; set; }
        public Guid? TrainingClassId { get; set; }
        public string ClassName { get; set; }

    }
}
