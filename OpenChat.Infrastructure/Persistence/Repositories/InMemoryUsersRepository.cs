using System.Linq.Expressions;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence.Repositories.Abstractions;

namespace OpenChat.Infrastructure.Persistence.Repositories
{
    public class InMemoryUsersRepository : InMemoryRepository<User>, IUsersRepository
    {
        public InMemoryUsersRepository(InMemoryDataContext context)
            : base(context)
        {
        }

        public User? GetById(Guid id)
        {
            return base.GetById(id);
        }

        public User Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = Guid.NewGuid();
            SetPrivateProperty(user, nameof(user.Id), id);

            Insert(user);

            return user;
        }

        public bool DoesUsernameExists(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"Null or empty argument: {nameof(username)}");
            }

            return _context.Users
                .Any(u => u.Username.ToLower() == username.ToLower());
        }

        public User? GetByUsernameAndPassword(string username, string password, bool caseSensitiveUsername = false)
        {
            Expression<Func<User, bool>> filter = u =>
                    u.Username.ToLower() == username.ToLower() &&
                    u.Password == password;

            if (caseSensitiveUsername)
            {
                filter = u =>
                    u.Username == username &&
                    u.Password == password;
            }

            return _context.Users.SingleOrDefault(filter.Compile());
        }

        public ICollection<User> GetAll()
        {
            return _context.Users
                .Select(user => Copy(user)!)
                .ToHashSet();
        }

        public void Follow(Guid followerId, Guid followeeId)
        {
            var following = new Following(
                followerId: followerId,
                followeeId: followeeId
            );

            _context.Followings.Add((Following)DeepCopy(following)!);
        }

        public ICollection<User> GetFolloweesForUser(Guid userId)
        {
            var followings = _context.Followings
                .Where(following => following.FollowerId == userId)
                .ToHashSet();

            return _context.Users
                .Where(user => followings.Any(
                    following => following.FolloweeId == user.Id))
                .Select(user => Copy(user)!)
                .ToHashSet();
        }
    }
}