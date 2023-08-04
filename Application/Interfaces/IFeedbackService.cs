using Application.ViewModels.FeedbackModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IFeedbackService
{
    Task<List<FeedbackVM>> GetFeedbacksAsync();
    Task<FeedbackVM> GetFeedbackByIdAsync(Guid id);
    Task CreateFeedbackAsync(FeedbackModel model);
    Task<bool> UpdateFeedbackAsync(Guid feedbackId, FeedbackModel model);
    Task<bool> DeleteFeedbackAsync(Guid id);
    Task<bool> SendFeedbacklink(Guid feedbackId);
}
