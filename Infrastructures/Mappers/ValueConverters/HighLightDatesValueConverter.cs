using AutoMapper;
using Domain.Entities.TrainingClassRelated;

namespace Infrastructures.Mappers.ValueConverters
{
    /// <summary>
    /// Convert DateTime list to HighlightDates list
    /// </summary>
    public class HighLightDatesValueConverter : IValueConverter<List<DateTime>, List<HighlightedDates>>
    {
        public List<HighlightedDates> Convert(List<DateTime> sourceMember, ResolutionContext context)
        {
            var mapped = new List<HighlightedDates>();
            sourceMember.ForEach(x =>
            {
                mapped.Add(new HighlightedDates() { HighlightedDate = x });
            });
            return mapped;
        }
    }
}
