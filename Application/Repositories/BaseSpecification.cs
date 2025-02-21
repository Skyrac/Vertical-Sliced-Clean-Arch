using System.Linq.Expressions;
using Application.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories.Specification;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; }

    List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> IncludeExpressions { get; }
        = new ();

    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public bool? OrderAscending { get; private set; }

    protected BaseSpecification()
    {
    }

    public Func<IQueryable<T>, IIncludableQueryable<T, object>>[] GetIncludes()
    {
        return IncludeExpressions.ToArray();
    }


    public static BaseSpecification<T> Create()
    {
        return new BaseSpecification<T>();
    }

    protected void AddInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression)
    {
        IncludeExpressions.Add(includeExpression);
    }
    
    public BaseSpecification<T> ApplyCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
        return this;
    }
    
    public BaseSpecification<T> ApplyOrder(
        bool isAscending, 
        Expression<Func<T, object>>? orderByExpression = null)
    {
        OrderBy = orderByExpression;
        OrderAscending = isAscending;
        return this;
    }
}