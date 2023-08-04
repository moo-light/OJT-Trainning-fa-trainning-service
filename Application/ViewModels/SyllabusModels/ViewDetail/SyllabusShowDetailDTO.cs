using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.ViewDetail
{
    public class SyllabusShowDetailDTO
    {
        public SyllabusGeneralDTO SyllabusBase { get; set; }

        public List<UnitDetailDTO> Units { get; set; }
    }
}
