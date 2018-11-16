using System;
using System.Collections.Generic;

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
        /// Atribuir status ao modelo ou composição
        /// </summary>
        /// <typeparam name="TEntity">tipo de entidade</typeparam>
        /// <param name="context">contexto do Entity Framework</param>
        /// <param name="entity"></param>
        /// <param name="isDelete"></param>
        /// <param name="keyName">nome da chave primaria</param>
        /// <returns></returns>
        bool SaveOrUpdate<TEntity>(TEntity entity, string keyName = "Id", long? value = default(long?))
            where TEntity : class;
        /// <summary>
        /// Atribuir status a agregação valida se será adicionada a coleção
        /// </summary>
        /// <typeparam name="TEntity">tipo de entidade</typeparam>
        /// <param name="context">contexto do Entity Framework</param>
        /// <param name="collection">listagem</param>
        /// <param name="propertyName">propriedade</param>
        /// <param name="propertyValue">valor</param>
        void SetAggregation<TEntity>(ICollection<TEntity> collection, string propertyName = null, long? propertyValue = null)
            where TEntity : IEntityState, IAssociationOfData;
        /// <summary>
        /// Atribuir status a composição e valida se será adicionada a coleção
        /// </summary>
        /// <typeparam name="TEntity">tipo de entidade</typeparam>
        /// <param name="collection">coleção</param>
        /// <param name="propertyValue">valor a atribuir</param>
        /// <param name="propertyName">nome da propriedade</param>
        bool SetComposition<TEntity>(ICollection<TEntity> collection, string propertyName = null, long? propertyValue = default(long?), bool autoIncrement = true)
            where TEntity : class;
    }
}
