using Application.Filter.TrainingProgramFilter;
using Application.Interfaces;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TrainingProgramService : ITrainingProgramService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TrainingProgramService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TrainingProgramService(IUnitOfWork unitOfWork, IMapper mapper) : this(unitOfWork)
        {
            _mapper = mapper;
        }

        public async Task<TrainingProgramViewModel> GetTrainingProgramDetail(Guid id)
        {
            var result = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(id);
            if (result is not null && result.IsDeleted == false)
            {
                var trainingProgramView = _mapper.Map<TrainingProgramViewModel>(result);
                if (trainingProgramView.TrainingProgramId is not null)
                    trainingProgramView.Contents = _mapper.Map<ICollection<SyllabusTrainingProgramViewModel>>((ICollection<Syllabus>?)await _unitOfWork.SyllabusRepository.GetSyllabusByTrainingProgramId(trainingProgramView.TrainingProgramId.Value));

                return trainingProgramView;
            }
            return null;
        }

        public async Task<TrainingProgram> CreateTrainingProgram(CreateTrainingProgramDTO createTrainingProgramDTO)
        {
            var syllabusesId = createTrainingProgramDTO.SyllabusesId;
            if (syllabusesId is not null)
            {


                // Create Training Program
                var trainingProgram = _mapper.Map<TrainingProgram>(createTrainingProgramDTO);
                trainingProgram.Id = Guid.NewGuid();
                trainingProgram.Status = "Active";
                await _unitOfWork.TrainingProgramRepository.AddAsync(trainingProgram);
                await _unitOfWork.SaveChangeAsync();

                // Create Training Program Detail (Syllabuses - TrainingProgram)
                foreach (var syllabusId in syllabusesId)
                {
                    var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
                    if (syllabus is not null)
                    {
                        var newDetailProgramSyllabus = new DetailTrainingProgramSyllabus { SyllabusId = syllabus.Id, TrainingProgramId = trainingProgram.Id, Status = "active" };
                        await _unitOfWork.DetailTrainingProgramSyllabusRepository.AddAsync(newDetailProgramSyllabus);
                    }
                    trainingProgram.Duration += syllabus!.Duration;
                }
                _unitOfWork.TrainingProgramRepository.Update(trainingProgram);
                if (await _unitOfWork.SaveChangeAsync() > 0) return trainingProgram;
            }
            return null;

        }

        public async Task<bool> UpdateTrainingProgram(UpdateTrainingProgramDTO updateProgramDTO)
        {
            var updateProgram = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(updateProgramDTO.Id!.Value);
            if (updateProgram is not null)
            {
                _ = _mapper.Map(updateProgram, updateProgramDTO, typeof(UpdateTrainingProgramDTO), typeof(TrainingProgram));
                updateProgram.Status = "Active";
                if (updateProgram is not null) _unitOfWork.TrainingProgramRepository.Update(updateProgram);
                var detailProgramSyllbuses = await _unitOfWork.DetailTrainingProgramSyllabusRepository.FindAsync(x => x.TrainingProgramId == updateProgram.Id);
                if (detailProgramSyllbuses is not null) _unitOfWork.DetailTrainingProgramSyllabusRepository.SoftRemoveRange(detailProgramSyllbuses);

                var syllabusesId = updateProgramDTO.SyllabusesId;
                foreach (var syllabusId in syllabusesId!)
                {
                    var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(syllabusId);
                    if (syllabus is not null)
                    {
                        var newDetailProgramSyllabus = new DetailTrainingProgramSyllabus { SyllabusId = syllabus.Id, TrainingProgramId = updateProgramDTO.Id.Value, Status = "active" };
                        await _unitOfWork.DetailTrainingProgramSyllabusRepository.AddAsync(newDetailProgramSyllabus);
                        updateProgram!.Duration += syllabus.Duration;
                    }
                }
                _unitOfWork.TrainingProgramRepository.Update(updateProgram!);

                if (await _unitOfWork.SaveChangeAsync() > 0)
                    return true;
            }
            return false;

        }

        public async Task<bool> DeleteTrainingProgram(Guid trainingProgramId)
        {
            var trainingProgram = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(trainingProgramId);
            if (trainingProgram is not null)
            {
                _unitOfWork.TrainingProgramRepository.SoftRemove(trainingProgram);

                var detailProgramSyllabuses = await _unitOfWork.DetailTrainingProgramSyllabusRepository.FindAsync(x => x.TrainingProgramId == trainingProgram.Id);
                if (detailProgramSyllabuses is not null)
                {
                    _unitOfWork.DetailTrainingProgramSyllabusRepository.SoftRemoveRange(detailProgramSyllabuses);
                }
                if (await _unitOfWork.SaveChangeAsync() > 0) return true;
            }
            return false;
        }



        public async Task<List<ViewAllTrainingProgramDTO>> ViewAllTrainingProgramDTOs()
        {
            var allTrainingProgram = await _unitOfWork.TrainingProgramRepository.GetAllAsync();
            var mapViewTrainingProgram = _mapper.Map<List<ViewAllTrainingProgramDTO>>(allTrainingProgram);
            var listAll = from a in mapViewTrainingProgram
                          select new
                          {
                              a.Id,
                          };
            var viewAllTraining = new List<ViewAllTrainingProgramDTO>();
            foreach (var a in listAll)
            {
                var result = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(a.Id);
                if (result is not null && result.IsDeleted == false)
                {
                    //viet ham lay syllabusid by trainingprogramid
                    var trainingProgramView = _mapper.Map<ViewAllTrainingProgramDTO>(result);
                    trainingProgramView.Content = await _unitOfWork.SyllabusRepository.GetSyllabusByTrainingProgramId(trainingProgramView.Id);
                    viewAllTraining.Add(trainingProgramView);
                }
            }
            return viewAllTraining.Count > 0 ? viewAllTraining : null;


        }
        public async Task<List<SearchAndFilterTrainingProgramViewModel>> SearchTrainingProgramWithFilter(string? searchString, string? status, string? createBy)
        {
            var listTP = await _unitOfWork.TrainingProgramRepository.GetAllAsync();
            var listUsers = await _unitOfWork.UserRepository.GetAllAsync();
            var listUserId = new List<Guid>();
            if (createBy != null)
            {
                listUserId.AddRange(listUsers.Where(u => u.FullName != null && u.FullName.ToLower().Contains(createBy.ToLower())).Select(u => u.Id).ToList());
                listUserId.AddRange(listUsers.Where(u => u.UserName != null && u.UserName.ToLower().Contains(createBy.ToLower())).Select(u => u.Id).ToList());
                listUserId = listUserId.Distinct().ToList();
            }
            ICriterias<TrainingProgram> statusCriteria = new StatusCriteria(status);
            ICriterias<TrainingProgram> createByCriteria = new CreateByCriteria(listUserId);
            ICriterias<TrainingProgram> andCriteria = new AndTrainingProgramCriteria(statusCriteria, createByCriteria);
            //search training program with filter
            if (searchString != null)
            {
                listTP = listTP.Where(tp => tp.ProgramName.ToLower().Contains(searchString.ToLower())).ToList();
                var result = _mapper.Map<List<SearchAndFilterTrainingProgramViewModel>>(andCriteria.MeetCriteria(listTP));
                return result;
            }
            //using filter only (filter from all Training Program)
            else if (status != null || createBy != null)
            {
                var result = _mapper.Map<List<SearchAndFilterTrainingProgramViewModel>>(andCriteria.MeetCriteria(listTP));
                return result;
            }
            //not inputting search or filter
            else
            {
                var result = _mapper.Map<List<SearchAndFilterTrainingProgramViewModel>>(listTP);
                return result;
            }
        }
        public async Task<TrainingProgram> DuplicateTrainingProgram(Guid TrainingProgramId)
        {
            var duplicateItem = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(TrainingProgramId, x => x.DetailTrainingProgramSyllabus);
            if (duplicateItem is not null)
            {
                var createItem = new TrainingProgram
                {
                    Id = Guid.NewGuid(),
                    ProgramName = duplicateItem.ProgramName,
                    Status = "Active"
                };
                await _unitOfWork.TrainingProgramRepository.AddAsync(createItem);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    List<DetailTrainingProgramSyllabus> createdDetail = new List<DetailTrainingProgramSyllabus>();
                    foreach (var item in duplicateItem.DetailTrainingProgramSyllabus)
                    {
                        createdDetail.Add(new DetailTrainingProgramSyllabus
                        {
                            TrainingProgramId = createItem.Id,
                            SyllabusId = item.SyllabusId,
                            Status = "Active"
                        });
                    }
                    await _unitOfWork.DetailTrainingProgramSyllabusRepository.AddRangeAsync(createdDetail);
                    if (await _unitOfWork.SaveChangeAsync() > 0) return createItem;
                    else throw new Exception("Can not Insert DetailTrainingProgramSyllabuses!");
                }
                else throw new Exception("Add Training Program Failed _ Save Change Failed!");

            }
            else throw new Exception("Not found or TrainingProgram has been deleted");

        }
    }
}
