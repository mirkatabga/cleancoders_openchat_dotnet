namespace OpenChat.Domain.Users
{
    public interface IUsersRepository
    {
        User? GetById(Guid id);

        User? GetByUsernameAndPassword(string username, string password, bool caseSensitiveUsername = false);

        bool DoesUsernameExists(string username);

        ICollection<User> GetAll();

        User Add(User user);

        void Follow(Guid followerId, Guid followeeId);

        ICollection<User> GetFolloweesForUser(Guid userId);
    }
}