using OpenChat.Api.Tests.Abstractions;
using OpenChat.Domain.Posts;
using OpenChat.Domain.UseCases;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Api.Tests
{
    public class WallControllerTests : ControllerTests
    {
        private const string POST_TEXT = "Some post text.";

        private readonly WallController _wallController;

        public WallControllerTests()
        {
            IPostsService postsService = new PostsService(_unitOfWork);
            var getWallUseCase = new GetWallUseCase(_usersService, postsService);
            _wallController = new WallController(getWallUseCase);

        }

        [Fact]
        public void GetWall_UserThatFollowsOthers_ReturnsCorrectPosts()
        {
            var aliceId = RegisterUser(GetAliceRegisterRequest()).Id;
            var bobId = RegisterUser(GetBobRegisterRequest()).Id;
            var johnId = RegisterUser(GetJohnRegisterRequest()).Id;
            var expectedPosts = new List<PostResponse>();

            Follow(followerId: aliceId, followeeId: bobId);

            var alicePostRequest = new PostRequest { Text = $"{POST_TEXT} Alice #1" };
            var bobPostRequestOne = new PostRequest { Text = $"{POST_TEXT} Bob #1" };
            var bobPostRequestTwo = new PostRequest { Text = $"{POST_TEXT} Bob #2" };
            var johnPostRequestOne = new PostRequest { Text = $"{POST_TEXT} John #1" };

            expectedPosts.Add(CreatePost(aliceId, alicePostRequest));
            expectedPosts.Add(CreatePost(bobId, bobPostRequestOne));
            expectedPosts.Add(CreatePost(bobId, bobPostRequestTwo));
            CreatePost(johnId, johnPostRequestOne);

            var actionResult = _wallController.GetWall(aliceId);

            var wallPosts = AssertObjectResult<OkObjectResult, ICollection<PostResponse>>(
                actionResult, assertObjectType: false);

            expectedPosts
                .OrderByDescending(p => p.CreatedDate)
                .Should()
                .BeEquivalentTo(wallPosts, opt => opt.WithStrictOrdering());
        }

        [Fact]
        public void GetWall_NonExistingUser_ReturnsBadRequestWithError()
        {
             var actionResult = _wallController.GetWall(NON_EXISTING_USER_ID);

             var error = AssertObjectResult<BadRequestObjectResult, string>(actionResult);

             error
                .Should()
                .NotBeNullOrWhiteSpace();
        }
    }
}