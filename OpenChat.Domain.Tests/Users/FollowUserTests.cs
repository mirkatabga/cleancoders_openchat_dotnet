using FluentAssertions;
using OpenChat.Domain.Tests.Common;
using OpenChat.Domain.Users;
using OpenChat.Domain.Users.Results;
using OpenChat.Infrastructure.Persistence;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Domain.Tests.Users
{
    public class FollowUserTests : DomainTests
    {
        private readonly InMemoryUnitOfWork _uow;
        private readonly IUsersService _usersService;

        public FollowUserTests()
        {
            _uow = new InMemoryUnitOfWork(new InMemoryDataContext());
            _usersService = new UsersService(_uow);
        }

        [Fact]
        public void Follow_NonExistingUser_ReturnsErrorResult()
        {
            var alice = _usersService.Create(ALICE_DTO).Value;

            var invalidResults = new List<CreateFollowingResult>
            {
                 _usersService.Follow(
                    followerId: alice!.Id!.Value,
                    followeeId: NON_EXISTING_USER_ID
                ),

                _usersService.Follow(
                    followerId: NON_EXISTING_USER_ID,
                    followeeId: alice!.Id!.Value
                ),

                _usersService.Follow(
                    followerId: NON_EXISTING_USER_ID,
                    followeeId: NON_EXISTING_USER_ID
                )
            };

            foreach (var invalidResult in invalidResults)
            {
                invalidResult.IsFailed
                            .Should()
                            .BeTrue();

                invalidResult.Reason
                    .Should()
                    .BeDefined();
            }
        }

        [Fact]
        public void Follow_UserFollowsAnother_ShouldCreateFollowing()
        {
            var alice = _usersService.Create(ALICE_DTO).Value;
            var bob = _usersService.Create(BOB_DTO).Value;
            
            _uow.ResetSaveChangesInvocations();

            var result = _usersService.Follow(
                followerId: alice!.Id!.Value,
                followeeId: bob!.Id!.Value
            );

            result.IsSuccess
                .Should()
                .BeTrue();

            var bobResult = _usersService
                .GetFolloweesForUser(alice.Id)
                .Single();

            bobResult
                .Should()
                .BeEquivalentTo(bob);

            AssertUnitOfWork(_uow);
        }

        [Fact]
        public void GetFolloweesForUser_UserFollowsMultipleUsers_ReturnFollowees()
        {
            var alice = _usersService.Create(ALICE_DTO).Value;
            var bob = _usersService.Create(BOB_DTO).Value;
            var john = _usersService.Create(JOHN_DTO).Value;

            _usersService.Follow(
                followerId: alice!.Id!.Value,
                followeeId: bob!.Id!.Value);

            _usersService.Follow(
                followerId: alice!.Id!.Value,
                followeeId: john!.Id!.Value);

            _usersService.Follow(
                followerId: bob.Id!.Value,
                followeeId: john.Id!.Value);

            _usersService.Follow(
                followerId: john.Id!.Value,
                followeeId: alice.Id!.Value);

            var users = _usersService.GetFolloweesForUser(alice.Id);

            users
                .Should()
                .ContainEquivalentOf(bob)
                .And
                .ContainEquivalentOf(john);
        }
    }
}