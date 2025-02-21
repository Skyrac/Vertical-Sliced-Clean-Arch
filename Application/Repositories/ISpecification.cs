using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Repositories;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    
    Expression<Func<T, object>>? OrderBy { get; }
    bool? OrderAscending { get; }

    public Func<IQueryable<T>, IIncludableQueryable<T, object>>[] GetIncludes();
}