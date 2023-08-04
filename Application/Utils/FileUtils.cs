using Domain.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public static class FileUtils
    {

        public static string ImportFile(this IFormFile file,string folder,int version,Guid userID)
        {
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net7.0", string.Empty));
            var folderName = Path.Combine("Resources", folder);
            var pathToSave = Path.Combine(dirName, folderName);
            var fileName = version + "_" + userID + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName); 
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return dbPath;//""
        }

        public static Dictionary<string, string> GetMimeTypes()
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

        public static FileEntity GetFileEntity(this string fileName)
        {
            FileEntity file = new FileEntity();
            if (!System.IO.File.Exists(fileName))
            {
                throw new Exception("File is not existed!");
            }
            file.FileName = fileName;
            file.FileType = System.IO.Path.GetExtension(file.FileName);
            file.FileData = System.IO.File.ReadAllBytes(fileName);
            return file;
        }

        public static MemoryStream? ExportExcel<T>(List<T> dataList)
        {
            var stream = new MemoryStream();

            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(dataList, true);
                package.Save();
            }
            stream.Position = 0;
            return stream;
        }

    }
}
