using OpenChat.Domain.Common;

namespace OpenChat.Domain.Posts
{
    public interface IPostsRepository : IGenericRepository<Post>
    {
        
        ICollection<Post> GetForUser(Guid userId);

        void Add(Post newPost);

        ICollection<Post> GetOrderedDescForUsers(IEnumerable<Guid> userIds);
    }
}