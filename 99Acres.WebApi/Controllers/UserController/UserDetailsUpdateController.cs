using _99Acres.Service.Entities.UserEntities;
using _99Acres.Service.Interface.UserInterface;
using _99Acres.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _99Acres.WebApi.Controllers.UserController
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UserDetailsUpdateController : Controller
    {
        public readonly IUserUpdateDetails _userUpdateDetails;
        public UserDetailsUpdateController(IUserUpdateDetails userUpdateDetails)
        {
            _userUpdateDetails = userUpdateDetails;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdateRequest request)
        {
            UserUpdateResponse response = new UserUpdateResponse();
            try
            {
                response = await _userUpdateDetails.UpdateUser(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }
    }
}
