using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TournamentCalendar.Tests;
internal class RouteHelper
{
    public static IEnumerable<(string? Method, string? Route, string? Action, string? ControllerMethod)> GetRegisteredRoutes(IServiceProvider services)
    {
        var endpoints = services.GetRequiredService<IEnumerable<EndpointDataSource>>()
            .SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>();
        var output = endpoints.Select(
            e =>
            {
                var controller = e.Metadata
                    .OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();
                var action = controller != null
                    ? $"{controller.ControllerName}.{controller.ActionName}"
                    : null;
                var controllerMethod = controller != null
                    ? $"{controller.ControllerTypeInfo.FullName}:{controller.MethodInfo.Name}"
                    : null;
                return
                    (e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods[0],
                        $"/{e.RoutePattern.RawText?.TrimStart('/')}",
                        action,
                        controllerMethod);
            }
        );

        return output;
    }
}
