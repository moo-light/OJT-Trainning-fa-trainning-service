using Application.Interfaces;
using Application.ViewModels.TrainingClassModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Filter.ClassFilter
{
    public class AndClassFilter : ICriterias<TrainingClassFilterDTO>
    {
        private ICriterias<TrainingClassFilterDTO> firstCriterias;
        private ICriterias<TrainingClassFilterDTO> secondCriterias;
        private ICriterias<TrainingClassFilterDTO> thirdCriteria;
        private ICriterias<TrainingClassFilterDTO> fourthCriteria;
        private ICriterias<TrainingClassFilterDTO> fifthCriteria;
        private ICriterias<TrainingClassFilterDTO> sixthCriteria;
        public AndClassFilter(ICriterias<TrainingClassFilterDTO> firstCriteria, ICriterias<TrainingClassFilterDTO> secondCriteria, ICriterias<TrainingClassFilterDTO> thirdCriteria, ICriterias<TrainingClassFilterDTO> fourthCriteria, ICriterias<TrainingClassFilterDTO> fifthCriteria, ICriterias<TrainingClassFilterDTO> sixthCriteria)
        {
            this.firstCriterias = firstCriteria;
            this.secondCriterias = secondCriteria;
            this.thirdCriteria = thirdCriteria;
            this.fourthCriteria = fourthCriteria;
            this.fifthCriteria = fifthCriteria;
            this.sixthCriteria = sixthCriteria;
        }

        public List<TrainingClassFilterDTO> MeetCriteria(List<TrainingClassFilterDTO> classlist)
        {
            List<TrainingClassFilterDTO> firstResultList = firstCriterias.MeetCriteria(classlist);
            List<TrainingClassFilterDTO> secondResultList = secondCriterias.MeetCriteria(firstResultList);
            List<TrainingClassFilterDTO> thirdResultList = thirdCriteria.MeetCriteria(secondResultList);
            List<TrainingClassFilterDTO> fourthResultList = fourthCriteria.MeetCriteria(thirdResultList);
         List<TrainingClassFilterDTO> fifthResultList= fifthCriteria.MeetCriteria(fourthResultList);
            return sixthCriteria.MeetCriteria(fifthResultList);
        }
    }
}
