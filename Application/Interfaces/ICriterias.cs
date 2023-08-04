namespace Application.Interfaces
{
    public interface ICriterias<T>
    {
        List<T> MeetCriteria(List<T> TEntity);
    }
}
