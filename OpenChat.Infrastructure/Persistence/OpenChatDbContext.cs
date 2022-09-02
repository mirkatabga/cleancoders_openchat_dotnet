using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OpenChat.Domain.Posts;
using OpenChat.Domain.Users;

namespace OpenChat.Infrastructure.Persistence
{
    public class OpenChatDbContext : DbContext
    {
        public OpenChatDbContext(DbContextOptions<OpenChatDbContext> options)
            : base(options)
        {
        }

        public DbSet<User>? Users { get; set; }

        public DbSet<Post>? Posts { get; set; }

        public DbSet<Following>? Followings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Following>()
                .HasKey(following => new { following.FolloweeId, following.FollowerId });

            modelBuilder.Entity<User>()
                .HasMany(user => user.Followers)
                .WithOne(following => following.Followee)
                .HasForeignKey(following => following.FolloweeId);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Followees)
                .WithOne(following => following.Follower)
                .HasForeignKey(following => following.FollowerId);
        }
    }
}