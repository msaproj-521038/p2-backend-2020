using REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.DTO
{
    public class ArticleEditTemplateDTO
    {
        //Basic authentication to verify user.
        public string UserName { get; set; }
        public string PassWord { get; set; }

        // Article content suggested edits.
        public string Title { get; set; }
        public string Introduction { get; set; }
    }
}
