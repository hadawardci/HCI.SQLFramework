using HCI.SQLFramework.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HCI.SQLFramework.Data
{
    public class DataQuery<T> : IDataQuery<T>    //where T : class
    {
        public Expression Expression { get; private set; }

        public Type ElementType { get; set; }

        public IQueryProvider Provider => throw new NotImplementedException();

        internal void SetExpression(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
