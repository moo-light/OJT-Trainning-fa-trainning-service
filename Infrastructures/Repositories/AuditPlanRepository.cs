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
    public class AuditPlanRepository : GenericRepository<AuditPlan>, IAuditPlanRepository
    {
        private readonly AppDbContext _dbContext;

        public AuditPlanRepository(AppDbContext dbContext,
        ICurrentTime timeService,
        IClaimsService claimsService)
        : base(dbContext,
              timeService,
              claimsService)
        {
            _dbContext = dbContext;
        }
    }
}
