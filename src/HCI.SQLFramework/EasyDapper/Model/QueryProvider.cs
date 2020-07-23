using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace EasyDapper.Model
{
    public sealed class QueryProvider<TConnection> : ICustomProvider
        where TConnection : DbConnection
    {
        private DataContext<TConnection> _context;

        public QueryProvider(DataContext<TConnection> context)
        {
            _context = context;
        }

        public IQueryable CreateQuery(Expression expression)
        {

            var block = (BlockExpression)expression;

            /*             
            //var body = expression.Body.ToString();

            //// var query = new System.Text.StringBuilder();
            //foreach (var item in expression.Parameters)
            //{
            //    body = body.Replace($"{item.Name}.", $"{schema}[{item.Type.Name}].");
            //}
            //body = body.Replace("==", "=");
            //throw new NotImplementedException(); 
             */

            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var block = (BlockExpression)expression;
            var target = ((GotoExpression)block.Result).Target;
            //target.Type;
            //target.Name.Replace
            /*             
            //var body = expression.Body.ToString();

            //// var query = new System.Text.StringBuilder();
            //foreach (var item in expression.Parameters)
            //{
            //    body = body.Replace($"{item.Name}.", $"{schema}[{item.Type.Name}].");
            //}
            //body = body.Replace("==", "=");
            //throw new NotImplementedException(); 
             */
            throw new NotImplementedException();
        }

        public long CustomCount<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IList<TResult> CustomDistinct<T1, TResult>(Expression expression, string v)
        {
            throw new NotImplementedException();
        }

        public long CustomMax<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public long CustomMin<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IList<TResult> CustomSelect<TSource, TResult>(Expression expression, string selector)
        {
            throw new NotImplementedException();
        }

        public long CustomSum<T>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }

    internal interface ICustomProvider : IQueryProvider
    {
        IList<TResult> CustomSelect<TSource, TResult>(Expression expression, string selector);
        IList<TResult> CustomDistinct<T1, TResult>(Expression expression, string v);
        long CustomCount<T>(Expression expression);
        long CustomMax<T>(Expression expression);
        long CustomMin<T>(Expression expression);
        long CustomSum<T>(Expression expression);
    }
}
