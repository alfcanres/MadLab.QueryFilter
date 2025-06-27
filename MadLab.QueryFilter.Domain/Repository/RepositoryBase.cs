

using Microsoft.EntityFrameworkCore;

namespace MadLab.QueryFilter.Domain.Repository
{
    public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class    
    {
        protected readonly DataBaseContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public RepositoryBase(DataBaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = _dbSet.Find(id);
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

    }
}
