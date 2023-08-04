using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITopicService
    {
        Task<bool> AddNewTopic(TopicModel topic);

        Task<bool> DeleteTopic(Guid TopicID);

        Task<List<Topic>> ViewAllTopic();

        Task<bool> UpdateTopic(Guid TopicID, string TopicNameChange);
    }
}
