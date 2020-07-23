using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace HCI.EasyDapper.Model
{
    public interface ISearchable<TSource> : IQueryable<TSource>
    {
        TSource Find(Expression<Func<TSource, bool>> predicate);
        IList<TResult> Load<TResult>(Expression<Func<TSource, TResult>> selector);
        IList<TResult> DistinctLoad<TResult>(Func<TSource, TResult> selector);
        IQueryable<TSource> And(Expression<Func<TSource, bool>> predicate);
        long Count(Expression<Func<TSource, bool>> predicate);
        long Max(Expression<Func<TSource, bool>> predicate);
        long Min(Expression<Func<TSource, bool>> predicate);
        long Sum(Expression<Func<TSource, bool>> predicate);
    }

    internal sealed class Searchable<TSource> : ISearchable<TSource>
    {
        public Searchable(IQueryProvider provider) => Provider = provider ?? throw new ArgumentNullException("provider");
        public Searchable(IQueryProvider provider, Expression<Func<TSource, bool>> predicate)
        {
            Provider = provider ?? throw new ArgumentNullException("provider");
            Expression = predicate?.Body ?? throw new ArgumentNullException("expression");
        }

        public Type ElementType => typeof(TSource);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; }

        //private readonly DataContext<TProvider> _context;

        public IEnumerator<TSource> GetEnumerator() => Provider.CreateQuery<TSource>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Provider.CreateQuery(Expression).GetEnumerator();

        public TSource Find(Expression<Func<TSource, bool>> predicate)
        {
            Expression = predicate.Body;
            return Provider.Execute<TSource>(Expression);
        }

        public IQueryable<TSource> And(Expression<Func<TSource, bool>> predicate)
        {
            Expression = (Expression == null) ? predicate.Body : Expression.AndAlso(Expression, predicate.Body);
            return this;
        }

        public IList<TResult> Load<TResult>(Expression<Func<TSource, TResult>> selector) //=> ((ICustomProvider)Provider).CustomSelect<TSource, TResult>(Expression, selector.ToString());
        {
            var label = Expression.Label(selector.Body.ToString());
            Expression = Expression.Block(Expression, Expression.Return(label));
            var result = Provider.CreateQuery<TResult>(Expression).ToList();
            return result;
        }

        public IList<TResult> DistinctLoad<TResult>(Func<TSource, TResult> selector) => ((ICustomProvider)Provider).CustomDistinct<TSource, TResult>(Expression, selector.ToString());

        public long Count(Expression<Func<TSource, bool>> predicate)
        {
            Expression = predicate.Body;
            return ((ICustomProvider)Provider).CustomCount<TSource>(Expression);
        }

        public long Max(Expression<Func<TSource, bool>> predicate)
        {
            Expression = predicate.Body;
            return ((ICustomProvider)Provider).CustomMax<TSource>(Expression);
        }

        public long Min(Expression<Func<TSource, bool>> predicate)
        {
            Expression = predicate.Body;
            return ((ICustomProvider)Provider).CustomMin<TSource>(Expression);
        }

        public long Sum(Expression<Func<TSource, bool>> predicate)
        {
            Expression = predicate.Body;
            return ((ICustomProvider)Provider).CustomSum<TSource>(Expression);
        }

    }
}
