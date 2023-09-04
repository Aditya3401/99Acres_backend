using _99Acres.Service.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Interface.UserInterface
{
    public interface IUserToken
    {
        public string GenerateJWTToken(int UserId);
        public Task UserTokenDB(UserLoginResponse response);
        public string GenerateRefresh();
    }
}
