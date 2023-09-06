using _99Acres.Service.Entities.User;
using _99Acres.Service.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _99Acres.Service.Interface.UserInterface
{
    public interface IUserAuthentication
    {
        public Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request);
        public Task<UserLoginResponse> LoginUser(UserLoginRequest request);
        public Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request);
        public Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request);
    }
}
