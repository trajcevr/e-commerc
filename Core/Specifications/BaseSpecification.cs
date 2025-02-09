using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specifications;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null)
    {
    }

    public Expression<Func<T, bool>>? Criteria => criteria;
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public bool IsDistinct { get; private set; }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpresion)
    {
        OrderBy = orderByExpresion;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpresion)
    {
        OrderByDescending = orderByDescExpresion;
    }

    protected void ApplyDisctinct() 
    {
        IsDistinct = true;
    }
}

public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
{
    public BaseSpecification(Expression<Func<T, bool>>? criteria = null) : base(criteria)
    {
    }

    protected BaseSpecification() : this(null)
    {
    }

    public Expression<Func<T, TResult>>? Select { get; private set; }

    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }
}
