using FluentAssertions;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Posts.Results;
using OpenChat.Domain.Tests.Common;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Domain.Tests.Posts
{
    public class PostsServiceTests : DomainTests
    {
        private const string POST_TEXT = "Some text.";
        private readonly InMemoryUnitOfWork _uow;
        private readonly IUsersService _usersService;
        private readonly IPostsService _postsService;

        public PostsServiceTests()
        {
            _uow = new InMemoryUnitOfWork(new InMemoryDataContext());

            _usersService = new UsersService(_uow);
            _postsService = new PostsService(_uow);
        }

        [Fact]
        public void Create_NullPostDto_Throws()
        {
            _postsService
                .Invoking(service => service.Create(null!))
                .Should()
                .ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_InvalidPost_Throws(string text)
        {
            var invalidPost = new PostDto(USER_ID, text);

            _postsService
                .Invoking(service => service.Create(invalidPost))
                .Should()
                .ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_InvalidPostUserIdEmpty_Throws()
        {
            var invalidPost = new PostDto(USER_ID_EMPTY, POST_TEXT);

            _postsService
                .Invoking(service => service.Create(invalidPost))
                .Should()
                .ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_ValidPostDto_CreatesPost()
        {
            var userResult = _usersService.Create(ALICE_DTO);

            var postDto = new PostDto(
                userId: userResult.Value!.Id!.Value,
                text: POST_TEXT);

            _uow.ResetSaveChangesInvocations();

            var result = _postsService.Create(postDto);

            result.IsSuccess
                .Should()
                .BeTrue();

            result.Value
                .Should()
                .BeEquivalentTo(postDto);

            result.Value!.Id
                .Should()
                .NotBeEmpty();

            result.Value!.CreatedDate
                .Should()
                .BeAfter(DateTime.MinValue);

            AssertUnitOfWork(_uow);
        }

        [Fact]
        public void Create_PostForNonExistingUser_ReturnsError()
        {
            var postDto = new PostDto(
                userId: USER_ID,
                text: POST_TEXT
            );

            var result = _postsService.Create(postDto);

            result.IsFailed
                .Should()
                .BeTrue();

            result.Reason
                .Should()
                .Be(CreatePostError.UserNotFound);
        }

        [Fact]
        public void GetPostsForUser_ByUserId_ReturnsPosts()
        {
            var createUserResult = _usersService.Create(ALICE_DTO);
            var aliceId = createUserResult.Value!.Id!.Value;
            var alicePosts = SetUpDefaultPosts(userId: aliceId, count: 2);

            var posts = _postsService.GetPostsForUser(userId: aliceId);

            posts
                .Should()
                .BeEquivalentTo(alicePosts, opt => opt.Excluding(post => post.Id));
        }

        private IEnumerable<Post> SetUpDefaultPosts(Guid userId, int count)
        {
            var posts = new List<Post>();

            for (int i = 0; i < count; i++)
            {
                var postDto = new PostDto(
                    userId: userId,
                    text: POST_TEXT + $" #{i + 1}"
                );

                var post = _postsService.Create(postDto).Value!;
                posts.Add(post);
            }

            return posts;
        }
    }
}