using Application.Interfaces;
using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuditSubmissionService : IAuditSubmissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AuditSubmissionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AuditSubmission> CreateAuditSubmission(CreateAuditSubmissionDTO createAuditSubmissionDTO)
        {
            var auditSubmission = _mapper.Map<AuditSubmission>(createAuditSubmissionDTO);
            if (auditSubmission is not null)
            {
                auditSubmission.Id = Guid.NewGuid();
                await _unitOfWork.AuditSubmissionRepository.AddAsync(auditSubmission);
                if (await _unitOfWork.SaveChangeAsync() <= 0) throw new Exception("Save Changes Failed");
                var detailSubmissionDTO = createAuditSubmissionDTO.AuditSubmissions;
                if (detailSubmissionDTO is not null)
                {
                    var detailSubmissionList = new List<DetailAuditSubmission>();
                    foreach (var detail in detailSubmissionDTO)
                    {
                        var detailQuestion = await _unitOfWork.DetailAuditQuestionRepository.GetByIdAsync(detail.DetailQuesionId);
                        if (detailQuestion is not null)
                        {
                            detailSubmissionList.Add(new DetailAuditSubmission { Id = Guid.NewGuid(), Answer = detail.Answer, AuditSubmissionId = auditSubmission.Id, DetailAuditQuestionId = detailQuestion.Id });
                        }
                    }
                    await _unitOfWork.DetailAuditSubmissionRepository.AddRangeAsync(detailSubmissionList);
                    if (await _unitOfWork.SaveChangeAsync() > 0) return await _unitOfWork.AuditSubmissionRepository.GetByIdAsync(auditSubmission.Id);
                }
                else throw new Exception("Not have any Detail! Please try again");


            }
            else throw new AutoMapperMappingException();

            throw new Exception("Create AuditSubmission Failed");
        }

        public async Task<bool> DeleteSubmissionDetail(Guid auditSubmissionId)
        {
            var auditSubmission = await _unitOfWork.AuditSubmissionRepository.GetByIdAsync(auditSubmissionId);
            if (auditSubmission is not null)
            {
                _unitOfWork.AuditSubmissionRepository.SoftRemove(auditSubmission);
                var submissionDetail = await _unitOfWork.DetailAuditSubmissionRepository.FindAsync(x => x.AuditSubmissionId == auditSubmission.Id && x.IsDeleted == false);
                if (submissionDetail is not null)
                {
                    _unitOfWork.DetailAuditSubmissionRepository.SoftRemoveRange(submissionDetail);
                }
                if (await _unitOfWork.SaveChangeAsync() > 0) return true;

            }
            throw new Exception("Not Found any Submission");
        }

        public async Task<IEnumerable<AuditSubmissionViewModel>> GetAllAuditSubmissionByAuditPlan(Guid auditPlanId)
        {
            var auditSubmissionViewList = new List<AuditSubmissionViewModel>();
            var auditSubmissions = await _unitOfWork.AuditSubmissionRepository.FindAsync(x => x.AuditPlanId == auditPlanId);
            if(auditSubmissions.Count() <= 0) throw new Exception("Not have any AuditSubmission");
            foreach (var auditSubmission in auditSubmissions)
            {
                var auditSubmissionView = _mapper.Map<AuditSubmissionViewModel>(auditSubmission);
                var auditSubmissionDetail = await _unitOfWork.DetailAuditSubmissionRepository.GetDetailView(auditSubmission.Id);
                if(auditSubmissionDetail is not null && auditSubmissionDetail.Count() > 0) 
                {
                    auditSubmissionView.DetailAuditSubmisisonViewModel = auditSubmissionDetail;
                    if (auditSubmissionView.DetailAuditSubmisisonViewModel is not null) auditSubmissionViewList.Add(auditSubmissionView);
                }
                
                    else throw new Exception("Not have any detail submission");                
            }
            return auditSubmissionViewList;
        }

        public async Task<AuditSubmissionViewModel> GetAuditSubmissionDetail(Guid auditSubmissionId)
        {
            var auditSubmission = await _unitOfWork.AuditSubmissionRepository.GetByIdAsync(auditSubmissionId);
            var auditSubmissionView = _mapper.Map<AuditSubmissionViewModel>(auditSubmission);
            if (auditSubmissionView is not null)
            {
                auditSubmissionView.DetailAuditSubmisisonViewModel = await _unitOfWork.DetailAuditSubmissionRepository.GetDetailView(auditSubmissionId);
                if (auditSubmissionView.DetailAuditSubmisisonViewModel is not null) return auditSubmissionView;
                else throw new Exception("Not have any detail submission");
            }
            throw new Exception("Can not find any Submission");
        }

        public async Task<bool> UpdateSubmissionDetail(UpdateSubmissionDTO updateSubmisionDTO)
        {
            var updatedItem = await _unitOfWork.AuditSubmissionRepository.GetByIdAsync(updateSubmisionDTO.Id);
            if (updatedItem is not null)
            {
                _ = _mapper.Map(updateSubmisionDTO, updatedItem, typeof(UpdateSubmissionDTO), typeof(AuditSubmission));
                _unitOfWork.AuditSubmissionRepository.Update(updatedItem);
                var submissionDetail = await _unitOfWork.DetailAuditSubmissionRepository.FindAsync(x => x.AuditSubmissionId == updateSubmisionDTO.Id && x.IsDeleted == false);
                if (submissionDetail is not null)
                {
                    _unitOfWork.DetailAuditSubmissionRepository.SoftRemoveRange(submissionDetail);
                }
                var detailSubmissionDTO = updateSubmisionDTO.AuditSubmissions;
                if (detailSubmissionDTO is not null)
                {
                    var detailSubmissionList = new List<DetailAuditSubmission>();
                    foreach (var detail in detailSubmissionDTO)
                    {
                        var detailQuestion = await _unitOfWork.DetailAuditQuestionRepository.GetByIdAsync(detail.DetailQuesionId);
                        if (detailQuestion is not null)
                        {
                            detailSubmissionList.Add(new DetailAuditSubmission { Id = Guid.NewGuid(), Answer = detail.Answer, AuditSubmissionId = updateSubmisionDTO.Id, DetailAuditQuestionId = detailQuestion.Id });
                        }
                    }
                    await _unitOfWork.DetailAuditSubmissionRepository.AddRangeAsync(detailSubmissionList);
                    if (await _unitOfWork.SaveChangeAsync() > 0) return true;
                }
            }
            return false;
        }
    }
}
