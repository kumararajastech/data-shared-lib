using DataSharedLib.Connection;
using DataSharedLib.Services;
using DataSharedLib.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataSharedLib.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionOptions>(
            configuration.GetSection(DatabaseConnectionOptions.SectionName));

        return AddCoreServices(services);
    }

    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, Action<DatabaseConnectionOptions> configureOptions)
    {
        services.Configure(configureOptions);

        return AddCoreServices(services);
    }

    private static IServiceCollection AddCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IRequestValidator, RequestValidator>();
        services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
        services.AddScoped<IDatabaseQueryService, DatabaseQueryService>();
        services.AddScoped<IDatabaseCrudService, DatabaseCrudService>();
        services.AddScoped<IDatabaseBulkService, DatabaseBulkService>();
        services.AddScoped<IStoredProcedureService, StoredProcedureService>();

        return services;
    }
}
