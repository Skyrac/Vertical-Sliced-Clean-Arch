using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationRegistrationExtensions
{

    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
    {
        services.AddMediatR(
            config => config.RegisterServicesFromAssemblies(
                typeof(ApplicationRegistrationExtensions).Assembly
            ));
        return services;
    }

}