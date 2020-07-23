using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDapper.Contracts
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
