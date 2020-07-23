using Dapper;
using HCI.EasyDapper.Contracts;
using HCI.EasyDapper.Data;
using HCI.EasyDapper.Validation;
using HCI.EasyDapper.Values;
using PESALEXMapper.Helper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace HCI.EasyDapper.Model
{
    /// <summary>
    /// Contexto de dados
    /// </summary>
    /// <typeparam name="TConnection"></typeparam>
    public class DataContext<TConnection> : IDataContext
        where TConnection : DbConnection
    {
        private readonly TransactionScope _transaction;
        private readonly DbConnection _connection;

        private static DataBaseValue? _dataBaseValue = null;
        public static DataBaseValue DataBaseValue
        {
            get
            {
                if (!_dataBaseValue.HasValue)
                {
                    var name = typeof(TConnection).Name;
                    if (name.ToLower().Contains("mysql"))
                        _dataBaseValue = DataBaseValue.MySQL;
                    else if (name.ToLower().Contains("oracle"))
                        _dataBaseValue = DataBaseValue.Oracle;
                    else
                        _dataBaseValue = DataBaseValue.MSSQL;
                }
                return _dataBaseValue.Value;
            }
        }

        /// <summary>
        /// Contexto de dados
        /// </summary>
        /// <param name="connectionString">string de conexão</param>
        /// <param name="isTransaction">habilitar transação</param>
        public DataContext(string connectionString, bool isTransaction = false)
        {
            if (isTransaction)
                _transaction = Activator.CreateInstance<TransactionScope>();
            _connection = (TConnection)Activator.CreateInstance(typeof(TConnection), connectionString);
            // var ss = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            _connection.Open();
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant callsparametersNames

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _connection.Dispose();
                }
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    GC.Collect();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
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

        private long Insert<T>(T data, bool autoIncrement = true) => Insert(data, Mapper.GetTableName<T>(), autoIncrement);
        private long Insert(object data, string tableName, bool autoIncrement = true)
        {
            var request = Mapper.GetSQLRequest(data);
            var param = autoIncrement ? request.ParametersWithoutKeys() : request.ParametersWithKeys();
            var parametersNames = autoIncrement ? request.ParametersNames : request.ParametersNamesWithKeys;
            var valuesClause = autoIncrement ? request.ValuesClause : request.ValuesClauseWithKeys;
            if (autoIncrement)
            {
                string IDENTITY = GetIDENTITY();
                var sql = $"INSERT INTO [{tableName}] ({parametersNames}) VALUES ({valuesClause}) {IDENTITY}";
                return FirstOrDefault<long>(ValidateSyntax(sql), param);
            }
            else
            {
                var sql = $"INSERT INTO [{tableName}] ({parametersNames}) VALUES ({valuesClause})";
                return Execute(ValidateSyntax(sql), param) ? 1 : 0;
            }
        }

        private static string ValidateSyntax(string sql)
        {
            if (DataBaseValue == DataBaseValue.MySQL)
                sql = sql.Replace("[", "`").Replace("]", "`");
            return sql;
        }

        private static string GetIDENTITY()
        {
            switch (DataBaseValue)
            {
                case DataBaseValue.MSSQL:
                    return "SELECT @@IDENTITY AS 'Identity'";
                case DataBaseValue.MySQL:
                    return "; SELECT LAST_INSERT_ID();";
                case DataBaseValue.Oracle:
                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        private bool Update<T>(T data) => Update(data, Mapper.GetTableName<T>());
        private bool Update(object data, string tableName)
        {
            var result = false;
            var request = Mapper.GetSQLRequest(data);
            if (string.IsNullOrWhiteSpace(request.SetClause))
                return true;
            var param = request.ParametersWithKeys();
            var sql = $"UPDATE [{tableName}] SET {request.SetClause} {request.WhereClause}";
            result = Execute(ValidateSyntax(sql), param);
            return result;
        }

        private bool Delete<T>(T data) => Delete(data, Mapper.GetTableName<T>());
        private bool Delete(object data, string tableName)
        {
            var result = false;
            var request = Mapper.GetSQLRequest(data);
            var param = request.ParametersKeys();
            var sql = $"DELETE FROM [{tableName}] {request.WhereClause}";
            result = Execute(ValidateSyntax(sql), param);
            return result;
        }

        private bool Delete(long id, string tableName, string key = "id")
        {
            var result = false;
            var sql = $"DELETE FROM [{tableName}] WHERE [{key}] = {id}";
            result = Execute(ValidateSyntax(sql));
            return result;
        }

        #region Do Command

        private bool Execute(string sql, object param = null)
        {
            bool result;
            try
            {
                result = _connection.Execute(sql, param) > 0;
            }
            catch (Exception ex)
            {
                throw new SQLException(ex.Message, sql, param);
            }
            return result;
        }

        #endregion

        #endregion

        #region SQL

        public void Complete() => _transaction.Complete();

        public bool Remove<T>(T entity) where T : class => Delete(entity);
        public bool Remove(object data, string tableName) => Delete(data, tableName);
        public bool Remove(long id, string tableName, string key) => Delete(id, tableName, key);

        [Obsolete]
        public bool SetComposition<TEntity>(ICollection<TEntity> collection, string propertyName, long? propertyValue, bool autoIncrement)
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
                            if (Insert(entity, autoIncrement) > 0)
                                result.Add(entity);
                        }
                        else if (((IAssociationOfData)entity).IsKeep && Update(entity))
                            result.Add(entity);
                        else
                            Delete(entity);
                    }
                    catch (SQLException ex)
                    {
                        resultStatus = false;
                        throw ex;
                    }
                }

            collection = result;
            return resultStatus;
        }

        public bool SetComposition<TEntity>(ICollection<TEntity> collection)
            where TEntity : class
        {
            var result = new List<TEntity>();
            var resultStatus = true;
            string[] keys = null;
            List<Type> keyType = null;
            bool? autoIncrement = null;
            if (collection != null)
                foreach (var entity in collection)
                {
                    try
                    {
                        Mapper.BindForeign(entity);
                        if (!autoIncrement.HasValue) autoIncrement = Mapper.IsAutoIncrement(entity);
                        if (keys == null)
                        {
                            keys = Mapper.GetKeyName(entity).Split('-');
                            keyType = new List<Type>();
                            foreach (var key in keys)
                                keyType.Add(entity.GetType().GetProperty(key).PropertyType);
                        }
                        if (Mapper.CheckIfIsNew(keys[0], entity) || (ImplementationUtil.ContainsInterface(entity.GetType(), nameof(IEntityState)) && !((IEntityState)entity).IsExists))
                        {
                            var value = Insert(entity, autoIncrement.Value);
                            if (autoIncrement.Value)
                                MapperUtil.SetValue(entity, keys[0], Convert.ChangeType(value, keyType[0]));
                            if (value > 0) result.Add(entity);
                        }
                        else if ((ImplementationUtil.ContainsInterface(entity.GetType(), nameof(IAssociationOfData)) && ((IAssociationOfData)entity).IsKeep))
                        {
                            if (Update(entity))
                                result.Add(entity);
                        }
                        else
                            Delete(entity);
                    }
                    catch (SQLException)
                    {
                        resultStatus = false;
                    }
                    catch (Exception)
                    {
                        resultStatus = false;
                    }
                }
            collection = result;
            return resultStatus;
        }


        [Obsolete]
        public bool SaveOrUpdate<TEntity>(TEntity entity, string keyName, long? value)
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

        public bool SaveOrUpdate<TEntity>(TEntity entity, bool isNavigated)
            where TEntity : class
        {
            var result = false;
            if (entity != null)
            {
                try
                {
                    var keyName = Mapper.GetKeyName(entity);
                    if (Exists(entity, keyName))
                        result = Update(entity);
                    else
                        result = Save(entity);
                    if (isNavigated && result)
                        Mapper.NavigationMapping(entity);
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

                    //var tableName = entity.GetType().Name;
                    if (!((IAssociationOfData)entity).IsKeep)
                        Delete(entity);
                    else if (!((IEntityState)entity).IsExists && Insert(entity, false) > 0)
                        result.Add(entity);

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
                isExists = (entity as dynamic).Id > 0;
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

        private bool Save<TEntity>(TEntity entity) where TEntity : class
        {
            bool result;
            Mapper.BindForeign(entity);
            var isAutoIncrement = Mapper.IsAutoIncrement(entity);
            var key = Insert(entity, isAutoIncrement);
            result = key > 0;
            if (isAutoIncrement && result)
            {
                var keyName = Mapper.GetKeyName(entity);
                MapperUtil.SetValue(entity, keyName, Convert.ChangeType(key, entity.GetType().GetProperty(keyName).PropertyType));
            }
            return result;
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var provider = (QueryProvider<TConnection>)Activator.CreateInstance(typeof(QueryProvider<TConnection>), this);
            var query = (ISearchable<T>)Activator.CreateInstance(typeof(Searchable<T>), provider, expression);
            // var query = new Searchable<TEntity>(provider);
            return query;//.And(expression);
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> expression)// where TEntity : class
        {
            var provider = (QueryProvider<TConnection>)Activator.CreateInstance(typeof(QueryProvider<TConnection>), this);
            var query = (ISearchable<T>)Activator.CreateInstance(typeof(Searchable<T>), provider);
            return query.Find(expression);
        }
        public IList<TEntity> Query<TEntity>(string sql, object param = null)
        {
            var result = _connection.Query<TEntity>(sql, param);
            return result.ToList();
        }

        public int Command(string sql, object param = null)
        {
            var result = _connection.Execute(sql, param);
            return result;
        }

        public T FirstOrDefault<T>(string sql, object param = null)
        {
            var result = _connection.QueryFirstOrDefault<T>(sql, param);
            return result;
        }

        #endregion

    }
}
