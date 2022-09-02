using OpenChat.Domain.Posts;
using OpenChat.Infrastructure.Persistence.Repositories.Abstractions;

namespace OpenChat.Infrastructure.Persistence.Repositories
{
    public class InMemoryPostsRepository : InMemoryRepository<Post>, IPostsRepository
    {
        public InMemoryPostsRepository(InMemoryDataContext dataContext)
            : base(dataContext)
        {
        }

        public void Add(Post newPost)
        {
            var id = Guid.NewGuid();
            SetPrivateProperty(newPost, nameof(newPost.Id), id);

            Insert(newPost);
        }

        public ICollection<Post> GetForUser(Guid userId)
        {
            return _context.Posts
                .Where(post => post.UserId == userId)
                .Select(post => Copy(post)!)
                .ToHashSet<Post>();
        }

        private IQueryable<Post> IncludeUser(IQueryable<Post> query)
        {
            var allPosts = query.ToList();

            foreach (var post in allPosts)
            {
                if(post.User is not null)
                {
                    continue;
                }

                var user =  _context.Users.SingleOrDefault(u => u.Id == post.UserId);
                SetPrivateProperty(post, nameof(post.User), user);
            }

            return allPosts.AsQueryable();
        }

        public ICollection<Post> GetOrderedDescForUsers(IEnumerable<Guid> userIds)
        {
            return _context.Posts
                .Where(p => userIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }
    }
}