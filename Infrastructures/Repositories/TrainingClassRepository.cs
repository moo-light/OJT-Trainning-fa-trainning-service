using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using AutoMapper.Internal;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    /// <summary>
    /// Training class repository
    /// </summary>
    public class TrainingClassRepository : GenericRepository<TrainingClass>, ITrainingClassRepository
    {
        private readonly AppDbContext _dbContext;
        public TrainingClassRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }


        public TrainingProgramViewForTrainingClassDetail GetTrainingProgramByClassID(Guid id)
        {
            var getAllTrainingProgram = _dbContext.TrainingClasses
                                       .Include(x => x.TrainingProgram)

                                       .Where(x => x.Id == id)
                                      .Select(x => new TrainingProgramViewForTrainingClassDetail()
                                      {
                                          programId = x.TrainingProgram.Id,
                                          programName = x.TrainingProgram.ProgramName,
                                          programDuration = new DurationView
                                          {
                                              TotalHours = x.TrainingProgram.Duration
                                          },
                                          lastEdit = new LastEditDTO
                                          {
                                              modificationBy = _dbContext.Users.Where(u => u.Id == x.TrainingProgram.ModificationBy).Select(u => u.UserName).FirstOrDefault(),
                                              modificationDate = x.TrainingProgram.ModificationDate
                                          }
                                      }).FirstOrDefault();
            return getAllTrainingProgram;

        }
        public List<TrainingClassFilterDTO> GetTrainingClassesForFilter()
        {
            var getAllTrainingClass = _dbContext.TrainingClasses
                                        .Where(x => x.IsDeleted == false)
                                     .Select(t => new TrainingClassFilterDTO
                                     {
                                         ClassID = t.Id,
                                         Name = t.Name,
                                         LocationName = t.Location.LocationName,
                                         CreationDate = t.CreationDate,
                                         Code = t.Code,
                                         Branch = t.Branch,
                                         StartDate = t.StartTime,
                                         EndDate = t.EndTime,
                                         Attendee = t.Attendee,
                                         ClassDuration = new DurationView
                                         {
                                             TotalHours = t.Duration
                                         },
                                         Status = t.StatusClassDetail,
                                         CreatedBy = string.Join(",", _dbContext.Users.Where(x => x.Id == t.CreatedBy).Select(u => u.UserName))
                                     }).ToList();
            return getAllTrainingClass;
        }

        public List<TrainingClassViewAllDTO> GetTrainingClasses()
        {
            var getAllTrainingClass = _dbContext.TrainingClasses
                                       .Include(t=>t.TrainingClassParticipates)
                                       .ThenInclude(participate=>participate.User)
                                       .Where(t=>t.IsDeleted==false)
                                     .Select(t => new TrainingClassViewAllDTO
                                     {
                                         id = t.Id,
                                         className = t.Name,
                                         location = t.Location.LocationName,
                                         createdOn = t.CreationDate,
                                         classCode = t.Code,
                                         fsu = t.Branch,
                                         attendee = t.Attendee,
                                         classDuration = new DurationView
                                         {
                                             TotalHours = t.Duration
                                         },
                                         createdBy = string.Join(",", t.TrainingClassParticipates.Select(parti=>parti.User).Select(u=>u.UserName))
                                     }).ToList();
            return getAllTrainingClass;
        }

        /// <summary>
        /// SearchClassByName find and return training classes
        /// which name are the same as the name parameter
        /// </summary>
        /// <param name="name">name of a training class</param>
        /// <returns>List of training classes</returns>
        public List<TrainingClassViewAllDTO> SearchClassByName(string name)
        {
            if (name.IsNullOrEmpty())
            {
                var searchClass =  GetTrainingClasses();
                return searchClass;
            }
            var classByName=_dbContext.TrainingClasses
                             .Where(t=>t.Name.ToLower().Contains(name.ToLower())&&t.IsDeleted==false)
                             .Select(t => new TrainingClassViewAllDTO
                              {
                                  id = t.Id,
                                  className = t.Name,
                                  location = t.Location.LocationName,
                                  createdOn = t.CreationDate,
                                  classCode = t.Code,
                                  fsu = t.Branch,
                                  attendee = t.Attendee,
                                  classDuration = new DurationView
                                  {
                                      TotalHours = t.Duration
                                  },
                                  createdBy = string.Join(",", _dbContext.Users.Where(x => x.Id == t.CreatedBy).Select(u => u.UserName))
                              }).ToList();
            return classByName;
        }

        /// <summary>
        /// GetByIdAsync return training class by id
        /// </summary>
        /// <param name="id">Training class id</param>
        /// <returns>Training class</returns>
        public async Task<TrainingClass?> GetByIdAsync(Guid id)
        {
            return await GetByIdAsync(id,
                    x => x.TrainingClassAdmins,
                    x => x.TrainingClassTrainers,
                    x => x.TrainingClassTimeFrame.HighlightedDates,
                    x => x.TrainingClassAttendee);
        }

        /// <summary>
        /// AddAsync add new training class to db
        /// </summary>
        /// <param name="trainingClass">New training class</param>
        /// <returns></returns>
        public new async Task AddAsync(TrainingClass trainingClass)
        {
            // timeframe
            if (trainingClass.TrainingClassTimeFrame != null)
            {
                trainingClass.TrainingClassTimeFrame.TrainingClassId = trainingClass.Id;
                if (!trainingClass.TrainingClassTimeFrame.HighlightedDates.IsNullOrEmpty())
                {
                    foreach (var hilightDate in trainingClass.TrainingClassTimeFrame.HighlightedDates!)
                    {
                        hilightDate.TrainingClassTimeFrameId = trainingClass.TrainingClassTimeFrame.Id;
                    }
                }
            }

            // attendees
            if (trainingClass.TrainingClassAttendee != null)
            {
                trainingClass.TrainingClassAttendee.TrainingClassId = trainingClass.Id;
            }

            await base.AddAsync(trainingClass);
        }
        public new async void Update(TrainingClass trainingClass)
        {
            if (!trainingClass.TrainingClassAdmins.IsNullOrEmpty())
            {
                List<TrainingClassAdmin> admins = new();
                foreach (var admin in trainingClass.TrainingClassAdmins)
                {
                    var duplicate = _dbContext.TrainingClassAdmins
                        .Where(x => x.TrainingClassId == trainingClass.Id
                        && x.AdminId == admin.AdminId
                        && x.Id != admin.Id);
                    if (duplicate.Any())
                    {
                        admins.TryAdd(admin);
                    }
                }
                admins.ForEach(x => trainingClass.TrainingClassAdmins.Remove(x));
            }
            if (!trainingClass.TrainingClassTrainers.IsNullOrEmpty())
            {
                List<TrainingClassTrainer> trainers = new();
                foreach (var trainer in trainingClass.TrainingClassTrainers)
                {
                    var duplicate = await _dbContext.TrainingClassTrainers
                        .AnyAsync(x => x.TrainingClassId == trainingClass.Id
                        && x.TrainerId == trainer.TrainerId
                        && x.Id != trainer.Id);
                    if (duplicate)
                    {
                        trainers.TryAdd(trainer);
                    }
                }
                trainers.ForEach(x => trainingClass.TrainingClassTrainers.Remove(x));
            }
            if (trainingClass.TrainingClassTimeFrame != null)
            {
                if (!trainingClass.TrainingClassTimeFrame.HighlightedDates.IsNullOrEmpty())
                {
                    List<HighlightedDates> duplicateDates = new List<HighlightedDates>();
                    foreach (var highlightDates in trainingClass.TrainingClassTimeFrame.HighlightedDates!)
                    {
                        var duplicate = _dbContext.HighlightedDates.AsEnumerable()
                            .Any(x => x.TrainingClassTimeFrameId == trainingClass.TrainingClassTimeFrame.Id
                            && (x.HighlightedDate - highlightDates.HighlightedDate).Days == 0
                            && x.Id != highlightDates.Id);
                        if (duplicate)
                        {
                            duplicateDates.TryAdd(highlightDates);
                        }
                    }
                    duplicateDates.ForEach(x => trainingClass.TrainingClassTimeFrame.HighlightedDates.Remove(x));
                }
            }
            base.Update(trainingClass);
        }
        public TrainingClassFilterDTO GetTrainingClassForViewDetailById(Guid id)
        {
            var getAllTrainingClass = _dbContext.TrainingClasses
                             .Where(x => x.IsDeleted == false && x.Id == id)

                          .Select(t => new TrainingClassFilterDTO
                          {
                              ClassID = t.Id,
                              Name = t.Name,
                              LocationName = t.Location.LocationName,
                              CreationDate = t.CreationDate,
                              Code = t.Code,
                              Branch = t.Branch,
                              StartDate = t.StartTime,
                              EndDate = t.EndTime,
                              Attendee = t.Attendee,
                              ClassDuration = new DurationView
                              {
                                  TotalHours = t.Duration
                              },
                              LastEditDTO = new LastEditDTO
                              {
                                  modificationBy = _dbContext.Users.Where(u => u.Id == t.ModificationBy).Select(u => u.UserName).SingleOrDefault(),
                                  modificationDate = t.ModificationDate,
                              },
                              Status = t.StatusClassDetail,
                              CreatedBy = _dbContext.Users.Where(x => x.Id == t.CreatedBy).Select(u => u.UserName).SingleOrDefault()
                          });
            TrainingClassFilterDTO trainingClassFilterDTO = getAllTrainingClass.FirstOrDefault();
            return trainingClassFilterDTO;
        }
    }
}
