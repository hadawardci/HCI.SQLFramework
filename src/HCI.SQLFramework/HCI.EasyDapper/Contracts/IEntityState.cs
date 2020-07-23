using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.EasyDapper.Contracts
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
