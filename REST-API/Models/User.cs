using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int UserID { get; set; }

        [Required]
        [Column("user")]
        public string UserName { get; set; }

        [Required]
        [Column("pass")]
        public string PassWord { get; set; }
    }
}
