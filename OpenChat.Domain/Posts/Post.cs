using OpenChat.Domain.Users;

namespace OpenChat.Domain.Posts
{
    public class Post
    {
        public Post(Guid userId, string text)
        {
            Validate(userId, text);

            UserId = userId;
            Text = text;
            CreatedDate = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public string Text { get; private set; }

        public Guid UserId { get; private set; }

        public User? User { get; private set; }

        public DateTime CreatedDate { get; private set; }

        private void Validate(Guid userId, string text)
        {
            if(userId == Guid.Empty)
            {
                throw new ArgumentException($"Null, empty or whitespace argument: {nameof(userId)}");
            }

            if(string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"Null, empty or whitespace argument: {nameof(text)}");
            }
        }
    }
}