using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST_API.DTO
{
    public class PasswordResetDTO
    {
        // Ensure that the user that wants to change password knows the old password.
        public string OldPassWord { get; set; }

        public string NewPassWord { get; set; }
    }
}
