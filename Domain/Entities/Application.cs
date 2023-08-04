namespace Domain.Entities
{
    public class Applications : BaseEntity
    {
        public DateTime AbsentDateRequested { get; set; }

        public string? Reason { get; set; }

        public bool Approved { get; set; }

        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public Guid? TrainingClassId { get; set; }

        public TrainingClass? TrainingClass { get; set; }

    }
}