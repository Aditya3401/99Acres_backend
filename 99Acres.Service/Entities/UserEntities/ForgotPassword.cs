using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.UserEntities
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
    public class ForgotPasswordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Information data { get; set; }

    }
    public class Information
    {
        public string Email { get; set; }

    }
    public class Employee
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }
        public DateTime TokenCreationTime { get; set; }
    }
}
