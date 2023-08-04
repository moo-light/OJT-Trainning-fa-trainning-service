using Application.Interfaces;
using Application.Services;
using Application.ViewModels.TrainingMaterialModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Google.Apis.Util;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class TrainingMaterialTests:SetupTest
    {
        private readonly ITrainingMaterialService _trainingMaterialService;
        //private readonly Mock<IFormFile> _file;
        private readonly IFormFile _file;
        private readonly Guid id= Guid.NewGuid();   
        public TrainingMaterialTests()
        {
            _trainingMaterialService = new TrainingMaterialService(_unitOfWorkMock.Object, _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object);
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        }

        //GetFile
        [Fact]
        public async Task GetFile_ShouldReturnFileById()
        {
            var fileMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(fileMaterialMock);

            var actualResult=await _trainingMaterialService.GetFile(id);

            actualResult.Should().BeOfType<TrainingMaterial>();
        }//End GetFile
         //Upload
        /*[Fact] 
        public async Task Upload_ShouldReturnMaterial_WhenUpload() 
        {
            var memoryStream = _fixture.Build<MemoryStream>().Create();
            string stringMock = "a";
            Guid lectureIdMock = Guid.NewGuid();
            _file.Object.CopyToAsync(memoryStream).Wait();
            TrainingMaterial trainingMaterial = new TrainingMaterial
            {
                Id = id,
                TMatName = _file.Object.FileName,
                TMatType = System.IO.Path.GetExtension(_file.Object.FileName),
                TMatURL = stringMock,
                lectureID = lectureIdMock,
                BlobName = stringMock,
            };
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.AddAsync(trainingMaterial)).Verifiable();

            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.Upload(id, _file.Object, lectureIdMock, stringMock, stringMock);

            actualResult.Should().BeOfType<TrainingMaterial>();
        }*/
        [Fact]
        public async Task Upload_ShouldReturnMaterial_WhenUpload()
        {
            var memoryStream = new MemoryStream();
            string stringMock = "a";
            Guid lectureIdMock = Guid.NewGuid();
            _file.CopyToAsync(memoryStream).Wait();
            TrainingMaterial trainingMaterial = new TrainingMaterial
            {
                Id = id,
                TMatName = _file.FileName,
                TMatType = System.IO.Path.GetExtension(_file.FileName),
                TMatURL = stringMock,
                lectureID = lectureIdMock,
                BlobName = stringMock,
            };
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.AddAsync(trainingMaterial)).Verifiable();

            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.Upload(id, _file, lectureIdMock, stringMock, stringMock);

            actualResult.Should().BeOfType<TrainingMaterial>();
        }
        [Fact]
        public async Task Upload_ShouldReturnException_WhenUploadNull()
        {
            var memoryStream = new MemoryStream();
            string stringMock = "a";
            Guid lectureIdMock = Guid.NewGuid();
            TrainingMaterial trainingMaterial = new TrainingMaterial();
           /* _file.CopyToAsync(memoryStream).Wait();
            TrainingMaterial trainingMaterial = new TrainingMaterial
            {
                Id = id,
                TMatName = _file.FileName,
                TMatType = System.IO.Path.GetExtension(_file.FileName),
                TMatURL = stringMock,
                lectureID = lectureIdMock,
                BlobName = stringMock,
            };*/
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.AddAsync(trainingMaterial)).Verifiable();

            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            Func<Task> actualResult = async()=> await _trainingMaterialService.Upload(id, _file, lectureIdMock, stringMock, stringMock);

            actualResult.Should().ThrowAsync<Exception>().WithMessage("File not exist");
        }
        //EndUpload

        //Delete
        /*[Fact]
        public async Task DeleteTrainingMaterial_ShouldReturnBoolisDeleteiisTrue()
        {
            bool isDelete = true;
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.DeleteTrainingMaterial(blobName)).ReturnsAsync(isDelete);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.DeleteTrainingMaterial(blobName);

            actualResult.Should().BeTrue();
        }*/

        /*[Fact]
        public async Task DeleteTrainingMaterial_ShouldReturnBoolisDeleteisFalse()
        {
            bool isDelete = false;
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.DeleteTrainingMaterial(id)).ReturnsAsync(isDelete);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.DeleteTrainingMaterial(id);

            actualResult.Should().BeFalse();
        }*/
        //End Delete

        //UpdateTrainingMaterial 
        [Fact]
        public async Task UpdateTrainingMaterial_ShouldReturnisUpdateisTrue_WhenfileisNotNull()
        {
            var memoryStream = new MemoryStream();
            string stringMock = "a";
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x=>x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock);
            _file.CopyToAsync(memoryStream).Wait();
            findTrainingMaterialMock.TMatName = _file.FileName;
            findTrainingMaterialMock.TMatType = System.IO.Path.GetExtension(_file.FileName);
            findTrainingMaterialMock.TMatURL = stringMock;
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.Update(findTrainingMaterialMock)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.UpdateTrainingMaterial(_file, id, stringMock);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateTrainingMaterial_ShouldReturnisUpdateisFalseandException_WhenfileisNull()
        {
            var memoryStream = new MemoryStream();
            string stringMock = "a";
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock=null);
            /*_file.CopyToAsync(memoryStream).Wait();
            findTrainingMaterialMock.TMatName = _file.FileName;
            findTrainingMaterialMock.TMatType = System.IO.Path.GetExtension(_file.FileName);
            findTrainingMaterialMock.TMatURL = stringMock;*/
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.Update(findTrainingMaterialMock)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();

            var actualResult = await _trainingMaterialService.UpdateTrainingMaterial(_file, id, stringMock);
            Func<Task> actualResultThrowExcep = async () => await _trainingMaterialService.UpdateTrainingMaterial(_file, id, stringMock);

            actualResult.Should().BeFalse();
            
            //actualResultThrowExcep().Should().ThrowA;
        }
        //End UpdateTrainingMaterial

        //GetBlobNameWithLectureId(
        [Fact]
        public async Task GetBlobNameWithLectureId_ShouldReturnBlobName()
        {
            var listTMatMock= _fixture.Build<TrainingMaterial>()
                                        .Without(x=>x.Lecture)
                                        .CreateMany(1).ToList();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetAllFileWithLectureId(id)).ReturnsAsync(listTMatMock);            List<string> blobName = new List<string>();
            List<string> blobNames = new List<string>();
            foreach(TrainingMaterial trainingMaterial in listTMatMock)
            {
                blobName.Add(trainingMaterial.BlobName);    
            }

            var actualResilt = await _trainingMaterialService.GetBlobNameWithLectureId(id);

            actualResilt.Should().BeOfType<List<string>>();
        }//End GetBlobNameWithLectureId(

        
        [Fact]
        public async Task GetBlobNameWithTMatId_ShouldReturnBlobName_WhenFindNaterialisNotNull()
        {
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x=>x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock);

            //string blobName = findTrainingMaterialMock.BlobName;
            var actualResult= await _trainingMaterialService.GetBlobNameWithTMatId(id);

            actualResult.Should().BeOfType<string>();
        }

        [Fact]
        public async Task GetBlobNameWithTMatId_ShouldReturnException_WhenFindNaterialisNull()
        {
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock=null);

            //string blobName = findTrainingMaterialMock.BlobName;
            Func<Task> actualResult =async()=>await _trainingMaterialService.GetBlobNameWithTMatId(id);

            await actualResult.Should().ThrowAsync<Exception>().WithMessage("File does not exist");
        }

        [Fact]
        public async Task GetFileNameWithTMatId_ShouldReturnBlobName_WhenFindNaterialisNotNull()
        {
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock);
          
            var actualResult = await _trainingMaterialService.GetFileNameWithTMatId(id);

            actualResult.Should().BeOfType<string>();
        }

        [Fact]
        public async Task GetFileNameWithTMatId_ShouldReturnException_WhenFindNaterialisNull()
        {
            var findTrainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(findTrainingMaterialMock = null);

            Func<Task> actualResult = async () => await _trainingMaterialService.GetFileNameWithTMatId(id);

            await actualResult.Should().ThrowAsync<Exception>().WithMessage("File does not exist");
        }

        [Fact]
        public async Task GetTrainingMaterial_ShouldReturnBlobName_WhentrainingMaterialDTOisNotNull()
        {
            var findTrainingMaterialDTOMock = _fixture.Build<TrainingMaterialDTO>().Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetTrainingMaterial(id)).ReturnsAsync(findTrainingMaterialDTOMock);

            //string blobName = findTrainingMaterialMock.BlobName;
            var actualResult = await _trainingMaterialService.GetTrainingMaterial(id);

            actualResult.Should().BeOfType<TrainingMaterialDTO>();
        }

        [Fact]
        public async Task GetTrainingMaterial_ShouldReturnBlobName_WhentrainingMaterialDTOisNull()
        {
            var findTrainingMaterialDTOMock = _fixture.Build<TrainingMaterialDTO>().Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetTrainingMaterial(id)).ReturnsAsync(findTrainingMaterialDTOMock=null);

            //string blobName = findTrainingMaterialMock.BlobName;
            Func<Task> actualResult =async()=> await _trainingMaterialService.GetTrainingMaterial(id);

            actualResult.Should().ThrowAsync<Exception>().WithMessage("File does not exist");
        }

        [Fact]
        public async Task SoftRemoveTrainingMaterial_ShouldReturnTrue_WhenSaveChangeis1()
        {
            var trainingMaterialMock= _fixture.Build<TrainingMaterial>().Without(x=>x.Lecture).Create();
            _unitOfWorkMock.Setup(x=>x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(trainingMaterialMock);
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.SoftRemove(trainingMaterialMock)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var actualReesult= await _trainingMaterialService.SoftRemoveTrainingMaterial(id);

            actualReesult.Should().BeTrue();
        }

        [Fact]
        public async Task SoftRemoveTrainingMaterial_ShouldReturnFalse_WhenSaveChangeis0()
        {
            var trainingMaterialMock = _fixture.Build<TrainingMaterial>().Without(x => x.Lecture).Create();
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.GetByIdAsync(id)).ReturnsAsync(trainingMaterialMock=null);
            _unitOfWorkMock.Setup(x => x.TrainingMaterialRepository.SoftRemove(trainingMaterialMock)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            var actualReesult = await _trainingMaterialService.SoftRemoveTrainingMaterial(id);

            actualReesult.Should().BeFalse();
        }

    }
}
