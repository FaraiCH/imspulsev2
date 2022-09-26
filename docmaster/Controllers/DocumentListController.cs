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
        [HttpGet("Company={company}&WichType={passdoc}")]
        public ActionResult<string> Passowrddoc(string company, string passdoc)
        {
            return "document password";
        }
    }
}
