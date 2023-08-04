using Application.ViewModels.SyllabusModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IDetailUnitLectureRepository : IGenericRepository<DetailUnitLecture>
    {

        public List<LectureDTO> GetByUnitID(Guid UnitId);
    }
}
