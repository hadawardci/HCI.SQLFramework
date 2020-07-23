using EasyDapper.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HCI.SQLFramework.XUnitTest.Entities.Enterprise
{
    public class TrademarkRPI: IEntityState
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RPINumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsSent { get; set; }

        public string GetId()
        {
            return RPINumber;
        }
        [NotMapped]
        public string UniqueCode { get { return RPINumber; } set { RPINumber = value; } }
        [NotMapped]
        public bool IsExists { get; set; }
    }
}
