using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace docmaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentListController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("Company={company}")]
        public ActionResult<string> Get(string company)
        {
            return "value";
        }
        [HttpGet("Company={company}&WichType={passdoc}&Mode={mode}")]
        public ActionResult<string> Get(string company, string passdoc, string mode)
        {
            
            return "document password";
        }
    }
}
