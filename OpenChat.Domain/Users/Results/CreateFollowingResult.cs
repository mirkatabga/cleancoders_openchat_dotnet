using OpenChat.Domain.Common;

namespace OpenChat.Domain.Users.Results
{
    public class CreateFollowingResult : Result<CreateFollowingError>
    {
        private CreateFollowingResult() 
            : base()
        {
            
        }

        private CreateFollowingResult(CreateFollowingError errorReason) 
            : base(errorReason)
        {
        }

        internal static CreateFollowingResult Success()
        {
            return new CreateFollowingResult();
        }

        internal static CreateFollowingResult Error(CreateFollowingError reason)
        {
            return new CreateFollowingResult(reason);
        }
    }
}