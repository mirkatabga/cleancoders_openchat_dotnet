using OpenChat.Api.Tests.Abstractions;
using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;
using OpenChat.Infrastructure.Persistence;

namespace OpenChat.Api.Tests
{
    public class PostsControllerTests : ControllerTests
    {
        private const string POST_TEXT = "Some post text.";

        [Fact]
        public void Create_ValidPost_ReturnsCreatedResultWithCorrectPost()
        {
            var aliceId = RegisterUser(GetAliceRegisterRequest()).Id;
            var postsController = new PostsController(_postsService);

            var postRequest = new PostRequest
            {
                Text = POST_TEXT
            };

            var actionResult = postsController.Create(aliceId, postRequest);
            var postResponse = AssertObjectResult<CreatedAtActionResult, PostResponse>(actionResult);

            postResponse
                .Should()
                .BeEquivalentTo(postRequest);

            AssertGuid(postResponse.UserId);
            AssertGuid(postResponse.PostId);

            postResponse.CreatedDate
                .Should()
                .BeAfter(DateTime.MinValue);
        }

        [Fact]
        public void Create_PostForNonExistingUser_ReturnsBadRequestWithError()
        {
            var postsController = new PostsController(_postsService);

            var postRequest = new PostRequest
            {
                Text = POST_TEXT
            };

            var actionResult = postsController.Create(Guid.NewGuid(), postRequest);
            var error = AssertObjectResult<BadRequestObjectResult, string>(actionResult);

            error
                .Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetPosts_ForUser_ReturnsPostsByUser()
        {
            var postsController = new PostsController(_postsService);
            var aliceId = RegisterUser(GetAliceRegisterRequest()).Id;
            var bobId = RegisterUser(GetBobRegisterRequest()).Id;
            CreatePosts(bobId);
            var alicePosts = CreatePosts(aliceId);

            var actionResult = postsController.GetByUserId(aliceId);
            var fetchedPosts = AssertObjectResult<OkObjectResult, ICollection<PostResponse>>(actionResult, assertObjectType: false);

            fetchedPosts
                .Should()
                .BeEquivalentTo(alicePosts, options =>
                {
                    options.WithMapping<Post, PostResponse>(post => post.Id, postResponse => postResponse.PostId);
                    options.Excluding(post => post.User);

                    return options;
                });
        }

        protected ICollection<Post> CreatePosts(Guid userId)
        {
            var posts = new HashSet<Post>();
            var first = _postsService.Create(new PostDto(userId, POST_TEXT + " #1"));
            var second = _postsService.Create(new PostDto(userId, POST_TEXT + " #2"));

            posts.Add(first.Value!);
            posts.Add(second.Value!);

            return posts;
        }
    }
}