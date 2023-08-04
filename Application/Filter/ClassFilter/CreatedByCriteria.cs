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
    public class CreatedByCriteria : ICriterias<TrainingClassFilterDTO>
    {
        public string? createdBy { get; set; } 
        public CreatedByCriteria(string? createdBy)
        {
            this.createdBy = createdBy;
        }
        public List<TrainingClassFilterDTO> MeetCriteria(List<TrainingClassFilterDTO> classList)
        {
            if (!createdBy.IsNullOrEmpty())
            {
                List<TrainingClassFilterDTO> trainingClassDTOs = new List<TrainingClassFilterDTO>();
                foreach (TrainingClassFilterDTO classDTO in classList)
                {
                    if (classDTO.CreatedBy.ToLower().Equals(createdBy.ToLower()))
                    {
                        trainingClassDTOs.Add(classDTO);
                    }
                }
                return trainingClassDTOs;
            }
            return classList;
        }
    }
}
