using Application.ViewModels.TrainingMaterialModels;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITrainingMaterialService
    {
        public Task<TrainingMaterial> GetFile(Guid id);
        public Task<TrainingMaterial> Upload(Guid id, IFormFile file, Guid lectureId, string blobUrl, string blobName);
        public Dictionary<string, string> GetMimeTypes();
        public Task<bool> DeleteTrainingMaterial(string blobName);
        public Task<bool> UpdateTrainingMaterial(IFormFile file, Guid id, string blobUrl);
        public Task<List<string>> GetBlobNameWithLectureId(Guid lectureId);
        public Task<string> GetBlobNameWithTMatId(Guid id);
        public Task<string> GetFileNameWithTMatId(Guid id);
        public Task<TrainingMaterialDTO> GetTrainingMaterial(Guid lectureId);
        public Task<bool> SoftRemoveTrainingMaterial(Guid TMatId);
        public Task<List<string>> CheckDeleted();
    }
}
