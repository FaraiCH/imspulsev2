
using docmaster.Areas.Identity.Data;
using docmaster.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.DocumentEditor;
using Syncfusion.EJ2.FileManager;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;
using System.Diagnostics;

namespace docmaster.Controllers
{
    public class HomeController : Controller
    {

        UserManager<docmasterUser> _userManager;
        public PhysicalFileProvider operation;
        public string basePath = "/var/www/html/imspulse/bunch-box";
        //public string basePath = "C:/Testing";
        string root = @"wwwroot";

        public HomeController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment, UserManager<docmasterUser> userManager)
        {
            _userManager = userManager;
            this.operation = new PhysicalFileProvider();
            this.operation.RootFolder(this.basePath);
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public object FileOperations([FromBody] FileManagerDirectoryContent args)
        {
            var fullPath = this.basePath.Replace('\\', '/') + args.Path;
            if (args.Action == "delete" || args.Action == "rename")
            {
                if ((args.TargetPath == null) && (args.Path == ""))
                {
                    FileManagerResponse response = new FileManagerResponse();
                    response.Error = new ErrorDetails { Code = "401", Message = "Restricted to modify the root folder." };
                    return this.operation.ToCamelCase(response);
                }
            }
            
      
            
            switch (args.Action)
            {
                case "read":
                    // reads the file(s) or folder(s) from the given path.
                    return this.operation.ToCamelCase(this.operation.GetFiles(args.Path, args.ShowHiddenItems));
                case "delete":
                    // deletes the selected file(s) or folder(s) from the given path.
                    return this.operation.ToCamelCase(this.operation.Delete(args.Path, args.Names));
                case "copy":
                    // copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "move":
                    // cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.operation.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData));
                case "details":
                    // gets the details of the selected file(s) or folder(s).
                    return this.operation.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                case "create":
                    // creates a new folder in a given path.
                    return this.operation.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // gets the list of file(s) or folder(s) from a given path based on the searched key string.
                    return this.operation.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive));
                case "rename":
                    // renames a file or folder.
                    return this.operation.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName));

          
             
            }
            return null;

           
        }

        // uploads the file(s) into a specified path
        public async Task<IActionResult> Upload(string path, IList<IFormFile> uploadFiles, string action)
        {
          
   
            FileManagerResponse uploadResponse;
            var user = await _userManager.GetUserAsync(this.User);
            double fCount = GetDirectorySize(this.basePath + "/" + user.Company);
            if (!this.User.IsInRole("Master"))
            {
                if (this.User.IsInRole("Basic"))
                {
                   
                    if (fCount > 50)
                    {
                        Response.Clear();
                        Response.ContentType = "application/json; charset=utf-8";
                        Response.StatusCode = Convert.ToInt32("1111");
                        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "You have reached the maximum uploads allowed on Basic Subscription. Upgrade plan";
                    }
                    else
                    {
                        uploadResponse = operation.Upload(path, uploadFiles, action, null);
                    }

             
                }
                else if (this.User.IsInRole("Premium"))
                {
                    if (fCount > 350)
                    {
                        Response.Clear();
                        Response.ContentType = "application/json; charset=utf-8";
                        Response.StatusCode = Convert.ToInt32("1111");
                        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "You have reached the maximum uploads allowed on Premium Subscription. Upgrade plan";
                    }
                    else
                    {
                        uploadResponse = operation.Upload(path, uploadFiles, action, null);
                    }
                  
                }
                else if (this.User.IsInRole("Ultimate"))
                {
                    if (fCount > 1000)
                    {
                        Response.Clear();
                        Response.ContentType = "application/json; charset=utf-8";
                        Response.StatusCode = Convert.ToInt32("1111");
                        Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "You have reached your limit";
                    }
                    else
                    {
                        uploadResponse = operation.Upload(path, uploadFiles, action, null);
                    }
                }
                else
                {

                    Response.Clear();
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.StatusCode = Convert.ToInt32("1111");
                    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "You do not have permissions to upload anything.";

                }
            }
            else
            {
                uploadResponse = operation.Upload(path, uploadFiles, action, null);
                if (uploadResponse.Error != null)
                {
                    Response.Clear();
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.StatusCode = Convert.ToInt32(uploadResponse.Error.Code);
                    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = uploadResponse.Error.Message;
                }
            }
        
            return Content("");
        }

        // downloads the selected file(s) and folder(s)
        public IActionResult Download(string downloadInput)
        {
            FileManagerDirectoryContent args = JsonConvert.DeserializeObject<FileManagerDirectoryContent>(downloadInput);
            return operation.Download(args.Path, args.Names, args.Data);
        }

        // gets the image(s) from the given path
        public IActionResult GetImage(FileManagerDirectoryContent args)
        {
            return this.operation.GetImage(args.Path, args.Id, false, null, null);
        }

        private double GetDirectorySize(string path)
        {


            DirectoryInfo info = new DirectoryInfo(path);
            long size = 0;
            foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                size += new FileInfo(file).Length;
            }
            return Math.Round((double)size / (double)(1024 * 1024), 2);
        }


     
        [AcceptVerbs("Post")]
        public string Import(IFormCollection data)
        {
            return "Hello" + data.Files.Count;
        }

        [HttpPost]
        public IActionResult Demo2(string fullName)
        {
            if (fullName == null)
                return null;
    
            int index = fullName.LastIndexOf('.');
            string type = index > -1 && index < fullName.Length - 1 ?
            fullName.Substring(index) : ".docx";
            FileStream fileStreamPath = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = WordDocument.Load(fileStreamPath, GetFormatType(type.ToLower()));
            string sfdt = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return new JsonResult(sfdt);
        }

        public IActionResult Demo(string fullName, string exportedDocument)
        {
     
            return new JsonResult(fullName);
        }
        internal static FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return FormatType.Docx;
                case ".dot":
                case ".doc":
                    return FormatType.Doc;
                case ".rtf":
                    return FormatType.Rtf;
                case ".txt":
                    return FormatType.Txt;
                case ".xml":
                    return FormatType.WordML;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }


    }

    public class Datam
    {
        public string datapack { get; set; }
    }
}