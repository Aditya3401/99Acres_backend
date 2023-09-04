using _99Acres.Service.Entities.PostProperty;
using _99Acres.Service.Interface.UserInterface;
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
        public async  Task<IActionResult> PostFormEntry([FromForm] PostPropertyRecord record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int propertyId =await _postproperty.PostPropertyDetails(record);
                return Ok(propertyId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
