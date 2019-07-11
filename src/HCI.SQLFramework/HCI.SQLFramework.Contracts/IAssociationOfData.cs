using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Contracts
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
