using AutoMapper;
using Domain.Entities.TrainingClassRelated;

namespace Infrastructures.Mappers.TypeConverter
{
    /// <summary>
    /// Convert HighlightDates object to DateTime
    /// </summary>
    public class CustomDateTimeTypeConverter : ITypeConverter<HighlightedDates, DateTime>
    {
        public DateTime Convert(HighlightedDates source, DateTime destination, ResolutionContext context)
        {
            return source.HighlightedDate;
        }
    }
}
