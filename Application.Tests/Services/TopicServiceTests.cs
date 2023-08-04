using Application.Interfaces;
using Application.Services;
using Application.ViewModels.QuizModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class TopicServiceTests:SetupTest
    {
        private readonly ITopicService _topicService;
        
        public TopicServiceTests()
        {
            _topicService = new TopicService(_unitOfWorkMock.Object, _claimsServiceMock.Object);
        }

        [Fact]
        public async Task AddNewTopic_ShouldReturnFalse_WhenCheckDuplicateis1()
        {
            var topicModelMock = _fixture.Build<TopicModel>().Create();
            //var mapperTopicModel=_mapperConfig.Map<Topic>(topicModelMock);
            var listTopicMock = _fixture.Build<Topic>()
                                        .Without(x => x.QuizBanks)
                                        .CreateMany(1).ToList();

            _unitOfWorkMock.Setup(x => x.TopicRepository.FindAsync(x=>x.TopicName==topicModelMock.TopicName)).ReturnsAsync(listTopicMock);


            var actualResult =await _topicService.AddNewTopic(topicModelMock);

            actualResult.Should().BeFalse();
            
        }

        [Fact]
        public async Task AddNewTopic_ShouldReturnTrue_WhenCheckDuplicateis0()
        {
            var topicModelMock = _fixture.Build<TopicModel>().Create();
            Topic newTopicModel = new Topic
            {
                TopicName = topicModelMock.TopicName,
                Id = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                IsDeleted = false
            };
            var listTopicMock = new List<Topic>();

            _unitOfWorkMock.Setup(x => x.TopicRepository.FindAsync(x => x.TopicName == topicModelMock.TopicName)).ReturnsAsync(listTopicMock);
            _unitOfWorkMock.Setup(x => x.TopicRepository.AddAsync(newTopicModel));

            var actualResult = await _topicService.AddNewTopic(topicModelMock);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTopic_ShouldReturnTrue_WhenTopicFindisNotNull()
        {
            var topicFindMock = _fixture.Build<Topic>()
                                    .Without(x=>x.QuizBanks)
                                    .Create();
            Guid id = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.TopicRepository.GetByIdAsync(id)).ReturnsAsync(topicFindMock);
            _unitOfWorkMock.Setup(x => x.TopicRepository.SoftRemove(topicFindMock)).Verifiable();

            var actualResult = await _topicService.DeleteTopic(id);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTopic_ShouldReturnFalse_WhenTopicFindisNull()
        {
            Topic topicFindMock = new Topic();
            Guid id = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.TopicRepository.GetByIdAsync(id)).ReturnsAsync(topicFindMock=null);
            _unitOfWorkMock.Setup(x => x.TopicRepository.SoftRemove(topicFindMock)).Verifiable();

            var actualResult = await _topicService.DeleteTopic(id);

            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task ViewAllTopic_ShouldReturnTopicList()
        {
            var topicListMock = _fixture.Build<Topic>()
                                    .Without(x=>x.QuizBanks)
                                    .CreateMany(2).ToList();

            _unitOfWorkMock.Setup(x => x.TopicRepository.GetAllAsync()).ReturnsAsync(topicListMock);

            var actualResult = await _topicService.ViewAllTopic();

            actualResult.Should().BeOfType<List<Topic>>();
        }

        [Fact]
        public async Task UpdateTopic_ShouldReturnTrue_WhenTopicFindisNotNull()
        {
            Guid id = Guid.NewGuid();
            string topicNameChange = "a";

            var topicMock = _fixture.Build<Topic>().Without(x => x.QuizBanks).Create();

            _unitOfWorkMock.Setup(x => x.TopicRepository.GetByIdAsync(id)).ReturnsAsync(topicMock);

            topicMock.TopicName = topicNameChange;
            _unitOfWorkMock.Setup(x => x.TopicRepository.Update(topicMock)).Verifiable();

            var actualResult = await _topicService.UpdateTopic(id, topicNameChange);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateTopic_ShouldReturnFalse_WhenTopicFindisNull()
        {
            Guid id = Guid.NewGuid();
            string topicNameChange = "a";

            var topicMock = _fixture.Build<Topic>().Without(x => x.QuizBanks).Create();

            _unitOfWorkMock.Setup(x => x.TopicRepository.GetByIdAsync(id)).ReturnsAsync(topicMock=null);

            //topicMock.TopicName = topicNameChange;
            _unitOfWorkMock.Setup(x => x.TopicRepository.Update(topicMock)).Verifiable();

            var actualResult = await _topicService.UpdateTopic(id, topicNameChange);

            actualResult.Should().BeFalse();
        }
    }
}
