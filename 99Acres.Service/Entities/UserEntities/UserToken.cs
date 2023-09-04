using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Entities.UserEntities
{
    public class UserToken
    {
        public string JWTToken { get; set; }
        public string UserRefreshToken { get; set; }
    }
}
