using Microsoft.AspNetCore.Mvc;
using OpenChat.Api.Models;
using OpenChat.Domain.Users;
using OpenChat.Domain.Users.Results;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class FollowingsController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public FollowingsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("followings")]
        public IActionResult Follow(FollowRequest followRequest)
        {
            var createFollowResult = _usersService.Follow(
                followerId: followRequest.FollowerId!.Value,
                followeeId: followRequest.FolloweeId!.Value
            );

            if(createFollowResult.IsFailed && createFollowResult.Reason == CreateFollowingError.UserNotFound)
            {
                return BadRequest("At least one of the users does not exit.");
            }
            
            return CreatedAtAction(nameof(Follow), null);
        }

        [HttpGet("followings/{userId:Guid}/followees")]
        public IActionResult GetFollowees(Guid userId)
        {
            var followees = _usersService.GetFolloweesForUser(userId);

            var response = followees
                .Select(followee => new UserResponse(
                    id: followee.Id!.Value,
                    username: followee.Username,
                    about: followee.About!
                ))
                .ToHashSet();

            return Ok(response);
        }
    }
}