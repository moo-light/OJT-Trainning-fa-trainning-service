using Application.Interfaces;
using Application.ViewModels.TrainingClassModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Filter.ClassFilter
{
    public class DateCriteria : ICriterias<TrainingClassFilterDTO>
    {
        public DateTime? date1;
        public DateTime? date2;
        public DateCriteria(DateTime? date1, DateTime? date2)
        {
            this.date1 = date1;
            this.date2 = date2;
        }

        public List<TrainingClassFilterDTO> MeetCriteria(List<TrainingClassFilterDTO> classList)
        {
            if (date1 != null && date2 != null)
            {
                List<TrainingClassFilterDTO> trainingClassDTOs = new List<TrainingClassFilterDTO>();
                foreach (TrainingClassFilterDTO item in classList)
                {
                    if (date1 <= item.StartDate && item.EndDate <= date2)
                    {
                        trainingClassDTOs.Add(item);
                    }

                }
                return trainingClassDTOs;
            }
            else if (date1 != null && date2 == null)
            {
                List<TrainingClassFilterDTO> trainingClassDTOs = new List<TrainingClassFilterDTO>();
                foreach (TrainingClassFilterDTO item in classList)
                {
                    if (date1 <= item.StartDate)
                    {
                        trainingClassDTOs.Add(item);
                    }

                }
                return trainingClassDTOs;
            }
            else if (date1 == null && date2 != null)
            {
                List<TrainingClassFilterDTO> trainingClassDTOs = new List<TrainingClassFilterDTO>();
                foreach (TrainingClassFilterDTO item in classList)
                {
                    if (item.EndDate <= date2)
                    {
                        trainingClassDTOs.Add(item);
                    }

                }
                return trainingClassDTOs;
            }
            return classList;
        }
    }
}
