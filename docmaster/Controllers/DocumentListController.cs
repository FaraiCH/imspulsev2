using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace docmaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentListController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("Company={company}&WichType={passdoc}&Mode={mode}")]
        public ActionResult<string> GetDocument(string company, string passdoc, string mode)
        {
            var documents = new List<Tuple<string, string, string>>();
            documents.Add(new Tuple<string, string, string>("12", "something", "something"));
            string json = JsonConvert.SerializeObject(new {            
                documents           
            });
            return json;
        }
       
       
    }
}
