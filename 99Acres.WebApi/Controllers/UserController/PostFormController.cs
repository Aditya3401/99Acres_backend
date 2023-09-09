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
        public async Task<IActionResult> PostProperty([FromForm]PostPropertyRecord request)
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
        [HttpGet]
     
        public async Task<IActionResult> GetProperty(int propertyId)
        {
            PostPropertyResponse response = new PostPropertyResponse();
            try
            {

                response = await _postproperty.GetProperty(propertyId);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> details([FromQuery] Filter filter)
        {
            try
            {
                var list = await _postproperty.GetAllProperties();
                var filterList = await _postproperty.FilterProperties(list, filter);
                return Ok(filterList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetPostFormDetails()
        {
            try
            {
                var list = await _postproperty.GetAllProperties();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
