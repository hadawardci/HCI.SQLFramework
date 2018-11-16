using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Contracts
{

    /// <summary>
    /// Estado da entidade
    /// </summary>
    public interface IEntityState
    {
        /// <summary>
        /// Pré carregado do banco de dados
        /// </summary>
        [JsonIgnore, NotMapped]
        bool IsExists { get; set; }
    }
}
