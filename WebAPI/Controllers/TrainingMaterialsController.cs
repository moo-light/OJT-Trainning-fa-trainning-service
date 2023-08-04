using Application.Interfaces;
using Application.Utils;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.FileProviders;
using System.Reflection.Metadata;
using Domain.Entities;
using Application.Services;
using static System.Reflection.Metadata.BlobBuilder;
using Azure;
using System.Linq;

namespace WebAPI.Controllers
{
    public class TrainingMaterialsController : BaseController
    {
        private readonly ITrainingMaterialService _trainingMaterialService;

        /*[HttpPost("UploadFile")]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> Upload(IFormFile file, Guid lectureId)
        {
            await _trainingMaterialService.Upload(file, lectureId);
            if (file == null)
            {
                return NotFound();
            }
            return Ok();
        }*/

        /*[HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteTrainingMaterial(string blobName)
        {
            bool deleteTraingMaterial = await _trainingMaterialService.DeleteTrainingMaterial(blobName);
            if (deleteTraingMaterial)
            {
                return Ok();
            }
            return BadRequest();
        }*/

        /*[HttpPut("EditFile")]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> UpdateTrainingMaterial(IFormFile file, Guid id)
        {
            bool updateTrainingMaterial = await _trainingMaterialService.UpdateTrainingMaterial(file, id);
            if (updateTrainingMaterial)
            {
                return Ok();
            }
            return BadRequest();
        }*/

        //===========================================
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public TrainingMaterialsController(ITrainingMaterialService trainingMaterialService)
        {
            // Replace the connection string and container name with your own values
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=storefilefatraining;AccountKey=V4fnfSO/yI5wyXQZNmUWNIdjcZhsI0C6Btwl7anlJbFBnPDmZa8q7vLv9PBvpf8ev0pqbhINBEHC+ASttAIwGw==;EndpointSuffix=core.windows.net";
            _containerName = "rootcontainer";
            // Create a BlobServiceClient object using the connection string
            _blobServiceClient = new BlobServiceClient(connectionString);
            _trainingMaterialService = trainingMaterialService;

        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> UploadTestAzure(IFormFile file, Guid lectureId)
        {
            // Verify that the file was provided
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please provide a file");
            }

            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            Guid id = Guid.NewGuid();
            var blobName = id.ToString() /*+ "." + file.FileName*/+ Path.GetExtension(file.FileName);
            //var blobName = "a5688551-bfdf-48e0-b440-e169d3f4321d.noelle.jpg";

            // Upload the file to the container
            var blobClient = containerClient.GetBlobClient(blobName);
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // Return the URL of the uploaded blob
            var blobUrl = blobClient.Uri.AbsoluteUri;

            // Upload to database
            await _trainingMaterialService.Upload(id, file, lectureId, blobUrl, blobName);

            return Ok(blobUrl);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadTestAzure(Guid id)
        {
            string blobName = await _trainingMaterialService.GetBlobNameWithTMatId(id);
            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(blobName);

            // Download the blob to a stream
            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);

            // Get FileName
            var result = await _trainingMaterialService.GetFileNameWithTMatId(id);
            /*// Get name without guid
            var result = blobName.Substring(blobName.IndexOf('.') + 1);*/

            // Reset the stream position to the beginning
            stream.Position = 0;

            // Get the properties of the blob
            var response = await blobClient.GetPropertiesAsync();

            // Return the file as a stream with the correct content type

            return File(stream, response.Value.ContentType, result);
        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> EditTestAzure(Guid id, IFormFile file)
        {
            string blobName = await _trainingMaterialService.GetBlobNameWithTMatId(id);

            // Verify that the file was provided
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please provide a file");
            }

            if (blobName == null)
            {
                return BadRequest("Please provide a file to edit");
            }

            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            // Return the URL of the uploaded blob
            var blobUrl = blobClient.Uri.AbsoluteUri;

            // Edit database
            bool updateTrainingMaterial = await _trainingMaterialService.UpdateTrainingMaterial(file, id, blobUrl);
            if (!updateTrainingMaterial)
            {
                return BadRequest();
            }

            // Upload the file to the container
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return Ok(blobUrl);
        }

        [HttpDelete]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> RemoveBlob(Guid id)
        {
            string blobName = await _trainingMaterialService.GetBlobNameWithTMatId(id);

            // Delete from database
            bool removeTraingMaterial = await _trainingMaterialService.SoftRemoveTrainingMaterial(id);
            if (!removeTraingMaterial)
            {
                return BadRequest();
            }

            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteAsync();

            return Ok();
        }

        // Restore files on Azure, not database :>>>>
        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.TrainingMaterialPermission), nameof(PermissionEnum.Create))]
        public void RestoreBlobsWithVersioning(string blobName)
        {
            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(blobName);
            // List blobs in this container that match prefix.
            // Include versions in listing.
            Pageable<BlobItem> blobItems = containerClient.GetBlobs
                            (BlobTraits.None, BlobStates.Version, prefix: blobClient.Name);

            // Get the URI for the most recent version.
            BlobUriBuilder blobVersionUri = new BlobUriBuilder(blobClient.Uri)
            {
                VersionId = blobItems
                            .OrderByDescending(version => version.VersionId)
                            .ElementAtOrDefault(0)?.VersionId
            };

            // Restore the most recently generated version by copying it to the base blob.
            blobClient.StartCopyFromUri(blobVersionUri.ToUri());
        }

        /*[HttpPost]
        public async Task<IActionResult> CheckFilesEveryday()
        {
            List<string> list = await _trainingMaterialService.CheckDeleted();

            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            List<string> blobNames = new List<string>();

            bool deleteTraingMaterial = false;

            var blobHierarchyItems = containerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/");

            await foreach (var blobHierarchyItem in blobHierarchyItems)
            {
                //check if the blob is a virtual directory.
                if (blobHierarchyItem.IsPrefix)
                {
                    // You can also access files under nested folders in this way,
                    // of course you will need to create a function accordingly (you can do a recursive function)
                    // var prefix = blobHierarchyItem.Name;
                    // blobHierarchyItem.Name = "folderA\"
                    // var blobHierarchyItems= container.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", prefix);     
                }
                else
                {
                    blobNames.Add(blobHierarchyItem.Blob.Name);
                }
            }

            var listDelete = list.Except(blobNames).ToList();

            foreach(var delete in listDelete)
            {
                deleteTraingMaterial = await _trainingMaterialService.DeleteTrainingMaterial(delete);
                
            }

            if (deleteTraingMaterial)
            {
                return Ok();
            }
            return BadRequest();
        }*/
    }
}