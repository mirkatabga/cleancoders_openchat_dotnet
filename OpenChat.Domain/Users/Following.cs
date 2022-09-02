namespace OpenChat.Domain.Users
{
    public class Following
    {
        public Following(){}
        
        public Following(Guid followerId, Guid followeeId)
        {
            if (followerId == Guid.Empty)
            {
                throw new ArgumentException($"Empty argument: {nameof(followerId)}");
            }

            if (followeeId == Guid.Empty)
            {
                throw new ArgumentException($"Empty argument: {nameof(followeeId)}");
            }

            FollowerId = followerId;
            FolloweeId = followeeId;
        }

        public Guid FollowerId { get; private set; }

        public User? Follower { get; private set; }

        public Guid FolloweeId { get; private set; }

        public User? Followee { get; private set; }
    }
}