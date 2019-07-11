using System.Linq;

namespace HCI.SQLFramework.Contracts
{
    public interface IDataQuery<T> : IOrderedQueryable<T>
    {
    }
}