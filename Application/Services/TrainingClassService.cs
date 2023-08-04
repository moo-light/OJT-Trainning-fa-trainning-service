using Application.Filter.ClassFilter;
using Application.Interfaces;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Training Class Services
    /// </summary>
    public class TrainingClassService : ITrainingClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainingClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        /// <summary>
        /// SoftRemoveTrainingClassAsync set the isDeleted value of training class to true
        /// </summary>
        /// <param className="trainingClassId">training class ID</param>
        /// <returns>True if save succesful, false if save fail</returns>
        public async Task<bool> SoftRemoveTrainingClassAsync(string trainingClassId)
        {
            var trainingClassObj = await GetTrainingClassByIdAsync(trainingClassId);

            _unitOfWork.TrainingClassRepository.SoftRemove(trainingClassObj);
            return (await _unitOfWork.SaveChangeAsync() > 0);
        }

        /// <summary>
        /// SearchClassByNameAsync return classes by class className
        /// </summary>
        /// <param className="className">Training class name</param>
        /// <returns>List of training classes<TrainingClass></returns>
        public async Task<List<TrainingClassViewAllDTO>> SearchClassByNameAsync(string className)
        {
            var listClass = _unitOfWork.TrainingClassRepository.SearchClassByName(className);
            return listClass;
        }

        /// <summary>
        /// DuplicateClassAsync duplicate an existed training class
        /// </summary>
        /// <param name="id">Training class ID</param>
        /// <returns>True if training class existed, false otherwise</returns>
        public async Task<bool> DuplicateClassAsync(Guid id)
        {
            var result = await _unitOfWork.TrainingClassRepository.GetByIdAsync(id);
            if (result != null)
            {
                TrainingClass trainingClass = new TrainingClass()
                {
                    Name = result.Name,
                    StartTime = result.StartTime,
                    EndTime = result.EndTime,
                    CreatedBy = result.CreatedBy,
                    Code = result.Code,
                    Attendee = result.Attendee,
                    Branch = result.Branch,
                    CreationDate = result.CreationDate,
                    LocationID = result.LocationID,
                    DeleteBy = result.DeleteBy,
                    DeletionDate = result.DeletionDate,
                    IsDeleted = result.IsDeleted,
                    TrainingProgramId = result.TrainingProgramId,
                    StatusClassDetail = result.StatusClassDetail,

                };
                await _unitOfWork.TrainingClassRepository.AddAsync(trainingClass);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// CreateTrainingClassAsync add new training class to the database
        /// </summary>
        /// <param name="createTrainingClassDTO">Create training class DTO</param>
        /// <returns>Training class view model</returns>
        public async Task<TrainingClassViewModel?> CreateTrainingClassAsync(CreateTrainingClassDTO createTrainingClassDTO)
        {
            var trainingClassObj = _mapper.Map<TrainingClass>(createTrainingClassDTO);
            trainingClassObj.Id = Guid.NewGuid();
            //check location
            var location = await _unitOfWork.LocationRepository.GetByNameAsync(createTrainingClassDTO.LocationName);
            if (location == null)
            {
                //location = trainingClassObj.Location = new Location()
                //{
                //    LocationName = createTrainingClassDTO.LocationName,
                //};
                trainingClassObj.LocationID = null;
            }
            else
            {
                trainingClassObj.LocationID = location.Id;
            }
            //check training program
            _ = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(createTrainingClassDTO.TrainingProgramId) ?? throw new Exception("Invalid training program Id");
            // admins
            if (!trainingClassObj.TrainingClassAdmins.IsNullOrEmpty())
            {
                foreach (var user in trainingClassObj.TrainingClassAdmins)
                {
                    if (!CheckTrainingClassAdminsIdAsync(user).Result)
                    {
                        throw new Exception("Incorrect admin id!");
                    }
                }
            }

            //trainers
            if (!trainingClassObj.TrainingClassTrainers.IsNullOrEmpty())
            {
                foreach (var user in trainingClassObj.TrainingClassTrainers)
                {
                    if (!CheckTrainingClassTrainersIdAsync(user).Result)
                    {
                        throw new Exception("Incorrect trainer id!");
                    }
                }
            }
            trainingClassObj.Duration = (createTrainingClassDTO.EndTime - createTrainingClassDTO.StartTime).TotalMinutes;
            await _unitOfWork.TrainingClassRepository.AddAsync(trainingClassObj);

            var viewModel = _mapper.Map<TrainingClassViewModel>(trainingClassObj);
            return (await _unitOfWork.SaveChangeAsync() > 0) ? viewModel : null;
        }

        /// <summary>
        /// UpdateTrainingClassAsync update training class based on its id
        /// </summary>
        /// <param className="trainingClassId">Training class ID</param>
        /// <param className="updateTrainingClassDTO">Update training class DTO</param>
        /// <returns>True if save successfully, false if save fail</returns>
        public async Task<bool> UpdateTrainingClassAsync(string trainingClassId, UpdateTrainingClassDTO updateTrainingClassDTO)
        {
            var trainingClassObj = await GetTrainingClassByIdAsync(trainingClassId);
            _mapper.Map(updateTrainingClassDTO, trainingClassObj);
            //check location
            var location = await _unitOfWork.LocationRepository.GetByNameAsync(updateTrainingClassDTO.LocationName);
            if (location == null)
            {
                //trainingClassObj.Location = new Location()
                //{
                //    LocationName = updateTrainingClassDTO.LocationName,
                //};
                trainingClassObj.LocationID = null;
            }
            else
            {
                trainingClassObj.LocationID = location.Id;
            }

            //check training program
            _ = await _unitOfWork.TrainingProgramRepository.GetByIdAsync(updateTrainingClassDTO.TrainingProgramId) ?? throw new NullReferenceException("Invalid training program Id");
            // admins
            if (!trainingClassObj.TrainingClassAdmins.IsNullOrEmpty())
            {
                foreach (var user in trainingClassObj.TrainingClassAdmins)
                {
                    if (!CheckTrainingClassAdminsIdAsync(user).Result)
                    {
                        throw new Exception("Incorrect admin id!");
                    }
                }
            }

            //trainers
            if (!trainingClassObj.TrainingClassTrainers.IsNullOrEmpty())
            {
                foreach (var user in trainingClassObj.TrainingClassTrainers)
                {
                    if (!CheckTrainingClassTrainersIdAsync(user).Result)
                    {
                        throw new Exception("Incorrect trainer id!");
                    }
                }
            }
            _unitOfWork.TrainingClassRepository.Update(trainingClassObj);
            return (await _unitOfWork.SaveChangeAsync() > 0);
        }

        /// <summary>
        /// This method find, return Training class and throw exception if can't find or get a mapping exception
        /// </summary>
        /// <param className="trainingClassId">Training class ID</param>
        /// <returns>Training class</returns>
        /// <exception cref="AutoMapperMappingException">When training class ID is not a guid</exception>
        public async Task<TrainingClass> GetTrainingClassByIdAsync(string trainingClassId)
        {
            try
            {
                var _classId = _mapper.Map<Guid>(trainingClassId);
                var trainingClassObj = await _unitOfWork.TrainingClassRepository.GetByIdAsync(_classId);
                return trainingClassObj ?? throw new NullReferenceException($"Incorrect Id: The training class with id: {trainingClassId} doesn't exist or has been deleted!");
            }
            catch (AutoMapperMappingException)
            {
                throw new AutoMapperMappingException("Incorrect Id!");
            }
        }

        /// <summary>
        /// GetAllTrainingClassesAsync returns all training classes
        /// </summary>
        /// <returns>List of training class</returns>
        public async Task<List<TrainingClassViewAllDTO>> GetAllTrainingClassesAsync()
        {
            var trainingClasses = _unitOfWork.TrainingClassRepository.GetTrainingClasses();
            return trainingClasses;
        }


        public async Task<List<TrainingClassViewAllDTO>> FilterLocation(string[]? locationName, string branchName, DateTime? date1, DateTime? date2, string[]? classStatus, string[]? attendInClass, string? trainerName)
        {
            ICriterias<TrainingClassFilterDTO> locationCriteria = new LocationCriteria(locationName);
            ICriterias<TrainingClassFilterDTO> dateCriteria = new DateCriteria(date1, date2);
            ICriterias<TrainingClassFilterDTO> branchCriteria = new ClassBranchCriteria(branchName);
            ICriterias<TrainingClassFilterDTO> statusCriteria = new StatusClassCriteria(classStatus);
            ICriterias<TrainingClassFilterDTO> attendCriteria = new AttendeeCriteria(attendInClass);
            ICriterias<TrainingClassFilterDTO> trainerCriteria = new CreatedByCriteria(trainerName);
            List<TrainingClassViewAllDTO> filterList = new List<TrainingClassViewAllDTO>();
            ICriterias<TrainingClassFilterDTO> andCirteria = new AndClassFilter(dateCriteria, locationCriteria, branchCriteria, statusCriteria, attendCriteria, trainerCriteria);
            var getAll = _unitOfWork.TrainingClassRepository.GetTrainingClassesForFilter();
            var filterResult = andCirteria.MeetCriteria(getAll);
            foreach (var item in filterResult)
            {
                var filterResultFormat = _mapper.Map<TrainingClassViewAllDTO>(item);

                filterList.Add(filterResultFormat);

            }
            return filterList;

        }

        public async Task<FinalTrainingClassDTO> GetFinalTrainingClassesAsync(Guid id)
        {
            FinalTrainingClassDTO finalDTO = new FinalTrainingClassDTO();
            var trainingClassFilterDetail = _unitOfWork.TrainingClassRepository.GetTrainingClassForViewDetailById(id);
            var trainingProgram = _unitOfWork.TrainingClassRepository.GetTrainingProgramByClassID(id);
            var detailProgramSyllabus = _unitOfWork.DetailTrainingProgramSyllabusRepository.GetDetailByClassID(trainingProgram.programId);
            var detailTrainingClass = await _unitOfWork.DetailTrainingClassParticipateRepository.GetDetailTrainingClassParticipatesByClassIDAsync(id);
            var adminInClass = await _unitOfWork.DetailTrainingClassParticipateRepository.GetAdminInClasssByClassIDAsync(id);
            if (trainingClassFilterDetail == null)
            {
                return null;
            }
            else
            {
                AttendeeDTO attendeeDTO = new AttendeeDTO()
                {
                    attendee = trainingClassFilterDetail.Attendee,
                    plannedNumber = 50,
                    acceptedNumber = 30,
                    actualNumber = 20,
                };
                CreatedByDTO createdByDTO = new CreatedByDTO()
                {
                    creationDate = trainingClassFilterDetail.CreationDate,
                    userName = trainingClassFilterDetail.CreatedBy
                };
                finalDTO.trainingPrograms = trainingProgram;
                finalDTO.syllabuses = detailProgramSyllabus;

                finalDTO.classId = trainingClassFilterDetail.ClassID;
                finalDTO.classCode = trainingClassFilterDetail.Code;
                finalDTO.classCode = trainingClassFilterDetail.Code;
                finalDTO.classStatus = trainingClassFilterDetail.Status;
                finalDTO.className = trainingClassFilterDetail.Name;
                finalDTO.dateOrder = new DateOrderForViewDetail
                {
                    current = 10,
                    total = 10 * 3
                };
                finalDTO.classDuration = trainingClassFilterDetail.ClassDuration;



                finalDTO.lastEdit = new LastEditDTO()
                {
                    modificationBy = trainingClassFilterDetail.LastEditDTO.modificationBy,
                    modificationDate = trainingClassFilterDetail.LastEditDTO.modificationDate
                };
                finalDTO.review = new TrainingClassReview
                {
                    reviewDate = DateTime.Now,
                    author = trainingClassFilterDetail.CreatedBy
                };
                finalDTO.admin = adminInClass;
                finalDTO.trainer = detailTrainingClass;
                finalDTO.location = trainingClassFilterDetail.LocationName;
                finalDTO.fsu = trainingClassFilterDetail.Branch;
                finalDTO.general = new GeneralTrainingClassDTO
                {
                    class_date = new ClassDateDTO
                    {
                        StartDate = trainingClassFilterDetail.StartDate,
                        EndDate = trainingClassFilterDetail.EndDate,
                    }
                };
                finalDTO.attendeesDetail = attendeeDTO;
                finalDTO.created = createdByDTO;


                return finalDTO;
            }
           
        }

        public async Task<List<TrainingClass>> ImportExcel(IFormFile file)
        {
            // chỉ nhận các file extension của excel
            var supportedTypes = new[] { "xls", "xlsx" };
            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                throw new Exception("You can only upload Excel files (.xls / .xlsx");
            }

            // license của EPPlus
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            try
            {
                var list = new List<TrainingClass>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowcount; row++)
                        {
                            try
                            {
                                List<Guid> guidList = new List<Guid>();
                                guidList.Add(new Guid(worksheet.Cells[row, 11].Value.ToString()));
                                guidList.Add(new Guid(worksheet.Cells[row, 9].Value.ToString()));
                                guidList.Add(new Guid(worksheet.Cells[row, 13].Value.ToString()));
                                list.Add(new TrainingClass
                                {
                                    Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                    StartTime = DateTime.Parse(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                    EndTime = DateTime.Parse(worksheet.Cells[row, 4].Value.ToString().Trim()),
                                    Code = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                    Duration = Double.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                    Attendee = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                    Branch = worksheet.Cells[row, 8].Value.ToString().Trim(),
                                    LocationID = guidList[1],
                                    StatusClassDetail = worksheet.Cells[row, 10].Value.ToString().Trim(),
                                    TrainingProgramId = guidList[0],
                                    CreationDate = DateTime.Parse(worksheet.Cells[row, 12].Value.ToString().Trim()),
                                    CreatedBy = guidList[2],
                                    IsDeleted = bool.Parse(worksheet.Cells[row, 14].Value.ToString().Trim()),
                                });
                            }
                            catch (NullReferenceException ex)
                            {
                                throw new Exception("Excel file is missing some data");

                            }
                        }
                        await _unitOfWork.TrainingClassRepository.AddRangeAsync(list);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    return list;
                }
            }
            catch (ArgumentException)
            {
                throw new Exception("Excel file has invalid data");
            }
        }

        /// <summary>
        /// CheckTrainingClassAdminsIdAsync check if the trainingclass admin id is qualified
        /// </summary>
        /// <param name="user">Training class admin</param>
        /// <returns>Return false if user doesn't exist or is not admin. Otherwise, return true</returns>
        public async Task<bool> CheckTrainingClassAdminsIdAsync(TrainingClassAdmin user)
        {
            var databaseUser = await _unitOfWork.UserRepository.GetByIdAsync(user.AdminId);
            if (databaseUser == null || databaseUser.RoleId != (int)RoleEnums.Admin)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// CheckTrainingClassTrainersIdAsync check if the trainingclass Trainer id is qualified
        /// </summary>
        /// <param name="user">Training class Trainer</param>
        /// <returns>Return false if user doesn't exist or is not Trainer. Otherwise, return true</returns>
        public async Task<bool> CheckTrainingClassTrainersIdAsync(TrainingClassTrainer user)
        {
            var databaseUser = await _unitOfWork.UserRepository.GetByIdAsync(user.TrainerId);
            if (databaseUser == null || databaseUser.RoleId != (int)RoleEnums.Trainer)
            {
                return false;
            }
            return true;
        }
    }

}
