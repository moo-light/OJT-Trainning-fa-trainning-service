using Application.Interfaces;
using Domain.Entities;

namespace Application.Filter.TrainingProgramFilter
{
    public class StatusCriteria : ICriterias<TrainingProgram>
    {
        private string? searchCriteria;
        public StatusCriteria(string? searchCriteria)
        {
            this.searchCriteria = searchCriteria;
        }
        public List<TrainingProgram> MeetCriteria(List<TrainingProgram> trainingPrograms)
        {
            if (searchCriteria != null)
            {
                List<TrainingProgram> trainingProgramData = new List<TrainingProgram>();
                foreach (TrainingProgram tp in trainingPrograms)
                {
                    if (tp.Status.ToLower().Equals(searchCriteria.ToLower()))
                    {
                        trainingProgramData.Add(tp);
                    }
                }
                return trainingProgramData;
            }
            else
                return trainingPrograms;
        }
    }
}
