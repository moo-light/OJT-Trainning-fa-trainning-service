using Application.Interfaces;
using Domain.Entities;

namespace Application.Filter.TrainingProgramFilter
{
    public class AndTrainingProgramCriteria : ICriterias<TrainingProgram>
    {
        private ICriterias<TrainingProgram> criteria;
        private ICriterias<TrainingProgram> otherCriteria;

        public AndTrainingProgramCriteria(ICriterias<TrainingProgram> criteria, ICriterias<TrainingProgram> otherCriteria)
        {
            this.criteria = criteria;
            this.otherCriteria = otherCriteria;
        }

        public List<TrainingProgram> MeetCriteria(List<TrainingProgram> trainingPrograms)
        {
            List<TrainingProgram> firstCriteriaTrainingPrograms = criteria.MeetCriteria(trainingPrograms);
            return otherCriteria.MeetCriteria(firstCriteriaTrainingPrograms);
        }
    }
}
