using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.DTO
{
    public class ArticleContentFullDTO
    {
        public int ArticleID { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public ICollection<FieldDTO> Fields { get; set; }
    }
}
