using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LectureService : ILectureService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LectureService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Lecture> AddNewLecture(LectureDTO lecture)
        {
            Lecture NewLecture = new Lecture()
            {
                Id = Guid.NewGuid(),
                LectureName = lecture.LectureName,
                OutputStandards = lecture.OutputStandards,
                Duration = lecture.Duration,
                DeliveryType = lecture.DeliveryType,
                Status = lecture.Status,
            };
            await _unitOfWork.LectureRepository.AddAsync(NewLecture);
            return NewLecture;
        }

        public async Task<DetailUnitLecture> AddNewDetailLecture(Lecture lecture, Unit unit)
        {
            DetailUnitLecture NewLecture = new DetailUnitLecture()
            {
                UnitId = unit.Id,
                LectureID = lecture.Id,
                CreatedBy = lecture.CreatedBy,
                IsDeleted = false
                ,Id = Guid.NewGuid()
            };
            await _unitOfWork.DetailUnitLectureRepository.AddAsync(NewLecture);

            return NewLecture;


        }

        public async Task<Lecture> AddNewLectureHotFix(UpdateLessonModel updateLessonModel)
        {
            var lectureMapper = _mapper.Map<Lecture>(updateLessonModel);
            lectureMapper.Id = Guid.NewGuid();
            await _unitOfWork.LectureRepository.AddAsync(lectureMapper);
            return lectureMapper;
        }

        

        

    }

}

