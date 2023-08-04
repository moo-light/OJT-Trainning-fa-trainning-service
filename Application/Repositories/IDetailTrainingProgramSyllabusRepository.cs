using Application.ViewModels.SyllabusModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IDetailTrainingProgramSyllabusRepository : IGenericRepository<DetailTrainingProgramSyllabus>
    {
        public Guid TakeDetailTrainingID(Guid user_id, Guid training_class_id);
        public List<SyllabusViewForTrainingClassDetail> GetDetailByClassID(Guid programID);
    }
}
