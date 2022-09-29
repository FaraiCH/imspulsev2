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
            var documents = new List<Tuple<string, int, string>>();
            int counter = 0;
            string[] directory = Directory.GetFiles("/var/www/html/imspulse/bunch-box/" + company, "*", System.IO.SearchOption.AllDirectories);
            //string[] directory = Directory.GetFiles(@"C:\Testing\", "*", System.IO.SearchOption.AllDirectories);       
            foreach (string f in directory)
            {
                FileFormatInfo info = FileFormatUtil.DetectFileFormat(f);
                if(info.IsEncrypted == true)
                {
                    //Count All encrypted documents
                    counter ++;
                }              
            }

            documents.Add(new Tuple<string, int, string>("Documents Protected", counter, "1"));
            string json = JsonConvert.SerializeObject(new
            {
                documents
            });

            return json;
        }
       
       
    }
}
