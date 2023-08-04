using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class GradingReport : BaseEntity
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; }
    public Guid TraineeId { get; set; }
    public string Username { get; set; }
    public string TraineeName { get; set; }
    public string LectureName { get; set; }
    public string DeliveryType { get; set; }
    public double NumericGrade { get; set; }
    public string LetterGrade { get; set; }
}
