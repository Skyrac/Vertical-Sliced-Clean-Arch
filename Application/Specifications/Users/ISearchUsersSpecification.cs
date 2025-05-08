using Application.Repositories;
using Domain;

namespace Application.Specifications.Users;

public interface ISearchUsersSpecification : ISpecification<User>
{
    public ISearchUsersSpecification ByName(string name);
}
