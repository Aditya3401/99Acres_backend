using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.User
{
    public class UserRegisterRequest
    {
        [Required(ErrorMessage = "UserId Is Mandetory")]
        public int UsertId { get; set; }
        [Required(ErrorMessage = "UserName Is Mandetory")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email Is Mandetory")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Mandetory")]
        public string Password { get; set; }
        [Required(ErrorMessage = "ContactNumber Is Mandetory")]
        public string ContactNo { get; set; }
    }
    public class UserRegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
