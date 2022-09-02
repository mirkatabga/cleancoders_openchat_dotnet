using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;

namespace OpenChat.Domain.Common
{
    public interface IUnitOfWork
    {
        IUsersRepository Users { get; }

        IPostsRepository Posts{ get; }

         void SaveChanges();
    }
}