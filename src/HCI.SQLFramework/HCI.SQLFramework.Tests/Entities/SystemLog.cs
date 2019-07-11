using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Tests.Entities
{
    public class SystemLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Application { get; set; }
        public string Version { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
