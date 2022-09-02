using Microsoft.EntityFrameworkCore;
using OpenChat.Domain.Posts;

namespace OpenChat.Infrastructure.Persistence.Repositories
{
    public class EfPostsRepository : EfGenericRepository<Post>, IPostsRepository
    {
        private readonly OpenChatDbContext _dbContext;
        private readonly DbSet<Post> _set;

        public EfPostsRepository(OpenChatDbContext dbContext)
            : base(dbContext)
        {

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _set = _dbContext.Set<Post>();
        }

        public void Add(Post newPost)
        {
           if(newPost is null)
           {
                throw new ArgumentNullException(nameof(newPost));
           }

           _set.Add(newPost);
        }

        public ICollection<Post> GetForUser(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException($"Empty argument: {nameof(userId)}");
            }

            return _set
                .Where(p => p.UserId == userId)
                .ToHashSet<Post>();
        }

        public ICollection<Post> GetOrderedDescForUsers(IEnumerable<Guid> userIds)
        {
            return _set
                .Where(p => userIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }
    }
}