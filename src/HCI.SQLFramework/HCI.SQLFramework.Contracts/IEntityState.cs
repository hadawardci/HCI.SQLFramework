using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Contracts
{

    /// <summary>
    /// State of entity
    /// </summary>
    public interface IEntityState
    {
        /// <summary>
        /// Data exists
        /// </summary>
        [JsonIgnore, NotMapped]
        bool IsExists { get; set; }
    }
}
