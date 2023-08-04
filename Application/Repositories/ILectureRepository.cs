using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ILectureRepository : IGenericRepository<Lecture>
    {
        public Task<IEnumerable<Lecture>> GetLectureBySyllabusId(Guid syllabusId);
        public Guid GetLectureIdByName(string name);

    }
}
