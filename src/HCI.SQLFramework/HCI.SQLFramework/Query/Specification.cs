using HCI.SQLFramework.Contracts;
using HCI.SQLFramework.Data;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HCI.SQLFramework.Query
{
    public class Specification//<T>
         //where T : class
    {

        public IDataQuery<T> Where<T>(Expression<Func<T, bool>> expression)
        {
            var result = Activator.CreateInstance<DataQuery<T>>();
            result.SetExpression(expression);
            var test = result.Where(x => x != null);
            return result;
        }

    }

}
