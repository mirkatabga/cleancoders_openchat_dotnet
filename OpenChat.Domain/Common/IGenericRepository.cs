namespace OpenChat.Domain.Common
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity? GetById(object id);

        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);
    }
}
