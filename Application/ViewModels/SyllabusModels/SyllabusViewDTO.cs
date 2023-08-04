using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels
{
    public class SyllabusViewDTO
    {
        public SyllabusGeneralDTO SyllabusBase { get; set; }
        public ICollection<UnitDTO> Units { get; set; }
    }
}
