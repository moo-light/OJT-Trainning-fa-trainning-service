using Application.Commons;
using Application.ViewModels.AtttendanceModels;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void SoftRemove(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void SoftRemoveRange(List<TEntity> entities);


        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        Task<Pagination<TEntity>> ToPagination(int pageIndex = 0, int pageSize = 10);
        Task<Pagination<TEntity>> ToPagination(Expression<Func<TEntity, bool>> expression , int pageIndex = 0, int pageSize = 10);
        Task<Pagination<TEntity>> ToPagination(IQueryable<TEntity> value, Expression<Func<TEntity, bool>> expression, int pageIndex, int pageSize);
    }
}
