using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HCI.SQLFramework.Contracts
{
    public interface IDataContext : IDisposable
    {
        void Complete();
        bool Remove(object data, string tableName);
        bool Remove(long id, string tableName, string key);
        bool Remove<T>(T entity)
            where T : class;

        /// <summary>
        /// Assign status to the template or composition
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="collection">collection of data</param>
        /// <returns></returns>
        [Obsolete]
        bool SaveOrUpdate<TEntity>(TEntity entity, string keyName, long? value) where TEntity : class;

        /// <summary>
        /// Assign status to the template or composition
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="collection">collection of data</param>
        /// <returns></returns>
        bool SaveOrUpdate<TEntity>(TEntity entity, bool isNavigated = true) where TEntity : class;

        /// Assign status to validation if collection is added
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="collection">collection of data</param>
        void SetAggregation<TEntity>(ICollection<TEntity> collection, string propertyName = null, long? propertyValue = null)
            where TEntity : IEntityState, IAssociationOfData;

        /// <summary>
        /// Assign status to composition and validate if you want to add collection
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="collection">collection of data</param>
        [Obsolete]
        bool SetComposition<TEntity>(ICollection<TEntity> collection, string propertyName, long? propertyValue, bool autoIncrement = true)
            where TEntity : class;

        /// <summary>
        /// Assign status to composition and validate if you want to add collection
        /// </summary>
        /// <typeparam name="TEntity">entity type</typeparam>
        /// <param name="collection">collection of data</param>
        bool SetComposition<TEntity>(ICollection<TEntity> collection) where TEntity : class;

        #region SELECT

        IList<TResult> Where<TEntity, TResult>(Expression<Func<TEntity, bool>> whereClause, Func<TEntity, TResult> SelectClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        IList<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        long Count<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        long Avg<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        long Sum<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        long Max<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        long Min<TEntity>(Expression<Func<TEntity, bool>> whereClause, bool isDistinct = false, int? top = null, string orderBy = null) where TEntity : class;
        IList<TEntity> Query<TEntity>(string sql) where TEntity : class;

        #endregion
    }
}
