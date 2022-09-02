using Microsoft.EntityFrameworkCore;
using OpenChat.Domain.Common;
using OpenChat.Infrastructure.Persistence;

public abstract class EfGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly OpenChatDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public EfGenericRepository(OpenChatDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual TEntity? GetById(object id)
    {
        return _dbSet.Find(id);
    }

    public virtual void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        _dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}