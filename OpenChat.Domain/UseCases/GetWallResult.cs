using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;

namespace OpenChat.Domain.UseCases
{
    public class GetWallResult : Result<ICollection<Post>, GetWallErrors>
    {
        private GetWallResult(ICollection<Post> value) 
            : base(value)
        {
        }

        private GetWallResult(GetWallErrors errorReason) 
            : base(errorReason)
        {
        }

        internal static GetWallResult Error(GetWallErrors error)
        {
            return new GetWallResult(error);
        }

        internal static GetWallResult Success(ICollection<Post> posts)
        {
            return new GetWallResult(posts);
        }
    }
}