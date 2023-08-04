using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.FixViewSyllabus;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{

    public interface ISyllabusRepository : IGenericRepository<Syllabus>
    {
        public Task<List<SyllabusViewAllDTO>> GetAllAsync();
        public Task<List<Syllabus>> FilterSyllabusByDuration(double duration1, double duration2);

        public Task<List<SyllabusViewAllDTO>> SearchByName(string name);

        Task<Syllabus> AddSyllabusAsync(SyllabusGeneralDTO syllabusDTO);
        Task<List<Syllabus>> GetSyllabusByTrainingProgramId(Guid trainingProgramId);

        Task<SyllabusOutlineDTO> GetBySession(int Session, Guid syllabusID);

        public List<LessonDTO> LessonDTOsAsync(Guid unitID);


    }
}
