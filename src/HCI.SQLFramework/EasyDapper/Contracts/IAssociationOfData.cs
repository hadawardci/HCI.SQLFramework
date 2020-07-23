using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDapper.Contracts
{
    /// <summary>
    /// State of association between data
    /// </summary>
    public interface IAssociationOfData
    {
        /// <summary>
        /// Keep Association
        /// </summary>
        [JsonIgnore, NotMapped]
        bool IsKeep { get; set; }


    }
}
