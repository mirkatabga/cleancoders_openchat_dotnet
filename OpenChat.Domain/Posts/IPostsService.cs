using OpenChat.Domain.Posts.Results;

namespace OpenChat.Domain.Posts
{
    public interface IPostsService
    {
        CreatePostResult Create(PostDto post);

        ICollection<Post> GetPostsForUser(Guid userId);

        ICollection<Post> GetWall(Guid userId);
    }
}