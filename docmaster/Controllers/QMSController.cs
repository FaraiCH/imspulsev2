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
    public class QMSController : ControllerBase
    {

        UserManager<docmasterUser> _userManager;

        [HttpGet("DocumnetAll={company}")]
        public async Task<ActionResult<string>> GetAll(string company)
        {
            var documents = new List<Tuple<string, int, int, int>>();
            int operations = 0;
            int resources = 0;
            int qms = 0;
            try
            {
                if (!Directory.Exists("/var/www/html/imspulse/bunch-box/" + company + "/Operations"))
                {
                    DirectoryInfo op = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/Operations");
                    FileInfo[] directoryop = op.GetFiles("*", System.IO.SearchOption.AllDirectories);
                    foreach (FileInfo f in directoryop)
                    {
                        operations++;
                    }

                }
                if (!Directory.Exists("/var/www/html/imspulse/bunch-box/" + company + "/Resources"))
                {
                    DirectoryInfo re = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/Resources");
                    FileInfo[] directoryre = re.GetFiles("*", System.IO.SearchOption.AllDirectories);
                    foreach (FileInfo f in directoryre)
                    {
                        resources++;
                    }
                }
                if (!Directory.Exists("/var/www/html/imspulse/bunch-box/" + company + "/QMS"))
                {
                    DirectoryInfo q = new DirectoryInfo("/var/www/html/imspulse/bunch-box/" + company + "/QMS");
                    FileInfo[] directoryqms = q.GetFiles("*", System.IO.SearchOption.AllDirectories);
                    foreach (FileInfo f in directoryqms)
                    {
                        qms++;
                    }
                }             
             
                //string[] directory = Directory.GetFiles(@"C:\Testing\", "*", System.IO.SearchOption.AllDirectories);       
                        
            }
            catch (Exception)
            {

                operations = 0;
                resources = 0;
                qms = 0;
            }
          

            documents.Add(new Tuple<string, int, int, int>("RE QMS OP", resources, qms, operations));
            string json = JsonConvert.SerializeObject(new
            {
                documents
            });

            return json;
        }
    }
}
