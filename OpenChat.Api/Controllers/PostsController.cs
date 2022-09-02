using Microsoft.AspNetCore.Mvc;
using OpenChat.Api.Models;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Posts.Results;

namespace OpenChat.Api.Controllers
{
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        [HttpPost("users/{userId:Guid}/timeline")]
        public IActionResult Create(Guid userId, PostRequest postRequest)
        {
            var postDto = new PostDto
            (
                userId: userId,
                text: postRequest.Text!
            );

            var createResult = _postsService.Create(postDto);

            if(createResult.IsFailed && createResult.Reason == CreatePostError.UserNotFound)
            {
                return BadRequest("User does not exit.");
            }

            var post = createResult.Value;
            var postResponse = new PostResponse
            {
                PostId = post!.Id,
                UserId = post.UserId,
                Text = post.Text,
                CreatedDate = post.CreatedDate
            };

            return CreatedAtAction(nameof(Create), postResponse);
        }

        [HttpGet("users/{userId:Guid}/timeline")]
        public IActionResult GetByUserId(Guid userId)
        {
            var fetchedPosts = _postsService.GetPostsForUser(userId);

            var response = fetchedPosts
                .Select(post => new PostResponse
                {
                    PostId = post.Id,
                    UserId = post.UserId,
                    Text = post.Text,
                    CreatedDate = post.CreatedDate
                })
                .ToHashSet();

            return Ok(response);
        }
    }
}