using OpenChat.Domain.Common;

namespace OpenChat.Domain.Users.Results
{
    public class CreateUserResult : Result<User, CreateUserError>
    {
        private CreateUserResult(User value) : base(value)
        {   
        }

        private CreateUserResult(CreateUserError errorReason) : base(errorReason)
        {
        }

        internal static CreateUserResult Success(User user)
        {
            return new CreateUserResult(user);
        }

        internal static CreateUserResult Error(CreateUserError reason)
        {
            return new CreateUserResult(reason);
        }
    }
}