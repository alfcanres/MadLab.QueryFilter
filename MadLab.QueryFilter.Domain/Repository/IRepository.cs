namespace MadLab.QueryFilter.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(int id);      
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        IQueryable<TEntity> Query();

    }
}
