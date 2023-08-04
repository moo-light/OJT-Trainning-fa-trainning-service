using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public class TrainingClassFilterModel
    {

        public string[]? locationName { get; set; }
        public string? branchName { get; set; }

        public DateTime? date1 { get; set; }

        public DateTime? date2 { get; set; }
        public string[]? classStatus { get; set; }
        public string[]? attendInClass { get; set; }
        public string[]? classTime { get; set; }    
        public string? trainer { get; set; }
    }
}
