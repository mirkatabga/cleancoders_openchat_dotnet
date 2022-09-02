using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence.Repositories;

namespace OpenChat.Infrastructure.Persistence
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly InMemoryDataContext _context;
        private IUsersRepository? _usersRepository;
        private IPostsRepository? _postsRepository;

        public InMemoryUnitOfWork(InMemoryDataContext context)
        {
            _context = context;
        }

        public int SaveChangesInvocations { get; private set; } = 0;

        public IUsersRepository Users
        {
            get
            {
                if (_usersRepository is null)
                {
                    _usersRepository = new InMemoryUsersRepository(_context);
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
                    _postsRepository = new InMemoryPostsRepository(_context);
                }

                return _postsRepository;
            }
        }

        public void SaveChanges()
        {
            SaveChangesInvocations++;
            _context.SaveChanges();
        }

        public void ResetSaveChangesInvocations()
        {
            SaveChangesInvocations = 0;
        }
    }
}