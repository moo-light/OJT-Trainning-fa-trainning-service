using Application.Interfaces;
using Application.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class DetailQuizQuestionRepository : GenericRepository<DetailQuizQuestion>, IDetailQuizQuestionRepository
    {
        private readonly AppDbContext _appDbContext;
        public DetailQuizQuestionRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _appDbContext = context;
        }
    }
}
