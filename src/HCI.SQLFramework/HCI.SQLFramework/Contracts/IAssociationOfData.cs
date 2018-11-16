using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Contracts
{
    /// <summary>
    /// Associação de dados
    /// </summary>
    public interface IAssociationOfData
    {
        /// <summary>
        /// Manter relação
        /// </summary>
        [JsonIgnore, NotMapped]
        bool IsKeep { get; set; }


    }
}
