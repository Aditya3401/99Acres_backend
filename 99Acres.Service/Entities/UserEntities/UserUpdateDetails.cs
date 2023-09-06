using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.UserEntities
{
    public class UserUpdateRequest
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Email Is Mandetory")]
        public string Email { get; set; }
        public string UserName { get; set; }
       
        [Required(ErrorMessage = "ContactNumber Is Mandetory")]
        public string ContactNo { get; set; }
    }
    public class UserUpdateResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
