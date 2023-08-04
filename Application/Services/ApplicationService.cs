using Application.Commons;
using Application.Interfaces;
using Application.Models.ApplicationModels;
using Application.Utils;
using Application.ViewModels.ApplicationViewModels;
using AutoMapper;
using Domain.Entities;
using Domain.Enums.Application;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Domain.Enums.Application.ApplicationFilterByEnum;

namespace Application.Services
{



    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _configuration;
        private readonly ICurrentTime _currentTime;
        private readonly IClaimsService _claimsService;
        public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration configuration, ICurrentTime currentTime, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _currentTime = currentTime;
            _claimsService = claimsService;
        }
        public async Task<bool> UpdateStatus(Guid id, bool status)
        {
            var Tset = await _unitOfWork.ApplicationRepository.GetByIdAsync(id);
            if (Tset != null)
            {
                Tset.Approved = status;
                _unitOfWork.ApplicationRepository.Update(Tset);
                await _unitOfWork.SaveChangeAsync();
                return true;

            }
            return false;
        }

        public async Task<bool> CreateApplication(ApplicationDTO applicationDTO)
        {

            //   var Test = _mapper.Map<Applications>(applicationDTO);
            var detailTrainingClass = await _unitOfWork.DetailTrainingClassParticipateRepository.GetDetailTrainingClassParticipateAsync(_claimsService.GetCurrentUserId, applicationDTO.TrainingClassID);
            if (detailTrainingClass != null)
            {
                Applications applications = new Applications()
                {
                    TrainingClassId = applicationDTO.TrainingClassID,
                    UserId = _claimsService.GetCurrentUserId,
                    AbsentDateRequested = applicationDTO.AbsentDateRequested,
                    Reason = applicationDTO.Reason,
                };

                await _unitOfWork.ApplicationRepository.AddAsync(applications);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;


        }

        public async Task<Pagination<Applications>> GetAllApplication(Guid classId, ApplicationFilterDTO filter, int pageIndex = 0, int pageSize = 10)
        {
            // null secure
            filter ??= new();

            filter.ByDateType.ThrowErrorIfNotValidEnum(typeof(ApplicationFilterByEnum), $"{nameof(filter.ByDateType)} is not valid");

            Expression<Func<Applications, bool>> expression = _unitOfWork.ApplicationRepository.GetFilterExpression(classId, filter);
            var applications = await _unitOfWork.ApplicationRepository.ToPagination(expression, pageIndex, pageSize);

            return applications.TotalPagesCount > 0 ? applications : null;
        }

    }
}
