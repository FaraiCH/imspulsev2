using Aspose.Words;
using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace docmaster.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class DocumentListController : ControllerBase
    {
        UserManager<docmasterUser> _userManager;
        // GET api/values/5
        [HttpGet("Company={company}&WichType={passdoc}&Mode={mode}")]
        public async Task<ActionResult<string>> GetDocument(string company, string passdoc, string mode)
        {
            var documents = new List<Tuple<string, int, int>>();
            int counter = 0;
            int total = 0;
            string[] directory = Directory.GetFiles("/var/www/html/imspulse/bunch-box/" + company, "*", System.IO.SearchOption.AllDirectories);
            //string[] directory = Directory.GetFiles(@"C:\Testing\", "*", System.IO.SearchOption.AllDirectories);       
            foreach (string f in directory)
            {
                total++;
                FileFormatInfo info = FileFormatUtil.DetectFileFormat(f);
                if(info.IsEncrypted == true)
                {
                    //Count All encrypted documents
                    counter ++;
                }              
            }

            documents.Add(new Tuple<string, int, int>("Documents Protected", counter, total));
            string json = JsonConvert.SerializeObject(new
            {
                documents
            });

            return json;
        }

        [HttpGet("DocumnetAll={company}")]
        public async Task<ActionResult<string>> GetAll(string company, string passdoc, string mode)
        {
            var documents = new List<Tuple<string, int, int, int>>();
            int operations = 0;
            int resources = 0;
            int qms = 0;
            DirectoryInfo op = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/Operations");
            DirectoryInfo re = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/Resources");
            DirectoryInfo q = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/QMS");
            FileInfo[] directoryre = re.GetFiles("*", System.IO.SearchOption.AllDirectories);
            FileInfo[] directoryop = re.GetFiles("*", System.IO.SearchOption.AllDirectories);
            FileInfo[] directoryqms = re.GetFiles("*", System.IO.SearchOption.AllDirectories);
            //string[] directory = Directory.GetFiles(@"C:\Testing\", "*", System.IO.SearchOption.AllDirectories);       
            foreach (FileInfo f in directoryre)
            {
                resources++;
            }
            foreach (FileInfo f in directoryqms)
            {
                qms++;
            }
            foreach (FileInfo f in directoryop)
            {
                operations++;
            }

            documents.Add(new Tuple<string, int, int, int>("RE QMS OP",resources, qms, operations));
            string json = JsonConvert.SerializeObject(new
            {
                documents
            });

            return json;
        }


    }
}
