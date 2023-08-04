
namespace Application.ViewModels.AtttendanceModels;

public class AttendanceDTO
{
    private DateTime _date;

    public bool Status { get; set; }
    public DateTime Date { get => _date.Date; set => _date = value; }
    public Guid? UserId { get; set; }
    public Guid? AttendanceId { get; set; } = Guid.Empty;
    //public  Guid UserGuid => Guid.Parse(UserId); 
}