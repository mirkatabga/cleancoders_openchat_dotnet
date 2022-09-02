using OpenChat.Domain.Common;
using OpenChat.Domain.Users.Results;

namespace OpenChat.Domain.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;

        public UsersService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public CreateUserResult Create(UserDto userDto)
        {
            if (_uow.Users.DoesUsernameExists(userDto.Username))
            {
                return CreateUserResult.Error(CreateUserError.UserAlreadyExists);
            }

            var user = new User(
                username: userDto.Username,
                password: userDto.Password,
                about: userDto.About
            );

            _uow.Users.Add(user);
            _uow.SaveChanges();

            return CreateUserResult.Success(user);
        }

        public CreateFollowingResult Follow(Guid followerId, Guid followeeId)
        {
            var follower = _uow.Users.GetById(followerId);
            var followee = _uow.Users.GetById(followeeId);

            if(follower is null || followee is null)
            {
                return CreateFollowingResult.Error(CreateFollowingError.UserNotFound);
            }

            _uow.Users.Follow(
                followerId: followerId,
                followeeId: followeeId
            );

            _uow.SaveChanges();

            return CreateFollowingResult.Success();
        }

        public ICollection<User> GetAll()
        {
            return _uow.Users.GetAll();
        }

        public GetUserResult GetByCredentials(string username, string password)
        {
            var user = _uow.Users
                .GetByUsernameAndPassword(username, password);

            if (user == null)
            {
                return GetUserResult.Error(GetUserError.NotFound);
            }

            return GetUserResult.Success(user);
        }

        public User? GetById(Guid userId)
        {
            return _uow.Users.GetById(userId);
        }

        public ICollection<User> GetFolloweesForUser(Guid? id)
        {
            return _uow.Users.GetFolloweesForUser(id!.Value);
        }
    }
}