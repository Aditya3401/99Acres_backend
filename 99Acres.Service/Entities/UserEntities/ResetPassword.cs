using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.UserEntities
{
    public class ResetPasswordRequest
    {
        public string Password { get; set; }

        public string ResetToken { get; set; }
    }
    public class ResetPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}
