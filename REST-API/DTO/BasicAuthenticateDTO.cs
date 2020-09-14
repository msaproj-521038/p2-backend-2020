using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.DTO
{
    public class BasicAuthenticateDTO
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
