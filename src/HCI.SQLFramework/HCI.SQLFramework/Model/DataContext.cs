using Dapper;
using HCI.SQLFramework.Contracts;
using HCI.SQLFramework.Data;
using PESALEXMapper.Helper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace HCI.SQLFramework.Model
{
    public class DataContext : IDataContext
    {
        private readonly TransactionScope _transaction;
        private readonly DbConnection _connection;

        public DataContext(string connectionString, bool isTransaction = false)
        {
            if (isTransaction)
                _transaction = new TransactionScope();
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _transaction.Dispose();
                    _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~DataContext()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region Operation

        private long Insert<T>(T data, bool autoIncrement = true) where T : class => Insert(data, typeof(T).Name, autoIncrement);
        private long Insert(object data, string tableName, bool autoIncrement = true)
        {
            string IDENTITY = "SELECT @@IDENTITY AS 'Identity'";
            var request = Mapper.GetSQLRequest(data);
            var param = autoIncrement ? request.ParametersWithoutKeys() : request.ParametersWithKeys();
            var parametersNames = autoIncrement ? request.ParametersNames : request.ParametersNamesWithKeys;
            var valuesClause = autoIncrement ? request.ValuesClause : request.ValuesClauseWithKeys;
            var increment = autoIncrement ? IDENTITY : string.Empty;
            if (autoIncrement)
            {
                var sql = $"INSERT INTO [{tableName}] ({parametersNames}) VALUES ({valuesClause}) {IDENTITY}";
                return Query<long>(sql, param).FirstOrDefault();
            }
            else
            {
                var sql = $"INSERT INTO [{tableName}] ({parametersNames}) VALUES ({valuesClause})";
                return Execute(sql, param) ? 1 : 0;

            }
        }

        private bool Update<T>(T data) where T : class => Update(data, typeof(T).Name);
        private bool Update(object data, string tableName)
        {
            var result = false;
            try
            {
                var request = Mapper.GetSQLRequest(data);
                var param = request.ParametersWithKeys();
                var sql = $"UPDATE {tableName} SET {request.SetClause} WHERE {request.WhereClause}";
                result = Execute(sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private bool Delete<T>(T data) where T : class => Delete(data, typeof(T).Name);
        private bool Delete(object data, string tableName)
        {
            var result = false;
            try
            {
                var request = Mapper.GetSQLRequest(data);
                var param = request.ParametersKeys();
                var sql = $"DELETE FROM [{tableName}] WHERE {request.WhereClause}";
                result = Execute(sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private bool Delete(long id, string tableName, string key = "id")
        {
            var result = false;
            try
            {
                var sql = $"DELETE FROM [{tableName}] WHERE [{key}] = {id}";
                result = Execute(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #region Do Command

        private bool Execute(string sql, object param = null)
        {
            bool result;
            result = _connection.Execute(sql, param) > 0;
            return result;
        }

        private T FirstOrdefault<T>(string sql, object param = null) => Query<T>(sql, param).FirstOrDefault();
        private IEnumerable<T> Query<T>(string sql, object param = null)
        {
            IEnumerable<T> result = new List<T>();
            result = _connection.Query<T>(sql, param);
            return result;
        }

        #endregion

        #endregion

        #region SQL
        public void Complete() => _transaction.Complete();

        public bool Remove<T>(T entity) where T : class => Delete(entity);
        public bool Remove(object data, string tableName) => Delete(data, tableName);
        public bool Remove(long id, string tableName, string key) => Delete(id, tableName, key);


        public bool SetComposition<TEntity>(ICollection<TEntity> collection, string propertyName = null, long? propertyValue = null, bool autoIncrement = true)
            where TEntity : class
        {
            var result = new List<TEntity>();
            var resultStatus = true;
            if (collection != null)
                foreach (var entity in collection)
                {
                    try
                    {
                        if ((ImplementationUtil.ContainsProperty(entity.GetType(), propertyName) && 0 == (long)MapperUtil.GetValue(entity, propertyName))
                            || (ImplementationUtil.ContainsInterface(entity.GetType(), nameof(IEntityState)) && !((IEntityState)entity).IsExists))
                        {
                            if (propertyValue.HasValue && !string.IsNullOrWhiteSpace(propertyName))
                                MapperUtil.SetValue(entity, propertyName, propertyValue);
                            if (Insert(entity, entity.GetType().Name, autoIncrement) > 0)
                                result.Add(entity);
                        }
                        else if (((IAssociationOfData)entity).IsKeep && Update(entity))
                            result.Add(entity);
                        else
                            Delete(entity);
                    }
                    catch (Exception ex)
                    {
                        resultStatus = false;
                        throw ex;
                    }
                }

            collection = result;
            return resultStatus;
        }


        public bool SaveOrUpdate<TEntity>(TEntity entity, string keyName = "Id", long? value = null)
            where TEntity : class
        {
            var result = false;
            if (entity != null)
            {
                try
                {
                    if (Exists(entity, keyName))
                        result = Update(entity);
                    else
                    {
                        var oldValue = (long)MapperUtil.GetValue(entity, keyName);
                        try
                        {
                            if (value.HasValue && value.Value != oldValue)
                                MapperUtil.SetValue(entity, keyName, value);
                            result = Save(entity, keyName, !value.HasValue);
                        }
                        catch (Exception)
                        {
                            if (value.HasValue && value.Value != oldValue)
                                MapperUtil.SetValue(entity, keyName, oldValue);
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    throw ex;
                }
            }
            return result;
        }


        public void SetAggregation<TEntity>(ICollection<TEntity> collection, string propertyName = null, long? propertyValue = null)
            where TEntity : IEntityState, IAssociationOfData
        {
            var result = new List<TEntity>();
            foreach (var entity in collection)
            {
                if (entity != null)
                {
                    try
                    {
                        var tableName = entity.GetType().Name;
                        if (!((IAssociationOfData)entity).IsKeep)
                            Delete(entity, tableName);
                        else if (!((IEntityState)entity).IsExists && Insert(entity, tableName, false) > 0)
                            result.Add(entity);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            collection = result;
        }

        private static bool Exists<TEntity>(TEntity entity, string keyName) where TEntity : class
        {
            var isExists = false;
            if (ImplementationUtil.ContainsInterface(entity.GetType(), nameof(IEntityState)))
                isExists = ((IEntityState)entity).IsExists;
            else if (ImplementationUtil.ContainsProperty(entity.GetType(), "Id"))
                isExists = ((entity as dynamic).Id > 0);
            else if (ImplementationUtil.ContainsProperty(entity.GetType(), keyName))
                isExists = MapperUtil.GetValue(entity, keyName).ToString() != "0";
            return isExists;
        }

        private bool Save<TEntity>(TEntity entity, string keyName, bool autoIncrement = true) where TEntity : class
        {
            bool result;
            var key = Insert(entity, autoIncrement);
            if ((ImplementationUtil.ContainsProperty(entity.GetType(), keyName) && autoIncrement))
                MapperUtil.SetValue(entity, keyName, key);
            result = key > 0;
            return result;
        }

        #endregion

    }
}
