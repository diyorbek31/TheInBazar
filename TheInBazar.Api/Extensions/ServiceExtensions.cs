using TheInBazar.Data.IRepositories;
using TheInBazar.Data.Repositories;
using TheInBazar.Service.Interfaces;
using TheInBazar.Service.Services;

namespace TheInBazar.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddCustomService(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped(typeof(IRepository<>),typeof(Repository<>));

    }
}
