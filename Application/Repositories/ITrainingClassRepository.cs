using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ITrainingClassRepository : IGenericRepository<TrainingClass>
    {
        public List<TrainingClassViewAllDTO> SearchClassByName(string name);
        public List<TrainingClassFilterDTO> GetTrainingClassesForFilter();
        public List<TrainingClassViewAllDTO> GetTrainingClasses();
        TrainingProgramViewForTrainingClassDetail GetTrainingProgramByClassID(Guid id);
       TrainingClassFilterDTO GetTrainingClassForViewDetailById(Guid id);
        public Task<TrainingClass?> GetByIdAsync(Guid id);
        public new Task AddAsync(TrainingClass trainingClass);
        public void Update(TrainingClass trainingClass);
    }
}
