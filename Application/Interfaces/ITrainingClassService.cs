using Application.ViewModels.TrainingClassModels;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITrainingClassService
    {
        public Task<List<TrainingClassViewAllDTO>> SearchClassByNameAsync(string name);
        public Task<bool> DuplicateClassAsync(Guid id);
        public Task<bool> SoftRemoveTrainingClassAsync(string traingingClassId);
        public Task<bool> UpdateTrainingClassAsync(string trainingClassId, UpdateTrainingClassDTO updateTrainingCLassDTO);
        public Task<TrainingClass> GetTrainingClassByIdAsync(string trainingClassId);
        public Task<TrainingClassViewModel?> CreateTrainingClassAsync(CreateTrainingClassDTO createTrainingClassDTO);
        public Task<List<TrainingClassViewAllDTO>> GetAllTrainingClassesAsync();
        public Task<List<TrainingClassViewAllDTO>> FilterLocation(string[]? locationName, string branchName, DateTime? date1, DateTime? date2, string[]? classStatus, string[]? attendInClass,string trainerName);
        public Task<FinalTrainingClassDTO> GetFinalTrainingClassesAsync(Guid id);
        public Task<bool> CheckTrainingClassAdminsIdAsync(TrainingClassAdmin user);
        public Task<bool> CheckTrainingClassTrainersIdAsync(TrainingClassTrainer user);
        public Task<List<TrainingClass>> ImportExcel(IFormFile file);
    }
}
