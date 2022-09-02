namespace OpenChat.Api.Models
{
    public class FollowRequest
    {
        public Guid? FollowerId { get; set; }
        
        public Guid? FolloweeId { get; set; }      
    }
}