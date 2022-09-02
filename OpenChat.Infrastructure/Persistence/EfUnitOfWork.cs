using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;
using OpenChat.Domain.Common;
using OpenChat.Infrastructure.Persistence.Repositories;

namespace OpenChat.Infrastructure.Persistence
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly OpenChatDbContext _context;
        private IUsersRepository? _usersRepository;
        private IPostsRepository? _postsRepository;

        public EfUnitOfWork(OpenChatDbContext context)
        {
            _context = context;
        }

        public IUsersRepository Users
        {
            get
            {
                if (_usersRepository is null)
                {
                    _usersRepository = new EfUsersRepository(_context);
                }

                return _usersRepository;
            }
        }

        public IPostsRepository Posts
        {
            get
            {
                if (_postsRepository is null)
                {
                    _postsRepository = new EfPostsRepository(_context);
                }

                return _postsRepository;
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}