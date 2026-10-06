namespace DataSharedLib.Configuration;

using DataSharedLib.Connection;
using DataSharedLib.Services;
using DataSharedLib.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConnectionOptions>(configuration.GetSection(DatabaseConnectionOptions.SectionName));
        return services.AddDataSharedLibCore();
    }

    public static IServiceCollection AddDataSharedLib(this IServiceCollection services, Action<DatabaseConnectionOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services.AddDataSharedLibCore();
    }

    private static IServiceCollection AddDataSharedLibCore(this IServiceCollection services)
    {
        services.AddSingleton<IRequestValidator, RequestValidator>();
        services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
        services.AddTransient<IDatabaseQueryService, DatabaseQueryService>();
        services.AddTransient<IDatabaseCrudService, DatabaseCrudService>();
        services.AddTransient<IDatabaseBulkService, DatabaseBulkService>();
        services.AddTransient<IStoredProcedureService, StoredProcedureService>();

        return services;
    }
}
