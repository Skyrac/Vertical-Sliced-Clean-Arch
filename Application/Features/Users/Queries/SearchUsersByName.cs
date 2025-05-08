using Application.Common;
using Application.Common.Extensions;
using Application.Features.Users.DTOs;
using Application.Specifications.Users;
using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Users.Queries;

public static class SearchUsersByName
{
    /// <summary>
    /// Route zum Suchen von Usern.
    /// </summary>
    public const string Endpoint = "api/users/search";

    /// <summary>
    /// Query-Request (hier als GET-Parameter).
    /// </summary>
    /// <param name="Name">Suchbegriff für den Namen der Userart</param>
    public record Query([FromQuery] string Name) : IRequest<Result<List<UserDto>>>;

    /// <summary>
    /// Validator, um sicherzustellen, dass der Name nicht leer ist.
    /// </summary>
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    /// <summary>
    /// Handler für die tatsächliche Datenbanksuche.
    /// Wir verwenden hier ein IRepository, das ein IQueryable liefert.
    /// </summary>
    internal sealed class Handler(ISearchUsersSpecification repo)
        : IRequestHandler<Query, Result<List<UserDto>>>
    {
        public async Task<Result<List<UserDto>>> Handle(Query request, CancellationToken ct)
        {
            // Beispielhafte Suche per EF.Functions.PlainToTsQuery + Matches()
            // Wir greifen auf das bereits konfigurierte SearchVector zu.
            var users = await repo.ByName(request.Name).Execute(ct);

            return Result<List<UserDto>>.Success(users);
        }
    }
}

public class SearchUsersByNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET-Route, die den Query-Parameter (Name) übernimmt
        app.MapGet(
                SearchUsersByName.Endpoint,
                async ([AsParameters] SearchUsersByName.Query query, ISender sender) =>
                {
                    // MediatR-Request anstoßen
                    var result = await sender.Send(query);
                    return result.ToHttpResult();
                }
            )
            .Produces<List<UserDto>>() // Erfolgreiche Rückgabe: Liste UserDto
            .Produces(StatusCodes.Status400BadRequest) // Bei Validation-Fehlern
            .WithName("SearchUsersByName")
            .WithTags("Users");
    }
}
