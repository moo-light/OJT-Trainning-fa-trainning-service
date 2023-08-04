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
    public interface ILectureService
    {
        Task<Lecture> AddNewLecture(LectureDTO lecture);

        Task<DetailUnitLecture> AddNewDetailLecture(Lecture lecture, Unit unit);

        Task<Lecture> AddNewLectureHotFix(UpdateLessonModel updateLessonModel);
    }
}
