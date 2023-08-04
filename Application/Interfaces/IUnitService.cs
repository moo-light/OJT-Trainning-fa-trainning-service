using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUnitService
    {

        Task<IEnumerable<Unit>> GetSyllabusDetail(Guid syllabusID);

        Task<Unit> AddNewUnit(UnitDTO unitDTO, Syllabus syllabus);

        Task<Unit> AddNewUnitHotFix(UpdateContentModel updateSyllabusModel, int session, Guid syllabusID);
    }
}
