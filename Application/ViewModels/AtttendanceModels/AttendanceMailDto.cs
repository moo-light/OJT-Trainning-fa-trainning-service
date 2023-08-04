using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.AtttendanceModels;

public class AttendanceMailDto
{
    public Guid? UserId { get; set; }
    public string Status { get; set; } = null!;
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime Date { get; set; }
    public int NumOfAbsented { get; set; } = 0;
    public Guid? TrainingClassId { get; set; }
    public string ClassName { get; set; }

}
