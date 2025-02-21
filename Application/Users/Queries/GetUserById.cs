using Application.Repositories;
using Application.Users.DTOs;
using Carter;
using Domain;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Application.Users.Queries;

public static class GetUser
{
    public const string Endpoint = "api/users/{userId}";
    public class GetUserQuery : IRequest<UserDto>
    {
        public required Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<GetUserQuery>
    {
        public Validator()
        {
            RuleFor(c => c.Id != default);
        }
    }

    internal sealed class Handler (IRepository<User> repository)
        : IRequestHandler<GetUserQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await repository.GetById(request.Id);
            // var altUser = await repository
            //     .QueryBySpecification(new GetUserSpecification(x => x.Id == request.Id), cancellationToken);
            return user.Adapt<UserDto>();
        }
    }

}

public class GetUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(GetUser.Endpoint, async (Guid userId, ISender sender) =>
        {
            var result = await sender.Send(new GetUser.GetUserQuery { Id = userId });
            return result;
        })
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            //.RequireAuthorization()
            /*
            .RequireAuthorization(t => t.RequireAuthenticatedUser().RequireAssertion(a =>
            {
                using var scope = _serviceProvider.CreateScope();
                var auth = scope.ServiceProvider.GetRequiredService<IAuth>();
                return auth.IsAdmin;
            }))
             */
            .WithDescription("Get User based on authorization")
            .WithTags("User")
            .WithOpenApi();
    }
}
