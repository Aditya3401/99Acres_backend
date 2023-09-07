using _99Acres.Service.Entities.PostProperty;
using _99Acres.Service.Entities.User;
using _99Acres.Service.Interface.UserInterface;
using _99Acres.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _99Acres.WebApi.Controllers.UserController
{
    [Route("api/[controller]/[Action]")]
    [ApiController]

    public class PostFormController : Controller
    {
        public readonly IPostProperty _postproperty;
        public PostFormController(IPostProperty postproperty)
        {
            _postproperty = postproperty;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostProperty(PostPropertyRecord request)
        {
            PostPropertyResponse response = new PostPropertyResponse();
            try
            {

                response = await _postproperty.PostPropertyDetails(request);

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
