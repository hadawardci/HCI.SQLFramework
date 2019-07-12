using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.SQLFramework.Tests.Entities
{
    [Table("KeyPair", Schema = "class4u_dev")]
    public class KeyPair
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
