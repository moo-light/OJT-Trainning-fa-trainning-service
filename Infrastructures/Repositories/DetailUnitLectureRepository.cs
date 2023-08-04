using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class DetailUnitLectureRepository : GenericRepository<DetailUnitLecture>, IDetailUnitLectureRepository
    {
        private readonly AppDbContext _AppDbContext;
        public DetailUnitLectureRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _AppDbContext = context;
        }

        public List<LectureDTO> GetByUnitID(Guid UnitId)
        {
            List<LectureDTO> lectureDTOs = new List<LectureDTO>();
            var result = from detaillecture in _AppDbContext.DetailUnitLecture
                         join lecture in _AppDbContext.Lectures on detaillecture.LectureID equals lecture.Id
                         where detaillecture.UnitId == UnitId
                         select new LectureDTO
                         {
                             DeliveryType = lecture.DeliveryType,
                             Duration = (float)lecture.Duration,
                             LectureName = lecture.LectureName,
                             OutputStandards = lecture.OutputStandards,
                             Status = lecture.Status,
                         };
            foreach (var item in result)
            {
                lectureDTOs.Add(item);
            }
            return lectureDTOs.ToList();

        }
    }
}
