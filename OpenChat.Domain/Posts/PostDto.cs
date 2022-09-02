namespace OpenChat.Domain.Posts
{
    public class PostDto
    {
        public PostDto(Guid userId, string text)
        {
            UserId = userId;
            Text = text;
        }

        public Guid UserId { get; }
        
        public string Text { get; }
    }
}