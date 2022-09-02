using OpenChat.Domain.Users.Results;

namespace OpenChat.Domain.Users
{
    public interface IUsersService
    {
        CreateUserResult Create(UserDto userDto);

        GetUserResult GetByCredentials(string username, string password);

        ICollection<User> GetAll();

        CreateFollowingResult Follow(Guid followerId, Guid followeeId);

        ICollection<User> GetFolloweesForUser(Guid? id);

        User? GetById(Guid userId);
    }
}