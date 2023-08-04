using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateOutlineModel
    {
        public int Day { get; set; }
        public List<UpdateContentModel> Content { get; set; } = default!;
    }
}
