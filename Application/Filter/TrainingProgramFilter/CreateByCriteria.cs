using Application.Interfaces;
using Domain.Entities;

namespace Application.Filter.TrainingProgramFilter
{
    public class CreateByCriteria : ICriterias<TrainingProgram>
    {
        private List<Guid>? searchCriteria;
        public CreateByCriteria(List<Guid>? searchCriteria)
        {
            this.searchCriteria = searchCriteria;
        }
        public List<TrainingProgram> MeetCriteria(List<TrainingProgram> trainingPrograms)
        {
            List<TrainingProgram> trainingProgramData = new List<TrainingProgram>();
            if (searchCriteria.Count > 0)
            {
                foreach (var search in searchCriteria)
                {
                    if (search != Guid.Empty)
                    {
                        foreach (TrainingProgram tp in trainingPrograms)
                        {
                            if (tp.CreatedBy.Equals(search))
                            {
                                trainingProgramData.Add(tp);
                            }
                        }
                    }
                }
            }
            else
                trainingProgramData = trainingPrograms;
            return trainingProgramData;
        }
    }
}
