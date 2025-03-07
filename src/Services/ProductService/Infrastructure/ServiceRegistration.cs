using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Đăng ký MongoDbContext
        services.AddSingleton<MongoDbContext>();

        // Đăng ký Repository
        services.AddScoped<IBrandRepository, MongoBrandRepository>();
        services.AddScoped<ICategoryRepository, MongoCategoryRepository>();
        services.AddScoped<IReviewRepository, MongoReviewRepository>();
        services.AddScoped<ISupplierRepository, MongoSupplierRepository>();

        return services;
    }
}
