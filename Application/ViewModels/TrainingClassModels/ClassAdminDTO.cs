using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public  class ClassAdminDTO
    {
        public Guid? adminID { get; set; }= Guid.NewGuid();
        public string name { get; set; } = "Admin 1";
        public string phone { get; set; } = "0902125467";
        public string email { get; set; } = "admin@fpt.edu.vn";
    }
}
