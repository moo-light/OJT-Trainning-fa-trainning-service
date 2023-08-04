using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.FixViewSyllabus;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.EntityState;
namespace Infrastructures.Repositories
{
    public class SyllabusRepository : GenericRepository<Syllabus>, ISyllabusRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public SyllabusRepository(AppDbContext context, IMapper mapper, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public async Task<Syllabus> AddSyllabusAsync(SyllabusGeneralDTO syllabusDTO)
        {
            var newSyllabus = new Syllabus()
            {
                UserId = _claimsService.GetCurrentUserId,
                CreationDate = DateTime.Now,
                IsDeleted = false
            };
            _mapper.Map(syllabusDTO, newSyllabus);
            //Khó dị trời
            await AddAsync(newSyllabus);
            return newSyllabus;
        }
        public async Task<List<Syllabus>> FilterSyllabusByDuration(double duration1, double duration2)
        {
            List<Syllabus> result = _dbContext.Syllabuses.Where(x => x.Duration > duration1 && x.Duration < duration2).ToList();
            return result;
        }

        public async Task<List<Syllabus>> GetSyllabusByTrainingProgramId(Guid trainingProgramId)
        {
            var syllabusList = from s in _dbContext.Syllabuses
                               join d in _dbContext.DetailTrainingProgramSyllabuses
                               on s.Id equals d.SyllabusId
                               where d.TrainingProgramId == trainingProgramId && d.IsDeleted == false
                               select s;
            if (!syllabusList.IsNullOrEmpty()) return await syllabusList.ToListAsync();
            else return null;
        }

        public async Task<List<SyllabusViewAllDTO>> SearchByName(string name)
        {
            var result = _dbContext.Syllabuses
                .Where(x => x.SyllabusName.ToUpper().Contains(name)&&x.IsDeleted==false)
                .Select(s => new SyllabusViewAllDTO
                {
                    ID = s.Id,
                    SyllabusName = s.SyllabusName,
                    Code = s.Code,
                    CreatedOn = s.CreationDate,
                    CreatedBy = s.User.UserName,
                    Duration = new DurationView() { TotalHours = s.Duration },
                    OutputStandard = s.Units.SelectMany(u => u.DetailUnitLectures)
                                 .Select(dul => dul.Lecture.OutputStandards)
                                 .ToList()
                }).ToList();

            return result;


        }

        public async Task<List<SyllabusViewAllDTO>> GetAllAsync()
        {
            var syllabusList = _dbContext.Syllabuses
     .Include(s => s.User)
     .Include(s => s.Units)
         .ThenInclude(u => u.DetailUnitLectures)
             .ThenInclude(dul => dul.Lecture)
             .Where(s=>s.IsDeleted== false)
     .Select(s => new SyllabusViewAllDTO
     {
         ID = s.Id,
         SyllabusName = s.SyllabusName,
         Code = s.Code,
         CreatedOn = s.CreationDate,
         CreatedBy = s.User.UserName,
         Duration = new DurationView() { TotalHours = s.Duration},
         OutputStandard = s.Units.SelectMany(u => u.DetailUnitLectures)
                                 .Select(dul => dul.Lecture.OutputStandards)
                                 .ToList()
     })
     .ToList();
            return syllabusList;
        }


        //public  async Task<SyllabusOutlineDTO> GetBySession(int Session, Guid syllabusID)
        //{
        //    //throw new NotImplementedException();
        //    //var list_unit_in_session =  from unit in _dbContext.Units
        //    List<ContentSyllabusDTO> contentSyllabusDTOs = new List<ContentSyllabusDTO>();
        //    var list_unit_in_session = _dbContext.Units.Where(x => x.Session == Session && x.SyllabusID == syllabusID);
        //    ContentSyllabusDTO contentSyllabusDTO1 = new ContentSyllabusDTO();
        //    contentSyllabusDTO1.Lessons = LessonDTOsAsync(Session, syllabusID);
        //    foreach (var item in list_unit_in_session)
        //    {
        //        //var lessonDTOs = await (from unit in _dbContext.Units
        //        //                  join detaillecture in _dbContext.DetailUnitLecture on unit.Id equals detaillecture.UnitId
        //        //                  join lecture in _dbContext.Lectures on detaillecture.LectureID equals lecture.Id
        //        //                  where unit.Session == Session
        //        //                  select new LessonDTO
        //        //                  {

        //        //                      DeliveryType = lecture.DeliveryType,
        //        //                      Hours = lecture.Duration,
        //        //                      Name = lecture.LectureName,
        //        //                      OutputStandard = lecture.OutputStandards,
        //        //                      Status = lecture.Status,

        //        //                  }).ToListAsync();


        //        ContentSyllabusDTO contentSyllabusDTO = new ContentSyllabusDTO
        //        {
        //            Hours = 20,
        //            UnitName = item.UnitName,
        //            UnitNum = item.UnitNum,
        //            Lessons = lessonDTOs

        //        };

        //        contentSyllabusDTOs.Add(contentSyllabusDTO);

        //    }


        //    SyllabusOutlineDTO syllabusOutlineDTOL = new SyllabusOutlineDTO
        //    {
        //        Day = Session,
        //        Content = contentSyllabusDTOs,

        //    };
        //    return syllabusOutlineDTOL;
        //}

        public List<LessonDTO> LessonDTOsAsync(Guid unitID)
        {
            var lessonDTOs = (from unit in _dbContext.Units
                              join detaillecture in _dbContext.DetailUnitLecture on unit.Id equals detaillecture.UnitId
                              join lecture in _dbContext.Lectures on detaillecture.LectureID equals lecture.Id
                              where unit.Id == unitID  && unit.IsDeleted == false
                              select new LessonDTO
                              {

                                  DeliveryType = lecture.DeliveryType,
                                  Hours = lecture.Duration,
                                  Name = lecture.LectureName,
                                  OutputStandard = lecture.OutputStandards,
                                  Status = lecture.Status,

                              }).ToList();
            return lessonDTOs;
        }

        public Task<SyllabusOutlineDTO> GetBySession(int Session, Guid syllabusID)
        {
            throw new NotImplementedException();
        }
    }

}
