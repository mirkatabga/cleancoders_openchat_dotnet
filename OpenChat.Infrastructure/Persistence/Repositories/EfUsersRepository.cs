using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpenChat.Domain.Users;

namespace OpenChat.Infrastructure.Persistence.Repositories
{
    public class EfUsersRepository : EfGenericRepository<User>, IUsersRepository
    {
        private readonly OpenChatDbContext _dbContext;
        private readonly DbSet<User> _set;

        public EfUsersRepository(OpenChatDbContext dbContext)
            : base(dbContext)
        {

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _set = _dbContext.Set<User>();
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

            _set.Add(user);

            return user;
        }

        public bool DoesUsernameExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"Missing argument: {nameof(username)}");
            }

            return _set.Any(u => u.Username.ToLower() == username.ToLower());
        }

        public void Follow(Guid followerId, Guid followeeId)
        {
            var following = new Following(
                followerId: followerId,
                followeeId: followeeId
            );

            _dbContext.Followings!.Add(following);
            _dbContext.SaveChanges();
        }

        public ICollection<User> GetAll()
        {
            return _set
                .AsNoTracking()
                .ToHashSet();
        }

        public ICollection<User> GetFolloweesForUser(Guid userId)
        {
            var result = _set
                .AsNoTracking()
                .Include($"{nameof(User.Followees)}.{nameof(Following.Followee)}")
                .Where(user => user.Id == userId)
                .SelectMany(user => user.Followees.Select(followee => followee.Followee!))
                .ToHashSet();

            return result;
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

            return _set.SingleOrDefault(filter);
        }
    }
}