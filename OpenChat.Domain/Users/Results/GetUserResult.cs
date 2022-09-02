using OpenChat.Domain.Common;

namespace OpenChat.Domain.Users.Results
{
    public class GetUserResult : Result<User, GetUserError>
    {
        private GetUserResult(User value) : base(value)
        {   
        }

        private GetUserResult(GetUserError errorReason) : base(errorReason)
        {
        }

        public static GetUserResult Success(User user)
        {
            return new GetUserResult(user);
        }

        public static GetUserResult Error(GetUserError reason)
        {
            return new GetUserResult(reason);
        }
    }
}