using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.UserEntities
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "UserId Is Mandetory")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Mandetory")]
        public string Password { get; set; }
    }

    public class UserLoginResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public UserLoginInformation data { get; set; }
        public UserToken UserToken { get; set; }
    }

    public class UserLoginInformation
    {
        public int UserId { get; set; }
        public string Email { get; set; }
  
    }
}
