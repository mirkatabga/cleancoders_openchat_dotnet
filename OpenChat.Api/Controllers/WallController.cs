using Microsoft.AspNetCore.Mvc;
using OpenChat.Api.Models;
using OpenChat.Domain.UseCases;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class WallController : ControllerBase
    {
        private readonly GetWallUseCase _getWallUseCase;

        public WallController(GetWallUseCase getWallUseCase)
        {
            _getWallUseCase = getWallUseCase;
        }

        [HttpGet("users/{userId:Guid}/wall")]
        public IActionResult GetWall(Guid userId)
        {
            var result = _getWallUseCase.Execute(userId);

            if(result.IsFailed && result.Reason == GetWallErrors.UserNotFound)
            {
                return BadRequest("User does not exist.");
            }

            var postResponses = result.Value!
                .Select(post => new PostResponse
                {
                    PostId = post.Id,
                    UserId = post.UserId,
                    Text = post.Text,
                    CreatedDate = post.CreatedDate,            
                })
                .ToList();

            return Ok(postResponses);
        }
    }
}