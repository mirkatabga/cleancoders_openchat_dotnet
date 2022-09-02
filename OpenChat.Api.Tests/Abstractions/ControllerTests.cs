using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Api.Tests.Abstractions
{
    public abstract class ControllerTests
    {
        private readonly AccountsController _accountsController;
        private readonly FollowingsController _followingsController;
        private readonly PostsController _postsController;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUsersService _usersService;
        protected readonly IPostsService _postsService;

        public ControllerTests()
        {
            _unitOfWork = new InMemoryUnitOfWork(new InMemoryDataContext());
            _usersService = new UsersService(_unitOfWork);
            _postsService = new PostsService(_unitOfWork);
            _accountsController = new AccountsController(_usersService);
            _followingsController = new FollowingsController(_usersService);
            _postsController = new PostsController(_postsService);
        }

        protected internal static TObject AssertObjectResult<TResult, TObject>(IActionResult actionResult, bool assertObjectType = true)
           where TResult : ObjectResult
           where TObject : class
        {
            var result = actionResult
                .Should()
                .BeOfType<TResult>()
                .Which;

            if (assertObjectType)
            {
                return result.Value
                    .Should()
                    .BeOfType<TObject>()
                    .Which;
            }

            return (TObject)result.Value!;
        }

        protected internal static void AssertResult<TResult>(IActionResult actionResult)
           where TResult : IActionResult
        {
            var result = actionResult
                .Should()
                .BeOfType<TResult>()
                .Which;
        }

        protected internal static RegisterRequest GetAliceRegisterRequest()
        {
            return new RegisterRequest
            {
                Username = ALICE_USERNAME,
                Password = ALICE_PASSWORD,
                About = ALICE_ABOUT
            };
        }

        protected internal RegisterRequest GetBobRegisterRequest()
        {
            return new RegisterRequest
            {
                Username = BOB_USERNAME,
                Password = BOB_PASSWORD,
                About = BOB_ABOUT
            };
        }

        protected internal RegisterRequest GetJohnRegisterRequest()
        {
            return new RegisterRequest
            {
                Username = JOHN_USERNAME,
                Password = JOHN_PASSWORD,
                About = JOHN_ABOUT
            };
        }

        protected internal UserResponse RegisterUser(RegisterRequest registerRequest)
        {
            var registerActionResult = _accountsController.Register(registerRequest);

            return registerActionResult
                .As<OkObjectResult>().Value
                .As<UserResponse>();
        }

        protected internal static void AssertGuid(Guid? guid)
        {
            guid
                .Should()
                .NotBeNull().And
                .NotBeEmpty();

            guid.ToString()
                .Should()
                .MatchRegex(GUID_PATTERN);
        }

        protected internal void Follow(Guid followerId, Guid followeeId)
        {
            _followingsController.Follow(
                new FollowRequest
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                });
        }

        protected internal PostResponse CreatePost(Guid userId, PostRequest postRequest)
        {
            return _postsController.Create(userId, postRequest)
                .As<CreatedAtActionResult>()
                .Value
                .As<PostResponse>();
        }
    }
}