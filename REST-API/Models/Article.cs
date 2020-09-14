using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ArticleID { get; set; }

        [Required]
        [Column("authorID")]
        public int UserID { get; set; }

        [Required]
        [Column("createdDate")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("intro")]
        public string Introduction { get; set; }
    }
}
