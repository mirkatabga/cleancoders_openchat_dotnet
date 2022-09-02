namespace OpenChat.Domain.Users
{
    public class UserDto
    {
        public UserDto(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public UserDto(string username, string password, string about)
            : this(username, password)
        {
            About = about;
        }

        public string Username { get; }

        public string Password { get; }

        public string? About { get; }
    }
}