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
    public class LocationCriteria : ICriterias<TrainingClassFilterDTO>
    {
        public string[]? locationName;
        public LocationCriteria(string[]? locationName)
        {
            this.locationName = locationName;
        }

        public List<TrainingClassFilterDTO> MeetCriteria(List<TrainingClassFilterDTO> classList)
        {
            if (!locationName.IsNullOrEmpty())
            {
                List<TrainingClassFilterDTO> classData = new List<TrainingClassFilterDTO>();
                for (int i = 0; i <= locationName.Length; i++)
                {
                    foreach (TrainingClassFilterDTO item in classList)
                    {
                        if (locationName[i].ToLower().Equals(item.LocationName.ToLower()))
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
