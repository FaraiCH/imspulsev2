using Aspose.Words;
using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace docmaster.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentListController : ControllerBase
    {
        UserManager<docmasterUser> _userManager;
        // GET api/values/5
        [HttpGet("Company={company}&WichType={passdoc}&Mode={mode}")]
        public async Task<ActionResult<string>> GetDocument(string company, string passdoc, string mode)
        {
            var documents = new List<Tuple<string, string, string>>();
            string[] directory = Directory.GetFiles("/vaw/www/html/imspulse/bunch-box/" + company + "/", "*", System.IO.SearchOption.AllDirectories);       
            foreach (string f in directory)
            {
                //FileFormatInfo info = FileFormatUtil.DetectFileFormat(d + "Document.doc");
                documents.Add(new Tuple<string, string, string>(f, "something", "something"));
            }
            string json = JsonConvert.SerializeObject(new
            {
                documents
            });

            return json;
        }
       
       
    }
}
