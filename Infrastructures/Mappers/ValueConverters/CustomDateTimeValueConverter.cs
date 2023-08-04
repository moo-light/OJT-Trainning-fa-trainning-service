using AutoMapper;
using Domain.Entities.TrainingClassRelated;

namespace Infrastructures.Mappers.ValueConverters
{
    /// <summary>
    /// Convert a HighlightDates list to a DateTime list
    /// </summary>
    public class CustomDateTimeValueConverter : IValueConverter<List<HighlightedDates>, List<DateTime>>
    {
        public List<DateTime> Convert(List<HighlightedDates> sourceMember, ResolutionContext context)
        {
            var mapped = new List<DateTime>();
            sourceMember.ForEach(x =>
            {
                mapped.Add(x.HighlightedDate);
            });
            return mapped;
        }
    }
}
