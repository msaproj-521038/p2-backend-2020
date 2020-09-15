using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.Models
{
    public class ArticleField
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int FieldID { get; set; }

        // Article has a 1 to many relationship with ArticleField.
        [Required]
        [Column("articleID")]
        public int ArticleID { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("value")]
        public string Value { get; set; }
    }
}
