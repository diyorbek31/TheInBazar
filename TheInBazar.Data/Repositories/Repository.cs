using Microsoft.EntityFrameworkCore;
using TheInBazar.Data.DbContexts;
using TheInBazar.Data.IRepositories;
using TheInBazar.Domain.Commons;

namespace TheInBazar.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Auditable
{
    private readonly AppDbContext dbContext;
    private readonly DbSet<TEntity> dbSet;

    public Repository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = dbContext.Set<TEntity>();
    }
    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await this.dbSet.FirstOrDefaultAsync(e => e.Id == id);
        this.dbSet.Remove(entity);
        return true;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        var model = await this.dbSet.AddAsync(entity);
        
        return model.Entity;
    }

    public async Task<bool> SaveChangeAsync()
    {
        return await this.dbContext.SaveChangesAsync() > 0;
    }

    public IQueryable<TEntity> SelectAll()
    {
        return this.dbSet;
    }

    public async Task<TEntity> SelectByIdAsync(long id)
        => await this.dbSet.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var model = this.dbSet.Update(entity);
        return model.Entity;
    }
}
