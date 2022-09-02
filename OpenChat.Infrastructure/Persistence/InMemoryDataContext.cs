using System.Reflection;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;

namespace OpenChat.Infrastructure.Persistence
{
    public class InMemoryDataContext
    {
        private readonly static PropertyInfo[] _properties;

        static InMemoryDataContext()
        {
            _properties = typeof(InMemoryDataContext).GetProperties();
        }
         
        public HashSet<User> Users { get; } = new HashSet<User>();

        public HashSet<Post> Posts { get; } = new HashSet<Post>();

        public HashSet<Following> Followings { get; } = new HashSet<Following>();

        public void SaveChanges()
        {
        }

        internal HashSet<TEntity> Set<TEntity>() where TEntity : class
        {
            foreach (var property in _properties)
            {
                var propertyType = property.PropertyType;

                if(propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    var entityType = propertyType.GetGenericArguments().Single();

                    if(entityType == typeof(TEntity))
                    {
                        return (HashSet<TEntity>)property.GetValue(this)!;
                    }
                }
            }

            throw new InvalidOperationException($"Cannot find data property for generic parameter {nameof(TEntity)}({typeof(TEntity).FullName})");
        }


    }
}