using System.Reflection;
using OpenChat.Domain.Common;
using OpenChat.Infrastructure.Extensions;

namespace OpenChat.Infrastructure.Persistence.Repositories.Abstractions
{
    public abstract class InMemoryRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private static readonly Type _entityType;
        private static PropertyInfo? _idProperty;
        protected static PropertyInfo[] _publicProperties;

        private readonly HashSet<TEntity> _set;
        protected readonly InMemoryDataContext _context;


        static InMemoryRepository()
        {
            _entityType = typeof(TEntity);
            _publicProperties = _entityType.GetProperties();
            _idProperty = GetIdProperty();
        }

        public InMemoryRepository(InMemoryDataContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        public TEntity? GetById(object? id)
        {
            var entity = _set
                .SingleOrDefault(x =>
                {
                    var currentId = _idProperty!.GetValue(x);
                    return currentId!.Equals(id);
                });

            return (TEntity?)DeepCopy(entity!);
        }

        public void Insert(TEntity entity)
        {
            _set.Add(Copy(entity)!);
        }

        public void Update(TEntity entityToUpdate)
        {
            var entity = GetById(_idProperty!.GetValue(entityToUpdate)!);

            _set.Remove(entity!);
            _set.Add(Copy(entityToUpdate)!);
        }

        protected static void SetPrivateProperty(TEntity entity, string propertyName, object? value)
        {
            var propertyInfo = _publicProperties.Single(pi => pi.Name == propertyName);
            propertyInfo!.SetValue(entity, value);
        }

        protected static TEntity? Copy(TEntity entity) => (TEntity?)DeepCopy(entity);

        protected static object? DeepCopy(object objSource)
        {
            if (objSource is null)
            {
                return null;
            }

            Type typeSource = objSource.GetType();
            object? objTarget = CreateInstance(objSource, typeSource);

            PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in propertyInfo)
            {
                if (property.CanWrite)
                {
                    if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
                    {
                        property.SetValue(objTarget, property.GetValue(objSource, null), null);
                    }
                    else
                    {
                        object objPropertyValue = property.GetValue(objSource, null)!;

                        if (objPropertyValue == null)
                        {
                            property.SetValue(objTarget, null, null);
                        }
                        else
                        {
                            property.SetValue(objTarget, DeepCopy(objPropertyValue), null);
                        }
                    }
                }
            }

            return objTarget;
        }

        private static object? CreateInstance(object source, Type typeSource)
        {
            var constrInfos = typeSource.GetConstructors();

            var parametersByConstrInfo = constrInfos
                .Select(ci => new Tuple<ConstructorInfo, ParameterInfo[]>(ci, ci.GetParameters()))
                .ToHashSet();

            if (HasPublicParameterlessConstructor(parametersByConstrInfo))
            {
                return Activator.CreateInstance(typeSource);
            }

            var args = FindArguments(source, typeSource, parametersByConstrInfo);

            return Activator.CreateInstance(typeSource, args);
        }

        private static object?[]? FindArguments(object source, Type typeSource, HashSet<Tuple<ConstructorInfo, ParameterInfo[]>> parametersByConstrInfo)
        {
            var parameters = parametersByConstrInfo
                            .OrderByDescending(tuple => tuple.Item2.Count())
                            .Select(tuple => tuple.Item2)
                            .First();

            var memberInfos = typeSource.GetMembers();
            var args = new List<object?>();

            foreach (var parameterInfo in parameters)
            {
                var memberInfo = memberInfos.FirstOrDefault(mi => mi.Name.ToUpper() == parameterInfo.Name!.ToUpper());

                if (memberInfo is not null)
                {
                    args.Add(memberInfo.GetValue(source));
                }
            }

            return args.ToArray();
        }

        private static bool HasPublicParameterlessConstructor(HashSet<Tuple<ConstructorInfo, ParameterInfo[]>> parametersByConstrInfo)
        {
            return parametersByConstrInfo
                .Any(tuple =>
                {
                    (ConstructorInfo constructorInfo, ParameterInfo[] parameterInfos) = tuple;

                    return constructorInfo.IsPublic && parameterInfos.Count() == 0;
                });
        }

        private static PropertyInfo? GetIdProperty()
        {
            foreach (var property in _publicProperties)
            {
                if (property.Name == "Id" || property.Name == _entityType.Name + "Id")
                {
                    return property;
                }
            }

            return null;
        }
    }
}