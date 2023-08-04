using Application.Interfaces;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class TopicController : BaseController
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopic()
        {
            var TopicList = await _topicService.ViewAllTopic();
            if (TopicList is not null)
            {
                return Ok(TopicList);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTopic(TopicModel topic)
        {
            var topicAdd = await _topicService.AddNewTopic(topic);
            if (topicAdd)
            {
                return Ok(await _topicService.ViewAllTopic());
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTopic(Guid topicID)
        {
            var TopicFind = await _topicService.DeleteTopic(topicID);
            if (TopicFind)
            {
                return Ok(await _topicService.ViewAllTopic());
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTopic(Guid topicID, string topicName)
        {
            var TopicFind = await _topicService.UpdateTopic(topicID, topicName);
            if (TopicFind)
            {
                return Ok(await _topicService.ViewAllTopic());
            }
            return BadRequest();
        }
    }
}
