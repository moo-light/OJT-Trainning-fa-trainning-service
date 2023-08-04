using Application;
using Application.Interfaces;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UnitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<IEnumerable<Unit>> GetSyllabusDetail(Guid syllabusID)
        {

            var listUnit = await _unitOfWork.UnitRepository.GetAllAsync();
            if (listUnit is null)
            {
                throw new Exception("Not Found");
            }
            return listUnit;


        }

        public async Task<Unit> AddNewUnit(UnitDTO unitDTO, Syllabus syllabus)
        {
            var NewUnit = new Unit()
            {
                Id = Guid.NewGuid(),
                UnitName = unitDTO.UnitName,
                TotalTime = unitDTO.TotalTime,
                Session = unitDTO.Session,
                IsDeleted = false,
                Syllabus = syllabus
            };
            throw new Exception();
            await _unitOfWork.UnitRepository.AddAsync(NewUnit);
            return NewUnit;
        }


        public async Task<Unit> AddNewUnitHotFix(UpdateContentModel updateSyllabusModel, int session, Guid syllabusID)
        {

            var unitMapper = _mapper.Map<Unit>(updateSyllabusModel);
            unitMapper.Id = Guid.NewGuid();
            unitMapper.Session = session;
            unitMapper.SyllabusID = syllabusID;
            await _unitOfWork.UnitRepository.AddAsync(unitMapper);
            return unitMapper;
        }
    }
}
