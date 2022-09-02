using Microsoft.EntityFrameworkCore;
using OpenChat.Domain.Common;
using OpenChat.Domain.Posts;
using OpenChat.Domain.UseCases;
using OpenChat.Domain.Users;
using OpenChat.Infrastructure.Persistence;
using OpenChat.Infrastructure.Persistence.Repositories;

namespace OpenChat.Api
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDomainLayer(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<GetWallUseCase>();
        }

        public static void AddInfrastructureLayer(this IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddDebug());
            services.AddDbContext<OpenChatDbContext>(opt => opt.UseInMemoryDatabase("OpenChatDatabase"));
            services.AddScoped<IUsersRepository, EfUsersRepository>();
            services.AddScoped<IPostsRepository, EfPostsRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        }
    }
}