using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Utils;
using Application.ViewModels.AtttendanceModels;
using Application.ViewModels.GradingModels;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commons;

public class ApplicationCronJob
{
    private readonly IConfiguration _configuration;
    private readonly IAttendanceService _attendanceService;
    private readonly ISendMailHelper _mailHelper;
    private readonly ICurrentTime _currentTime;
    private readonly IGradingService _gradingService;
    private readonly ITrainingMaterialService _trainingMaterialService;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public ApplicationCronJob(IConfiguration configuration, ICurrentTime currentTime,
        ISendMailHelper mailHelper,
        IAttendanceService attendanceService, IGradingService gradingService, ITrainingMaterialService trainingMaterialService)
    {
        _configuration = configuration;
        _attendanceService = attendanceService;
        _mailHelper = mailHelper;
        _currentTime = currentTime;
        _gradingService = gradingService;
        _trainingMaterialService = trainingMaterialService;

        // Replace the connection string and container name with your own values
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=storefilefatraining;AccountKey=V4fnfSO/yI5wyXQZNmUWNIdjcZhsI0C6Btwl7anlJbFBnPDmZa8q7vLv9PBvpf8ev0pqbhINBEHC+ASttAIwGw==;EndpointSuffix=core.windows.net";
        _containerName = "rootcontainer";
        // Create a BlobServiceClient object using the connection string
        _blobServiceClient = new BlobServiceClient(connectionString);
        _trainingMaterialService = trainingMaterialService;
    }

    public async Task CheckAttendancesEveryDay()
    {
        // get all attendances info of today
        var absentList = await _attendanceService.GetAllAbsentInfoAsync();
        // send email to these trainee
        foreach (var x in absentList)
        {
            var subject = "Confirm absence";
            //Get project's directory and fetch AbsentTemplate content from EmailTemplates
            string exePath = Environment.CurrentDirectory.ToString();
            if (exePath.Contains(@"\bin\Debug\net7.0"))
                exePath = exePath.Remove(exePath.Length - (@"\bin\Debug\net7.0").Length);
            string FilePath = exePath + @"\EmailTemplates\AbsentTemplate.html";
            StreamReader streamreader = new StreamReader(FilePath);
            string MailText = streamreader.ReadToEnd();
            streamreader.Close();
            //Replace [TraineeName] = email/fullname
            if (!x.FullName.IsNullOrEmpty())
                MailText = MailText.Replace("[TraineeName]", x.Email);
            else
                MailText = MailText.Replace("[TraineeName]", x.FullName);
            //Replace [resetpasswordkey] = current date
            MailText = MailText.Replace("[TodayDate]", _currentTime.GetCurrentTime().Date.ToString("dd/MM/yyyy"));
            //Replace [ClassName] = class name 
            MailText = MailText.Replace("[ClassName]", x.ClassName);
            //Replace [NumOfAbsented] = numOfAbsented
            MailText = MailText.Replace("[NumOfAbsented]", x.NumOfAbsented.ToString());
            await _mailHelper.SendMailAsync(x.Email, subject, MailText);
        }
    }

    public async Task ExtractGradingDataEveryDay()
    {
        await _gradingService.UpdateGradingReports();
    }

    public async Task CheckFilesEveryday()
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

        foreach (var delete in listDelete)
        {
            deleteTraingMaterial = await _trainingMaterialService.DeleteTrainingMaterial(delete);
        }     
    }
}

