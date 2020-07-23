using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCI.EasyDapper.XUnitTest.Entities.Sakila
{
    [Table("actor", Schema ="sakila")]
    public class Actor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int actor_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime last_update { get; set; }
        public ICollection<FilmeActor> FilmeActor { get; set; }
    }

    [Table("film", Schema ="sakila")]
    public class Film
{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int film_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int release_year { get; set; }
        public string rating { get; set; }
        public DateTime last_update { get; set; }
        public ICollection<FilmeActor> FilmeActor { get; set; }
    }
    
    [Table("film_actor", Schema = "sakila")]
    public class FilmeActor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int actor_id { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int film_id { get; set; }
        public DateTime last_update { get; set; }
        [ForeignKey("film_id")]
        public Film Film { get; set; }
        [ForeignKey("actor_id")]
        public Actor Actor { get; set; }
    }

}
