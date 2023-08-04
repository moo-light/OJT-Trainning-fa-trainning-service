using Application.Interfaces;
using Application.ViewModels.AuditModels;
using Application.ViewModels.AuditModels.UpdateModels;
using Application.ViewModels.AuditModels.ViewModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditPlanService : IAuditPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuditPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AuditPlan?> CreateAuditPlan(CreateAuditDTO createAuditDTO)
        {
            var auditPlan = _mapper.Map<AuditPlan>(createAuditDTO);
            auditPlan.Id = Guid.NewGuid();
            await _unitOfWork.AuditPlanRepository.AddAsync(auditPlan);
            if (await _unitOfWork.SaveChangeAsync() > 0)
            {
                var questions = createAuditDTO.CreateAuditQuestionDTOS;
                if (questions is not null)
                {
                    foreach (var questionDTO in questions)
                    {
                        var question = _mapper.Map<AuditQuestion>(questionDTO);
                        if (question is not null)
                        {
                            question.Id = Guid.NewGuid();
                            await _unitOfWork.AuditQuestionRepository.AddAsync(question);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                var detailAuditQuestion = new DetailAuditQuestion { Id = Guid.NewGuid(), AuditPlanId = auditPlan.Id, AuditQuestionId = question.Id };
                                await _unitOfWork.DetailAuditQuestionRepository.AddAsync(detailAuditQuestion);
                            }
                        }
                    }
                    if (await _unitOfWork.SaveChangeAsync() > 0) return await _unitOfWork.AuditPlanRepository.GetByIdAsync(auditPlan.Id);
                }
                else
                {
                    throw new Exception("Questions can not null");
                }
            }
            throw new Exception("Create Audit Plan Failed! Try Again!");
        }

        public async Task<bool> DeleteAuditPlan(Guid auditPlanId)
        {
            var auditPlan = await _unitOfWork.AuditPlanRepository.GetByIdAsync(auditPlanId);
            if (auditPlan is not null)
            {
                _unitOfWork.AuditPlanRepository.SoftRemove(auditPlan);
                var detailAuditQuestions = await _unitOfWork.DetailAuditQuestionRepository.FindAsync(x => x.AuditPlanId == auditPlanId);
                if (detailAuditQuestions is not null) _unitOfWork.DetailAuditQuestionRepository.SoftRemoveRange(detailAuditQuestions);
                if (await _unitOfWork.SaveChangeAsync() > 0) return true;
            }
            return false;
        }

        public async Task<AuditPlan> GetAuditPlanById(Guid auditId)
        {
            var result = await _unitOfWork.AuditPlanRepository.GetByIdAsync(auditId);
            if (result is not null) return result;
            else throw new Exception("Not found");
        }

        public async Task<bool> UpdateAuditPlan(UpdateAuditDTO updateAuditDTO)
        {
            var updateAuditPlan = await _unitOfWork.AuditPlanRepository.GetByIdAsync(updateAuditDTO.Id);
            if (updateAuditPlan is not null)
            {
                _ = _mapper.Map(updateAuditDTO, updateAuditPlan, typeof(UpdateAuditDTO), typeof(AuditPlan));
                _unitOfWork.AuditPlanRepository.Update(updateAuditPlan);

                var detailAuditQuestions = await _unitOfWork.DetailAuditQuestionRepository.FindAsync(x => x.AuditPlanId == updateAuditPlan.Id);
                if (detailAuditQuestions is not null) _unitOfWork.DetailAuditQuestionRepository.SoftRemoveRange(detailAuditQuestions);


                var questions = updateAuditDTO.CreateAuditQuestionDTOS;
                if (questions is not null)
                {
                    foreach (var questionDTO in questions)
                    {
                        var question = _mapper.Map<AuditQuestion>(questionDTO);
                        if (question is not null)
                        {
                            question.Id = Guid.NewGuid();
                            await _unitOfWork.AuditQuestionRepository.AddAsync(question);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                var detailAuditQuestion = new DetailAuditQuestion { Id = Guid.NewGuid(), AuditPlanId = updateAuditDTO.Id, AuditQuestionId = question.Id };
                                await _unitOfWork.DetailAuditQuestionRepository.AddAsync(detailAuditQuestion);
                            }

                        }
                    }
                    if (await _unitOfWork.SaveChangeAsync() > 0) return true;
                    else throw new Exception("Save Changes Fail");
                }
            }
            throw new Exception("Cant find any Audit Plan");

        }

        public async Task<AuditPlanViewModel?> ViewDetailAuditPlan(Guid auditId)
        {
            var auditPlan = await _unitOfWork.AuditPlanRepository.GetByIdAsync(auditId);
            if (auditPlan is not null)
            {
                var auditView = _mapper.Map<AuditPlanViewModel>(auditPlan);

                var questionsView = await _unitOfWork.DetailAuditQuestionRepository.GetAuditQuestionsByAuditId(auditId);
                auditView.AuditQuestions = questionsView;
                return auditView;
            }
            throw new Exception("Not found! AuditPlan is not existed or has been deleted");

        }
    }
}
