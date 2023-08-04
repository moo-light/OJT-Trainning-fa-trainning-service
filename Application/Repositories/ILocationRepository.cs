using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        public Task<Location?> GetByNameAsync(string locationName, params Expression<Func<Location, object>>[] includes);
    }
}
