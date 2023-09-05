using _99Acres.Service.Interface.UserInterface;
using Microsoft.AspNetCore.Mvc;

namespace _99Acres.WebApi.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearch _search;

        public SearchController(ISearch search)
        {
            _search = search;
        }

        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var searchList = _search.searchProperty(searchTerm);
                return Ok(searchList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

}
