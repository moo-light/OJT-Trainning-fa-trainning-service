using Application.Interfaces;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TopicService : ITopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;

        public TopicService(IUnitOfWork unitOfWork, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
        }

        public async Task<bool> AddNewTopic(TopicModel topic)
        {
            //throw new Exception();
            var checkDuplicate = await _unitOfWork.TopicRepository.FindAsync(x => x.TopicName == topic.TopicName);

            Topic NewTopic = new Topic
            {
                TopicName = topic.TopicName,
                Id = new Guid(),
                CreationDate = DateTime.Now
                ,
                IsDeleted = false
            };
            if (checkDuplicate.Count < 1)
            {
                await _unitOfWork.TopicRepository.AddAsync(NewTopic);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            return false;

        }

        public async Task<bool> DeleteTopic(Guid TopicID)
        {
            //throw new Exception();
            var TopicFind = await _unitOfWork.TopicRepository.GetByIdAsync(TopicID);
            if (TopicFind is not null)
            {

                //TopicFind.DeleteBy =
                _unitOfWork.TopicRepository.SoftRemove(TopicFind);
                await _unitOfWork.SaveChangeAsync();
                return true;

            }
            return false;
        }

        public async Task<List<Topic>> ViewAllTopic()
        {
            var TopicList = await _unitOfWork.TopicRepository.GetAllAsync();
            return TopicList;
        }

        public async Task<bool> UpdateTopic(Guid TopicID, string TopicNameChange)
        {
            //throw new Exception();
            var TopicFind = await _unitOfWork.TopicRepository.GetByIdAsync(TopicID);
            if (TopicFind is not null)
            {
                TopicFind.TopicName = TopicNameChange;
                _unitOfWork.TopicRepository.Update(TopicFind);
                await _unitOfWork.SaveChangeAsync();
                return true;
            }
            return false;
        }
    }
}
