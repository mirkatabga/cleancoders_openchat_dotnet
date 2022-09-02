using OpenChat.Domain.Common;
using OpenChat.Domain.Posts.Results;

namespace OpenChat.Domain.Posts
{
    public class PostsService : IPostsService
    {
        private readonly IUnitOfWork _uow;

        public PostsService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public CreatePostResult Create(PostDto post)
        {
            ValidateCreate(post);

            var user = _uow.Users.GetById(post.UserId);

            if (user == null)
            {
                return CreatePostResult.Error(CreatePostError.UserNotFound);
            }

            var newPost = new Post(
                userId: post.UserId,
                text: post.Text);

            _uow.Posts.Add(newPost);
            _uow.SaveChanges();

            return CreatePostResult.Success(newPost);
        }

        public ICollection<Post> GetPostsForUser(Guid userId)
        {
            var posts = _uow.Posts.GetForUser(userId);

            return posts;
        }

        public ICollection<Post> GetWall(Guid userId)
        {
            var userIds = new List<Guid> { userId };

            var followeesIds = _uow.Users
                .GetFolloweesForUser(userId)
                .Select(u => u.Id!.Value)
                .ToList();

            userIds.AddRange(followeesIds);

            return _uow.Posts.GetOrderedDescForUsers(userIds);
        }

        private void ValidateCreate(PostDto post)
        {
            if (post is null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            if (post.UserId == Guid.Empty)
            {
                throw new ArgumentException($"Empty argument: {nameof(post)}.{nameof(post.UserId)}");
            }

            if (string.IsNullOrWhiteSpace(post.Text))
            {
                throw new ArgumentException($"Null, empty or whitespace argument: {nameof(post)}.{nameof(post.Text)}");
            }
        }
    }
}