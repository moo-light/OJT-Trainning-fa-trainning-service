using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.TrainingMaterialModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Application.Services
{
    public class TrainingMaterialService : ITrainingMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly AppConfiguration _configuration;

        public TrainingMaterialService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, AppConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _configuration = configuration;
        }

        public async Task<TrainingMaterial> GetFile(Guid id)
        {
            var file = await _unitOfWork.TrainingMaterialRepository.GetByIdAsync(id);
            return file;
        }

        public async Task<TrainingMaterial> Upload(Guid id, IFormFile file, Guid lectureId, string blobUrl, string blobName)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var material = new TrainingMaterial();
                if (memoryStream.Length > 0)
                {
                    material = new TrainingMaterial
                    {
                        Id = id,
                        TMatName = file.FileName,
                        TMatType = System.IO.Path.GetExtension(file.FileName),
                        TMatURL = blobUrl,
                        lectureID = lectureId,
                        BlobName = blobName,
                    };

                    await _unitOfWork.TrainingMaterialRepository.AddAsync(material);
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    throw new Exception("File not existed");
                }
                return material;
            }
        }
        // get type of the upload file
        public Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".xlsx", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".rar", "application/vnd.rar"},
                {".zip", "application/zip"},
                {".mp3", "audio/mp3"},
                {".mp4", "video/mp4"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
            };
        }

        public async Task<bool> DeleteTrainingMaterial(string blobName)
        {
            await _unitOfWork.TrainingMaterialRepository.DeleteTrainingMaterial(blobName);
            return await _unitOfWork.SaveChangeAsync()>0;         
        }

        public async Task<bool> UpdateTrainingMaterial(IFormFile file, Guid id, string blobUrl)
        {
            TrainingMaterial findTrainingMaterial = await _unitOfWork.TrainingMaterialRepository.GetByIdAsync(id);
            bool isUpdated = false;
            if (findTrainingMaterial != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    if (memoryStream.Length > 0)
                    {

                        findTrainingMaterial.TMatName = file.FileName;
                        findTrainingMaterial.TMatType = System.IO.Path.GetExtension(file.FileName);
                        findTrainingMaterial.TMatURL = blobUrl;
                     /* findTrainingMaterial.lectureID = findTrainingMaterial.lectureID;

                        findTrainingMaterial.BlobName = blobName;*/


                        _unitOfWork.TrainingMaterialRepository.Update(findTrainingMaterial);
                        await _unitOfWork.SaveChangeAsync();
                        isUpdated = true;
                    }
                    else
                    {
                        throw new Exception("File not exist");
                        isUpdated = false;
                    }

                }

            }
            return isUpdated;
        }

        public async Task<List<string>> GetBlobNameWithLectureId(Guid lectureId)
        {
            List<TrainingMaterial> listTMat = await _unitOfWork.TrainingMaterialRepository.GetAllFileWithLectureId(lectureId);
            List<string> blobName = new List<string>();
            foreach (TrainingMaterial trainingMaterial in listTMat)
            {         
                blobName.Add(trainingMaterial.BlobName);
            }

            return blobName;
        }

        public async Task<string> GetBlobNameWithTMatId(Guid id)
        {
            TrainingMaterial findTrainingMaterial = await _unitOfWork.TrainingMaterialRepository.GetByIdAsync(id);
            if(findTrainingMaterial != null)
            {
                var blobName = findTrainingMaterial.BlobName;
                return blobName;
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public async Task<string> GetFileNameWithTMatId(Guid id)
        {
            TrainingMaterial findTrainingMaterial = await _unitOfWork.TrainingMaterialRepository.GetByIdAsync(id);
            if (findTrainingMaterial != null)
            {
                return findTrainingMaterial.TMatName;
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public async Task<TrainingMaterialDTO> GetTrainingMaterial(Guid lectureId)
        {
            var trainingMaterialDTO = await _unitOfWork.TrainingMaterialRepository.GetTrainingMaterial(lectureId);
            if (trainingMaterialDTO != null)
            {
                return trainingMaterialDTO;
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public async Task<List<string>> CheckDeleted()
        {
            var listTrainingMaterial = await _unitOfWork.TrainingMaterialRepository.GetAllDeletedTrainingMaterialNames();
            return listTrainingMaterial;
        }

        public async Task<bool> SoftRemoveTrainingMaterial(Guid TMatId)
        {
            TrainingMaterial trainingMaterial = await _unitOfWork.TrainingMaterialRepository.GetByIdAsync(TMatId);
            _unitOfWork.TrainingMaterialRepository.SoftRemove(trainingMaterial);
            return (await _unitOfWork.SaveChangeAsync() > 0);
        }
    }
}
