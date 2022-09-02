namespace OpenChat.Api.Models
{
    public class UserResponse
    {
        public UserResponse(Guid id, string username)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException($"Empty argument: {nameof(id)}");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"Null or empty argument: {nameof(username)}");
            }

            Id = id;
            Username = username;
        }

        public UserResponse(Guid id, string username, string about)
            : this(id, username)
        {
            About = about;
        }

        public Guid Id { get; }

        public string Username { get; }

        public string? About { get; }
    }
}