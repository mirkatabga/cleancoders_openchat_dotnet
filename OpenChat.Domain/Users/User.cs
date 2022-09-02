using OpenChat.Domain.Posts;

namespace OpenChat.Domain.Users
{
    public class User
    {
        public User(string username, string password)
        {
            Validate(username, password);

            Username = username;
            Password = password;
        }

        public User(string username, string password, string? about)
            : this(username, password)
        {
            About = about;
        }

        public Guid? Id { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string? About { get; private set; }

        public ICollection<Post> Posts { get; private set; } = new HashSet<Post>();

        public ICollection<Following> Followers { get; private set; } = new HashSet<Following>();

        public ICollection<Following> Followees { get; private set; } = new  HashSet<Following>();

        private static void Validate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException($"Null, empty or whitespace argument: {nameof(username)}");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException($"Null, empty or whitespace argument: {nameof(password)}");
            }
        }
    }
}