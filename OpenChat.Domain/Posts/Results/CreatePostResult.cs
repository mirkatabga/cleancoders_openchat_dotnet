using OpenChat.Domain.Common;

namespace OpenChat.Domain.Posts.Results
{
    public class CreatePostResult : Result<Post, CreatePostError>
    {
        public CreatePostResult(Post value) : base(value)
        {
        }

        public CreatePostResult(CreatePostError errorReason) : base(errorReason)
        {
        }

        internal static CreatePostResult Success(Post post) => new CreatePostResult(post);


        internal static CreatePostResult Error(CreatePostError userNotFound) => new CreatePostResult(userNotFound);
    }
}