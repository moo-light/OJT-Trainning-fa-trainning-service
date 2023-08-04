using Application.Interfaces;
using Application.Repositories;
using Application.Utils;
using Application.ViewModels.FeedbackModels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories;

public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    private readonly AppDbContext _dbContext;

    public FeedbackRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
    {
        _dbContext = context;
    }

    public List<string> GetTraineeEmailsOfClass(Guid classId)
    {
        var detailTraineeClass = _dbContext.DetailTrainingClassParticipates.Include("User").Where(x => x.TrainingClassID == classId).ToList();
        List<string> emailList = null;
        foreach (var d in detailTraineeClass)
        {
            if (emailList == null)
            {
                emailList = new List<string>();
            }
            emailList.Add(d.User.Email);
        }
        return emailList;
    }
}
