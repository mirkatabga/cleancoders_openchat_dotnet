using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;

namespace OpenChat.Domain.UseCases
{
    public class GetWallUseCase
    {
        private readonly IUsersService _usersService;
        private readonly IPostsService _postsService;

        public GetWallUseCase(IUsersService usersService, IPostsService postsService)
        {
            _usersService = usersService;
            _postsService = postsService;
        }
        
        public GetWallResult Execute(Guid? userId)
        {
            var user = _usersService.GetById(userId!.Value);

            if(user is null)
            {
                return GetWallResult.Error(GetWallErrors.UserNotFound);
            }

            var posts = _postsService.GetWall(user.Id!.Value);

            return GetWallResult.Success(posts);
        }
    }
}