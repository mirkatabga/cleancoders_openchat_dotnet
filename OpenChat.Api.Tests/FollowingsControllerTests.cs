using OpenChat.Api.Tests.Abstractions;
using static OpenChat.Tests.Common.UsersMother;

namespace OpenChat.Api.Tests
{
    public class FollowingsControllerTests : ControllerTests
    {
        [Fact]
        public void Create_ValidFollowing_ReturnsCreatedResult()
        {
            var followingsController = new FollowingsController(_usersService);
            var aliceId = RegisterUser(GetAliceRegisterRequest()).Id;
            var bobId = RegisterUser(GetBobRegisterRequest()).Id;
            var followRequest = GetFollowingRequest(aliceId, bobId);

            var actionResult = followingsController.Follow(followRequest);

            AssertResult<CreatedAtActionResult>(actionResult);
        }

        [Fact]
        public void Create_FollowingForMissingUsers_ReturnsBadRequestWithError()
        {
            var followingsController = new FollowingsController(_usersService);
            var aliceId = RegisterUser(GetAliceRegisterRequest()).Id;
            var bobId = RegisterUser(GetBobRegisterRequest()).Id;

            var invalidFollowRequests = new List<FollowRequest>()
            {
                GetFollowingRequest(aliceId, NON_EXISTING_USER_ID),
                GetFollowingRequest(NON_EXISTING_USER_ID, bobId),
                GetFollowingRequest(NON_EXISTING_USER_ID, NON_EXISTING_USER_ID),
            };
            
            foreach(var invalidRequest in invalidFollowRequests)
            {
                var actionResult = followingsController.Follow(invalidRequest);         
                var error = AssertObjectResult<BadRequestObjectResult, string>(actionResult);

                error
                    .Should()
                    .NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void GetFollowees_ForUser_ReturnsCreatedResultWithFolloweesUsers()
        {
            var followingsController = new FollowingsController(_usersService);
            var alice = RegisterUser(GetAliceRegisterRequest());
            var bob = RegisterUser(GetBobRegisterRequest());
            var john = RegisterUser(GetJohnRegisterRequest());
            var aliceFollowsBob = GetFollowingRequest(alice.Id, bob.Id);
            var aliceFollowsJohn = GetFollowingRequest(alice.Id, john.Id);
            var bobFollowsAlice = GetFollowingRequest(bob.Id, alice.Id);
            var johnFollowsBob = GetFollowingRequest(john.Id, bob.Id);

            followingsController.Follow(aliceFollowsBob);
            followingsController.Follow(aliceFollowsJohn);
            followingsController.Follow(bobFollowsAlice);
            followingsController.Follow(johnFollowsBob);

            var actionResult = followingsController.GetFollowees(alice.Id);

            var followees = AssertObjectResult<OkObjectResult, ICollection<UserResponse>>(
                actionResult, assertObjectType: false);

            followees
                .Should()
                .ContainEquivalentOf(bob)
                .And
                .ContainEquivalentOf(john);
        }

        private FollowRequest GetFollowingRequest(Guid followerId, Guid followeeId)
        {
            var followRequest = new FollowRequest
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };

            return followRequest;
        }
    }
}