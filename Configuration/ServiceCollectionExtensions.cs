namespace DataSharedLib.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataSharedLib.Connection;
using DataSharedLib.Services;
using DataSharedLib.Validation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionOptions>(configuration.GetSection(DatabaseConnectionOptions.SectionName));
        RegisterCoreServices(services);
        return services;
    }

    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, Action<DatabaseConnectionOptions> configureOptions)
    {
        services.Configure(configureOptions);
        RegisterCoreServices(services);
        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IRequestValidator, RequestValidator>();
        services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
        services.AddScoped<IDatabaseQueryService, DatabaseQueryService>();
        services.AddScoped<IDatabaseCrudService, DatabaseCrudService>();
        services.AddScoped<IDatabaseBulkService, DatabaseBulkService>();
        services.AddScoped<IStoredProcedureService, StoredProcedureService>();
    }
}
