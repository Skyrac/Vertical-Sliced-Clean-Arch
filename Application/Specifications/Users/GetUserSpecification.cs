using System.Linq.Expressions;
using Domain;
using Infrastructure.Repositories.Specification;

namespace Application.Specifications.Users;

public class GetUserSpecification : BaseSpecification<User>
{
    public GetUserSpecification(Expression<Func<User, bool>> criteria)
    {
        ApplyCriteria(criteria);
    }
}