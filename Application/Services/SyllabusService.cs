using Application.Commons;
using Application.Interfaces;
using Application.ViewModels.SyllabusModels;
using AutoMapper;
using Domain.Entities;
using System.Security.Authentication;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using Application.Repositories;
using Application.ViewModels.QuizModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Application.ViewModels.SyllabusModels.ViewDetail;
using Application.ViewModels.SyllabusModels.FixViewSyllabus;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using Microsoft.AspNetCore.Mvc;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;

namespace Application.Services
{
    public class SyllabusService : ISyllabusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsservice;
        private readonly IMapper _mapper;
        public SyllabusService(IUnitOfWork unitOfWork, IClaimsService claimsservice)
        {
            _unitOfWork = unitOfWork;
            _claimsservice = claimsservice;
        }
        public SyllabusService(IUnitOfWork unitOfWork, IClaimsService claimsservice, IMapper mapper) : this(unitOfWork, claimsservice)
        {
            _mapper = mapper;
        }
        //Add newSyllabus base
        public async Task<Syllabus> AddSyllabusAsync(SyllabusGeneralDTO syllabusDTO)
        {
            if (_claimsservice.GetCurrentUserId == Guid.Empty) throw new AuthenticationException("User not Logined!");
            var syllabusId = Guid.NewGuid();
            Guid userID = _claimsservice.GetCurrentUserId;
            var syllabus = _mapper.Map<Syllabus>(syllabusDTO);
            syllabus.Id = syllabusId;
            syllabus.UserId = userID;
            syllabus.Status = "Active";

           
            await _unitOfWork.SyllabusRepository.AddAsync(syllabus);
            return syllabus;
        }
        public async Task<List<Syllabus>> FilterSyllabus(double duration1, double duration2)
        {
            var filterSyllabusList = await _unitOfWork.SyllabusRepository.FilterSyllabusByDuration(duration1, duration2);
            return filterSyllabusList;

        }
        public async Task<bool> DeleteSyllabussAsync(string syllabusID)
        {
            bool check = false;
            var syllabusFind = await _unitOfWork.SyllabusRepository.GetByIdAsync(Guid.Parse(syllabusID));
            if (syllabusFind is not null && syllabusFind.IsDeleted == false)
            {
                _unitOfWork.SyllabusRepository.SoftRemove(syllabusFind);
                check = true;
            }
            await _unitOfWork.SaveChangeAsync();
            return check;


        }
        /// <summary>
        /// Update a Syllabus
        /// </summary>
        /// <param name="syllabusId">ID of updated syllabus</param>
        /// <param name="updateItem">Changing description</param>
        /// <returns>bool</returns>
        public async Task<bool> UpdateSyllabus(Guid syllabusId, UpdateSyllabusModel updateItem)
        {
            /*var result = false;
            var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);

            if (syllabus is not null)
            {
                if (_mapper is not null)
                {
                    var updatedUnitDTO = updateItem.Units.Where(u => u.UnitID is not null).ToList();

                    // Check if unitId is null or not, if null, create new unit
                    if (updateItem.Units is not null)
                    {
                        var createUnit = updateItem.Units.Where(u => u.UnitID is null).ToList();
                        foreach (var item in updateItem.Units) item.syllabusId = syllabusId;
                        if (createUnit.Count > 0)
                        {
                            foreach (var unit in createUnit)
                            {
                                unit.UnitID = Guid.NewGuid();
                            }
                            var unitList = _mapper.Map<List<Unit>>(createUnit);
                            await _unitOfWork.UnitRepository.AddRangeAsync(unitList);

                        }
                    }
                    // Section Get All Lecture, set unit Id, prepare for lecture to update
                    var updateLecture = from u in updateItem.Units
                                        select u.UpdateLectureDTOs.ToList().FirstOrDefault();
                    var lectures = await _unitOfWork.LectureRepository.GetLectureBySyllabusId(syllabusId);
                    var updateLectureList = updateLecture.ToList();
                    foreach (var unit in updateItem.Units)
                    {
                        foreach (var lec in unit.UpdateLectureDTOs)
                        {
                            lec.UnitID = unit.UnitID;
                        }
                    }
                    // Update unit which is not created and syllabus Information
                    updateItem.Units = updatedUnitDTO;
                    _mapper.Map(updateItem, syllabus, typeof(UpdateSyllabusDTO), typeof(Syllabus));
                    _unitOfWork.SyllabusRepository.Update(syllabus);
                    await _unitOfWork.SaveChangeAsync();

                    // Starting Update/Create lecture
                    foreach (var item in updateLectureList)
                    {
                        if (item.LectureID is null)
                        {
                            var newID = Guid.NewGuid();
                            item.LectureID = newID;
                            var lecture = _mapper.Map<Lecture>(item);

                            await _unitOfWork.LectureRepository.AddAsync(lecture);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                if (item.UnitID is null) return false;
                                UpdateUnitLectureDTO detailDTO = new()
                                {
                                    UnitId = item.UnitID.Value,
                                    LectureId = newID
                                };
                                var detail = _mapper.Map<DetailUnitLecture>(detailDTO);
                                await _unitOfWork.DetailUnitLectureRepository.AddAsync(detail);
                            }
                        }
                        else
                        {
                            var lecture = lectures.FirstOrDefault(l => l.Id == item.LectureID);
                            _ = _mapper.Map(item, lecture, typeof(UpdateLectureDTO), typeof(Lecture));
                            if (lecture is not null)
                            {
                                _unitOfWork.LectureRepository.Update(lecture);
                            }

                        }
                    }
                    if (await _unitOfWork.SaveChangeAsync() > 0) result = true;
                }
            }
            return result;*/
            var result = false;
            var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
            if(syllabus is not null)
            {
                _ = _mapper.Map(updateItem, syllabus, typeof(UpdateSyllabusModel), typeof(Syllabus));
                syllabus.Duration = updateItem.Durations.TotalHours;
                _unitOfWork.SyllabusRepository.Update(syllabus);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    foreach (var item in updateItem.Outline)
                    {
                        var resultUnit = await UpdateUnit(syllabusId, item.Content, item.Day);
                    }
                    result = true;
                }
                else throw new Exception("Can not Update Syllabus!");
            } 
            return result;

        }
        #region PrivateUpdateMethod
        private async Task<bool> UpdateUnit(Guid syllabusId, List<UpdateContentModel> updateUnit, int session)
        {
            var result = false;
            var units = await _unitOfWork.UnitRepository.FindAsync(x => x.SyllabusID == syllabusId);
            if (units.Count > 0)
            {
                _unitOfWork.UnitRepository.SoftRemoveRange(units);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    /*                    var updateUnits = _mapper.Map<List<Unit>>(updateUnit);
                                        foreach (var item in updateUnits)
                                        {
                                            item.Id = Guid.NewGuid();
                                            item.Session = session;
                                            item.SyllabusID = syllabusId;
                                            await _unitOfWork.UnitRepository.AddAsync(item);
                                            if(await _unitOfWork.SaveChangeAsync() > 0)
                                            {
                                                UpdateLecture(unitId : item.Id, syllabusId, updateUnit.)
                                            }
                                        }*/
                    foreach (var item in updateUnit)
                    {
                        var unit = _mapper.Map<Unit>(item);
                        unit.Id = Guid.NewGuid();
                        unit.Session = session;
                        unit.SyllabusID = syllabusId;
                        await _unitOfWork.UnitRepository.AddAsync(unit);
                        if (await _unitOfWork.SaveChangeAsync() > 0)
                        {
                            await UpdateLecture(unitId: unit.Id, syllabusId, item.Lessons);
                        }
                    }
                }
                else throw new Exception("Save Changes Failed");
                result = true;
            }

            return result;

        }
        private async Task<bool> UpdateLecture(Guid unitId, Guid syllabusId, List<UpdateLessonModel> updateLecture)
        {
            var result = false;
            var lectures = await _unitOfWork.LectureRepository.GetLectureBySyllabusId(syllabusId);
            if(lectures.Count() > 0)
            {
                // Remove old Lectures
                _unitOfWork.LectureRepository.SoftRemoveRange((List<Lecture>)lectures);
                if (await _unitOfWork.SaveChangeAsync() <= 0) throw new Exception("Save change fail");

                // Add New Lectures
                var newLectures = _mapper.Map<List<Lecture>>(updateLecture);
                foreach (var item in newLectures)
                {
                    item.Id = Guid.NewGuid();
                    await _unitOfWork.LectureRepository.AddAsync(item);
                    if(await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        await _unitOfWork.DetailUnitLectureRepository.AddAsync(new DetailUnitLecture {UnitId = unitId, LectureID = item.Id });
                    }
                    
                }
                if (await _unitOfWork.SaveChangeAsync() > 0) result = true;
          

            } 
            return result;
        }
        
        #endregion
        public Task<List<SyllabusViewAllDTO>> GetByName(string name)
        {

            var result = _unitOfWork.SyllabusRepository.SearchByName(name);
            return result;
        }


        public async Task<List<SyllabusViewAllDTO>> GetAllSyllabus()
        {
            var syllabusList = await _unitOfWork.SyllabusRepository.GetAllAsync();
            return syllabusList;
        }

        //public async Task<SyllabusShowDetailDTO> ViewDetailSyllabus(Guid SyllabusID)
        //{
        //    SyllabusShowDetailDTO viewDTO = new SyllabusShowDetailDTO();

        //    Syllabus syllabusDetail = await _unitOfWork.SyllabusRepository.GetByIdAsync(SyllabusID);

        //    //if (!syllabusDetail.IsDeleted)
        //    //{
        //    //    return null;
        //    //}

        //    SyllabusGeneralDTO syllabusGeneralDTO = new SyllabusGeneralDTO()
        //    {

        //        Code = syllabusDetail!.Code,
        //        CourseObject = syllabusDetail.CourseObjective,
        //        Duration = syllabusDetail.Duration,
        //        SyllabusName = syllabusDetail.SyllabusName,
        //        TechRequirements = syllabusDetail.TechRequirements
        //    };


        //    viewDTO.SyllabusBase = syllabusGeneralDTO;
        //    //viewDTO.SyllabusBase.SyllabusName = syllabusDetail.SyllabusName;
        //    //viewDTO.SyllabusBase.Code = syllabusDetail.Code;
        //    //viewDTO.SyllabusBase.CourseObject = syllabusDetail.CourseObjective;
        //    //viewDTO.SyllabusBase.TechRequirements = syllabusDetail.TechRequirements;
        //    //viewDTO.SyllabusBase.Duration = syllabusDetail.Duration;
        //    var unitList = await _unitOfWork.UnitRepository.FindAsync(x => x.SyllabusID == SyllabusID);

        //    foreach (var unit in unitList)
        //    {

        //        List<LectureDTO> lectures = new List<LectureDTO>();
        //        var lectureList = _unitOfWork.DetailUnitLectureRepository.GetByUnitID(unit.Id);
        //        foreach (var lecture in lectureList)
        //        {
        //            lectures.Add(lecture);
        //        }
        //        var unitAdd = new UnitDetailDTO()
        //        {
        //            UnitName = unit.UnitName,
        //            Session = unit.Session,
        //            TotalTime = (float)unit.TotalTime,
        //            Lectures = lectures
        //        };
        //        List<UnitDetailDTO> units = new List<UnitDetailDTO>();
        //        units.Add(unitAdd);
        //        viewDTO.Units = units;



        //    }
        //    return viewDTO;
        //}


        public async Task<FinalViewSyllabusDTO> FinalViewSyllabusDTO(Guid SyllabusID)
        {

            FinalViewSyllabusDTO view = new FinalViewSyllabusDTO();
            var SyllabusInformation = await _unitOfWork.SyllabusRepository.GetByIdAsync(SyllabusID);

            TimeAllocationDTO timeAllocationDTO = new TimeAllocationDTO {
                TestPercent = 12,
                GuidePercent = 12,
                 ExamPercent = 12,
                  ConceptPercent= 12,
                  AssignmentPercent = 12 
            
            };
            view.timeAllocationDTO = timeAllocationDTO;
            var user = await _unitOfWork.UserRepository.GetByIdAsync(SyllabusInformation.UserId);
            string userName = "Nguyễn Văn Tèo";
            if (SyllabusInformation.IsDeleted)
            {
                return null;
            }

            var UnitInformation = await _unitOfWork.UnitRepository.FindAsync(x => x.SyllabusID == SyllabusID && x.IsDeleted == false);

  
            var generalInformation = _mapper.Map<FinalViewSyllabusDTO>(SyllabusInformation);


            view.ID = SyllabusID;
            view.CreateBy = userName;
            view.Durations = new DurationView { TotalHours = SyllabusInformation.Duration };
            if(view.Durations.TotalDate == 0)
            {
                view.Durations.TotalHours = 12;
            }
            view.ProgramName = generalInformation.ProgramName;
            view.Status = generalInformation.Status;
            view.General = generalInformation.General;
       

            List<int> ListSession = new List<int>();
            List<SyllabusOutlineDTO> lst = new List<SyllabusOutlineDTO>();
            foreach (var item in UnitInformation)
            {
                ListSession = checkSession(item.Session, ListSession, UnitInformation.Count());

            }

            for (int i = 0; i < ListSession.Count; i++)
            {

                var unit_have_session = await _unitOfWork.UnitRepository.FindAsync(x => x.Session == ListSession[i] && x.IsDeleted == false && x.SyllabusID == SyllabusID);

                //trong tung unit co nhieu lesson 
                foreach (var item in unit_have_session)
                {
                    //tim thong tin cua Lesson 
                    List<ContentSyllabusDTO> contentSyllabusDTOs = new List<ContentSyllabusDTO>();

                    ContentSyllabusDTO contentSyllabusDTO = new ContentSyllabusDTO()
                    {
                        Lessons = _unitOfWork.SyllabusRepository.LessonDTOsAsync(item.Id),
                        Hours = 10,
                        UnitName = item.UnitName,
                        UnitNum = item.UnitNum
                    };

                    contentSyllabusDTOs.Add(contentSyllabusDTO);

                    SyllabusOutlineDTO syllabusOutlineDTO = new SyllabusOutlineDTO()
                    {
                        Content = contentSyllabusDTOs,
                        Day = ListSession[i]
                    };
                    lst.Add(syllabusOutlineDTO);

                }
                //lst.Add(syllabusOutlineDTO);

            }
            //view.General

            TimeAllocationDTO allocationDTO = new TimeAllocationDTO()
            {
                AssignmentPercent = 54,
                ConceptPercent = 29,
                GuidePercent = 9,
                ExamPercent = 6,
                TestPercent = 1
            };



            OutlineSyllabusDTO viewOutline = new OutlineSyllabusDTO()
            {
                outlineDTOs = lst,
            };
            view.outlineSyllabusDTO = viewOutline;

            AssessmentSchemeDTO assessmentSchemeDTO = new AssessmentSchemeDTO()
            {
                AssignmentPercent = 15,
                FinalPercent = 70,
                QuizPercent = 15,
                FinalTheoryPercent = 40,
                GPAPercent = 70,
                FinalPreactisePercent = 60
            };
            OtherSyllabusDTO otherSyllabusDTO = new OtherSyllabusDTO()
            {
                assessmentScheme = assessmentSchemeDTO,
                TrainingDeliveryPriciple = "ai biet gi ba"
            };
            view.OtherSyllabusDTOOther = otherSyllabusDTO;
            return view;
        }

        private List<int> checkSession(int Session, List<int> SessionL, int countNum)
        {
            //List<int> ListSession = new List<int>();
            if (SessionL.Count == 0)
            {
                SessionL = new List<int>();
                SessionL.Add(Session);
            }
            else
            {


                for (int i = 0; i <= countNum - 1; i++)

                    if (SessionL[i] != Session)
                    {
                        SessionL.Add(Session);
                    }
            }
            return SessionL;

        }

        public Task<int> SaveChangesAsync()
        {
            return _unitOfWork.SaveChangeAsync();
        }



        public async Task<Syllabus> AddNewSyllabusService(UpdateSyllabusModel updateSyllabusModel)
        {
            
            var mapperSyllabus  = _mapper.Map<Syllabus>(updateSyllabusModel);
            //mapperSyllabus.Duration = updateSyllabusModel.Duration.TotalHours;
            mapperSyllabus.UserId =  _claimsservice.GetCurrentUserId;
            mapperSyllabus.Id = Guid.NewGuid();
            await _unitOfWork.SyllabusRepository.AddAsync(mapperSyllabus);
            
            return mapperSyllabus;

        }
    }

}







