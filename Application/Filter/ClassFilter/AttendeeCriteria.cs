using Application.Interfaces;
using Application.ViewModels.TrainingClassModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Filter.ClassFilter
{
    public class AttendeeCriteria : ICriterias<TrainingClassFilterDTO>
    {
        public string[]? attendInClass;
        public AttendeeCriteria(string[]? attendInClass)
        {
            this.attendInClass = attendInClass;
        }

        public List<TrainingClassFilterDTO> MeetCriteria(List<TrainingClassFilterDTO> classList)
        {

            if (!attendInClass.IsNullOrEmpty())
            {
                List<TrainingClassFilterDTO> classData = new List<TrainingClassFilterDTO>();
                for (int i = 0; i <= attendInClass.Length; i++)
                {
                    foreach (TrainingClassFilterDTO item in classList)
                    {
                        if (attendInClass[i].ToLower().Equals(item.Attendee.ToLower()))
                        {
                            classData.Add(item);
                        }

                    }
                    return classData;
                }
            }
            return classList;
        }
    }
}
