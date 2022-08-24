
using Aspose.Cells;
using docmaster.Areas.Identity.Data;
using docmaster.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.DocumentEditor;
using Syncfusion.EJ2.FileManager;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;
using Syncfusion.EJ2.Spreadsheet;
using Syncfusion.Presentation;
using Syncfusion.XlsIO;
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
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
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
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");

            FileManagerResponse uploadResponse;
            var user = await _userManager.GetUserAsync(this.User);
            double fCount = GetDirectorySize(this.basePath + "/" + user.Company);
            foreach (var item in uploadFiles)
            {
                if (item.FileName.Contains(".doc") || item.FileName.Contains(".xls") || item.FileName.Contains(".xls") || item.FileName.Contains(".csv") || item.FileName.Contains(".ppt") || item.FileName.Contains(".pdf") || item.FileName.Contains(".vsd") || item.FileName.Contains(".pub") || item.FileName.Contains(".txt"))
                {          
               
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

                }
                else
                {
                    Response.Clear();
                    Response.ContentType = "application/json; charset=utf-8";
                    Response.StatusCode = Convert.ToInt32("1111");
                    Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "The Document Manager does not support this file format.";
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

        public static void Exec(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\""
                }
            };

            process.Start();
            process.WaitForExit();
        }
        [HttpPost]
        public IActionResult Demo2(string fullName)
        {
            Exec("sudo chmod 775 -R " + fullName);
         
            if (fullName == null)
                return null;
            try
            {
                if (fullName.Contains(".doc"))
                {
                    Aspose.Words.Document docu = new Aspose.Words.Document(fullName);

                    int index = fullName.LastIndexOf('.');
                    string type = index > -1 && index < fullName.Length - 1 ?
                    fullName.Substring(index) : ".docx";
                    FileStream fileStreamPath = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    WordDocument document = WordDocument.Load(fileStreamPath, GetFormatType(type.ToLower()));
                    string sfdt = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    fileStreamPath.Close();
                    return new JsonResult(sfdt);
                }
                if (fullName.Contains(".xls"))
                {

                    ExcelEngine excelEngine = new ExcelEngine();
                    IWorkbook workbook;
                    FileStream fs = System.IO.File.Open(fullName, FileMode.Open); // converting excel file to stream 
                    workbook = excelEngine.Excel.Workbooks.Open(fs, ExcelOpenType.Automatic); // coverting stream to XlsIO workbook 
                    MemoryStream outputStream = new MemoryStream();
                    workbook.SaveAs(outputStream);
                    IFormFile formFile = new FormFile(outputStream, 0, outputStream.Length, "", ""); // converting MemoryStream to IFormFile 
                    OpenRequest open = new OpenRequest();
                    open.File = formFile;
                    fs.Close();
                    return new JsonResult(Syncfusion.EJ2.Spreadsheet.Workbook.Open(open)); // Return Spreadsheet readable data 
                }
                return new JsonResult("");
         
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message);
            }
          
        }

        [HttpPost]
        public IActionResult Demo([FromBody] PayloadModel payload)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            try
            {
                Stream document = WordDocument.Save(payload.fullName, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
                System.IO.File.Delete(payload.path);
                FileStream file = new FileStream(payload.path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                document.CopyTo(file);
                file.Close();
                document.Close();

                return new JsonResult("Document Successfully Saved!");
            }

            catch (Exception e)
            {
                return new JsonResult(e.Message);
            }
          
        }

        public IActionResult Protect([FromBody] ProtectModel payload)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box/");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string basepath = "/var/www/html/imspulse/bunch-box";
            //string basepath = "C:/Testing";
            try
            {
                if (payload.state == "encrypt")
                {
                    if (payload.path.Contains(".doc"))
                    {
                        Aspose.Words.Saving.OoxmlSaveOptions opt = new Aspose.Words.Saving.OoxmlSaveOptions(Aspose.Words.SaveFormat.Docx);

                        opt.Compliance = Aspose.Words.Saving.OoxmlCompliance.Iso29500_2008_Transitional;

                        opt.Password = payload.fullName;
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = payload.fullName };
                        Aspose.Words.Document docu = new Aspose.Words.Document(basepath + payload.path, getum12);

                        docu.Save(basepath + payload.path, opt);
        

                    }
                    else if (payload.path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getum3 = new Aspose.Cells.LoadOptions { Password = payload.fullName };
                        Aspose.Cells.Workbook workt = new Aspose.Cells.Workbook(basepath + payload.path, getum3);
                        workt.Settings.Password = payload.fullName;
                        workt.Save(basepath + payload.path);
                     
                    }
                    else if (payload.path.Contains(".ppt"))
                    {
                        FileStream fileStreamPath = new FileStream(basepath + payload.path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (IPresentation presentation = Presentation.Open(fileStreamPath))
                        {
                            //Protects the file with password.
                            presentation.Encrypt(payload.fullName);

                            //Save the PowerPoint Presentation as stream.

                            using (FileStream outputStream = new FileStream(basepath + payload.path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                presentation.Save(outputStream);
                              
                            }
                        }
                      
                    }
                    return new JsonResult("Encrypt Successful");
                }
                else
                {
                    if (payload.path.Contains(".doc"))
                    {
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = payload.fullName };
                        Aspose.Words.Document docu = new Aspose.Words.Document(basepath + payload.path, getum12);
                        docu.Unprotect();
                        docu.Save(basepath + payload.path);
                     
                    }
                    else if (payload.path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getums = new Aspose.Cells.LoadOptions { Password = payload.fullName };
                        Aspose.Cells.Workbook worsk = new Aspose.Cells.Workbook(basepath + payload.path, getums);

                        worsk.Settings.Password = null;

                        worsk.Save(basepath + payload.path);
                    
                    }
                    else if (payload.path.Contains(".ppt"))
                    {
                        FileStream fileStreamPath = new FileStream(basepath + payload.path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (IPresentation presentation = Presentation.Open(fileStreamPath, payload.fullName))
                        {
                            //Protects the file with password.
                            presentation.RemoveEncryption();

                            //Save the PowerPoint Presentation as stream.

                            using (FileStream outputStream = new FileStream(basepath + payload.path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                presentation.Save(outputStream);
                                
                            }

                        }
       
                    }
                    return new JsonResult("Decrypt Successful");
                }
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message);
            }

        }

        internal static Syncfusion.EJ2.DocumentEditor.FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Docx;
                case ".dot":
                case ".doc":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Doc;
                case ".rtf":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Rtf;
                case ".txt":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Txt;
                case ".xml":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.WordML;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }


    }

    public class CustomParams
    {
        public string fileName
        {
            get;
            set;
        }
    }


}