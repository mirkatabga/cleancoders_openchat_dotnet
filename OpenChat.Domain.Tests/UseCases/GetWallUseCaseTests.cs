using FluentAssertions;
using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;
using OpenChat.Domain.UseCases;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Domain.Tests
{
    public class GetWallUseCaseTests
    {
        private readonly IUsersService _usersService;
        private readonly IPostsService _postsService;

        public GetWallUseCaseTests()
        {
            IUnitOfWork unitOfWork = new InMemoryUnitOfWork(new InMemoryDataContext());

            _usersService = new UsersService(unitOfWork);
            _postsService = new PostsService(unitOfWork);
        }

        [Fact]
        public void Execute_NonExistingUser_ReturnsErrorResult()
        {
            var getWallUseCase = new GetWallUseCase(_usersService, _postsService);

            var result = getWallUseCase.Execute(userId: NON_EXISTING_USER_ID);

            result.IsFailed
                .Should()
                .BeTrue();

            result.Reason
                .Should()
                .BeDefined();
        }

        [Fact]
        public void Execute_UserThatFollowsOthers_ReturnsCorrectPosts()
        {
            var getWallUseCase = new GetWallUseCase(_usersService, _postsService);
            var alice = _usersService.Create(ALICE_DTO).Value!;
            var bob = _usersService.Create(BOB_DTO).Value!;
            var john = _usersService.Create(JOHN_DTO).Value!;
            var expectedPosts = new List<Post>();

            var alicePosts = CreatePosts(
                userId: alice.Id,
                text: "Alice post.",
                count: 2);

            var bobPosts = CreatePosts(
                userId: bob.Id,
                text: "Bob post.",
                count: 2
            );

            var johnPosts = CreatePosts(
                userId: john.Id,
                text: "John post.",
                count: 3
            );

            _usersService.Follow(
                followerId: alice.Id!.Value,
                followeeId: john.Id!.Value
            );

            expectedPosts.AddRange(alicePosts);
            expectedPosts.AddRange(johnPosts);
            var orderedExpected = expectedPosts.OrderByDescending(p => p.CreatedDate);

            var aliceWallResult = getWallUseCase.Execute(alice.Id);

            aliceWallResult.IsSuccess
                .Should()
                .BeTrue();

            aliceWallResult.Value
                .Should()
                .BeEquivalentTo(orderedExpected, opt => opt.WithStrictOrdering());
        }

        private IEnumerable<Post> CreatePosts(Guid? userId, string text, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var postDto = new PostDto(
                    userId: userId!.Value,
                    text: $"{text} #{i + 1}"
                );

                yield return _postsService.Create(postDto).Value!;
            }
        }
    }
}