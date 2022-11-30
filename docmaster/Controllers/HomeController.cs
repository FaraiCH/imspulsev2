using Aspose.Words;
using Aspose.Words.Drawing;
using docmaster.Areas.Identity.Data;
using docmaster.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using Syncfusion.EJ2.DocumentEditor;
using Syncfusion.EJ2.FileManager.Base;
using Syncfusion.EJ2.FileManager.PhysicalFileProvider;
using Syncfusion.EJ2.PdfViewer;
using Syncfusion.EJ2.Spreadsheet;
using Syncfusion.Presentation;
using Syncfusion.XlsIO;
using System.Diagnostics;

namespace docmaster.Controllers
{
    [EnableCors("MyPolicy")]
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
        /// <summary>
        /// Compare documents
        /// Choose what do to with old document (Absolete or override)
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Demo2(string fullName)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            var fullpath = "/var/www/html/imspulse/bunch-box" + fullName;


            if (fullpath == null)
                return null;
            try
            {
                if (fullpath.Contains(".doc"))
                {
                    Aspose.Words.Document docu = new Aspose.Words.Document(fullpath);

                    int index = fullpath.LastIndexOf('.');
                    string type = index > -1 && index < fullpath.Length - 1 ?
                    fullpath.Substring(index) : ".docx";
                    FileStream fileStreamPath = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    WordDocument document = WordDocument.Load(fileStreamPath, GetFormatType(type.ToLower()));
                    string sfdt = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    fileStreamPath.Close();
                    return new JsonResult(sfdt);
                }
                if (fullpath.Contains(".xls"))
                {

                    ExcelEngine excelEngine = new ExcelEngine();
                    IWorkbook workbook;
                    FileStream fs = System.IO.File.Open(fullpath, FileMode.Open); // converting excel file to stream 
                    workbook = excelEngine.Excel.Workbooks.Open(fs, ExcelOpenType.Automatic); // coverting stream to XlsIO workbook 
                    MemoryStream outputStream = new MemoryStream();
                    workbook.SaveAs(outputStream);
                    IFormFile formFile = new FormFile(outputStream, 0, outputStream.Length, "", "Temp.xlsx"); // converting MemoryStream to IFormFile 
                    OpenRequest open = new OpenRequest();
                    open.File = formFile;
                    workbook.Close();
                    fs.Close();
                    return new JsonResult(Syncfusion.EJ2.Spreadsheet.Workbook.Open(open)); // Return Spreadsheet readable data 
                }

                return new JsonResult("");
         
            }
            catch (Exception ex)
            {

                return new JsonResult("Error Message: " + ex.Message);
            }
          
        }

        public string PDFView(string fullName)
        {
            var docBytes = System.IO.File.ReadAllBytes("/var/www/html/imspulse/bunch-box" + fullName);
            string docBase64 = "data:application/pdf;base64," + Convert.ToBase64String(docBytes);
            return (docBase64);
        }
        public IActionResult Opened(IFormCollection openRequest)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            OpenRequest open = new OpenRequest();
            open.File = openRequest.Files[0];
            return Content(Syncfusion.EJ2.Spreadsheet.Workbook.Open(open));
        }

        [HttpPost]
        public async Task<IActionResult> Demo([FromBody] PayloadModel payload)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            try
            {
                var user =  await _userManager.GetUserAsync(this.User);
                var fullpath = "/var/www/html/imspulse/bunch-box" + payload.path;
                if (fullpath.Contains(".doc"))
                {
                    //Get Old document using path and convert to JSON
                    Aspose.Words.Document docu = new Aspose.Words.Document(fullpath);

                    //Get Document Text
                    var myDoc = docu.ToString(SaveFormat.Text);

                    Stream document = WordDocument.Save(payload.fullName, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
                    System.IO.File.Delete(payload.path);
                    FileStream file = new FileStream(fullpath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                   
                    document.CopyTo(file);
                    file.Close();
                    document.Close();

                    var original = Path.GetFileName(fullpath);

                    Aspose.Words.Document newdoc = new Aspose.Words.Document(fullpath);
                    var mynewDoc = newdoc.ToString(SaveFormat.Text);
                    

                    //Check to see if document content was changed
                    if (mynewDoc != myDoc)
                    {
                       
                        List<string> diff;
                        IEnumerable<string> set1 = mynewDoc.Split(' ').Distinct();
                        IEnumerable<string> set2 = myDoc.Split(' ').Distinct();
                        string add = string.Empty;
                       

                        if (set2.Count() > set1.Count())
                        {
                            diff = set2.Except(set1).ToList();
                            add = "Content was changed or removed from this document. See Details: ";
                        }
                        else
                        {
                            diff = set1.Except(set2).ToList();
                            add = "Content was added to this document. See Details: ";
                        }

                      
                        var result = string.Join(" ", diff);

                        if (!Directory.Exists(this.basePath + "/" + user.Company + "/Absolete/"))
                        {
                            Directory.CreateDirectory(this.basePath + "/" + user.Company + "/Absolete/");
                        }

                        string watermarkText = "OBSOLETE";

                        // Create a watermark shape. This will be a WordArt shape.
                        // You are free to try other shape types as watermarks.
                        Aspose.Words.Drawing.Shape watermark = new Aspose.Words.Drawing.Shape(docu, ShapeType.TextPlainText);

                        // Set up the text of the watermark.
                        watermark.TextPath.Text = watermarkText;
                        watermark.TextPath.FontFamily = "Arial Black";
                        watermark.TextPath.Bold = true;
                        watermark.Width = 600;
                        watermark.Height = 100;

                        // Text will be directed from the bottom-left to the top-right corner.
                        watermark.Rotation = -40;

                        // Remove the following two lines if you need a solid black text.
                        watermark.Fill.Color = System.Drawing.Color.Red;
                        // Try LightGray to get more Word-style watermark
                        watermark.StrokeColor = System.Drawing.Color.Red;
                        //// Try LightGray to get more Word-style watermark
                        // Place the watermark in the page center.
                        watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                        watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                        watermark.WrapType = WrapType.None;
                        watermark.VerticalAlignment = VerticalAlignment.Center;
                        watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;

                        // Create a new paragraph and append the watermark to this paragraph.
                        Paragraph watermarkPara = new Paragraph(docu);
                        watermarkPara.AppendChild(watermark);

                        // Insert the watermark into all headers of each document section.
                        foreach (Section sect in docu.Sections)
                        {
                            // There could be up to three different headers in each section, since we want
                            // the watermark to appear on all pages, insert into all headers.
                            insertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                            insertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                            insertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
                        }
                        docu.Protect(Aspose.Words.ProtectionType.ReadOnly, "@Paradice1");

                        docu.Save(this.basePath + "/" + user.Company + "/Absolete/" + original);

                        using (var conn = new MySqlConnection("Server=92.205.25.31; Database=imspulse; Uid=manny; Pwd=@Paradice1;"))
                        {
                            await conn.OpenAsync();

                            // Insert some data
                            using (var cmd = new MySqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.CommandText = "INSERT INTO farai_document_revisions (document_name, content, company, user_name, email) VALUES (@document_name, @content, @company, @user_name, @email)";
                                cmd.Parameters.AddWithValue("@document_name", fullpath);
                                cmd.Parameters.AddWithValue("@content", result);
                                cmd.Parameters.AddWithValue("@company", user.Company);
                                cmd.Parameters.AddWithValue("@user_name", user.UserName);
                                cmd.Parameters.AddWithValue("@email", user.Email);
                                await cmd.ExecuteNonQueryAsync();
                            }

                        }
                        //return new JsonResult(mynewDoc);
                        return new JsonResult(add + result);
                    }   

                   
                }

                return new JsonResult("Document Successfully Saved!");
            }

            catch (Exception e)
            {
                return new JsonResult(e.ToString());
            }
          
        }

        static void insertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            HeaderFooter header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }

            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));


        }

        public IActionResult SaveExcel(SaveSettings saveSettings)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box");
            try
            {
                var filepaths = "/var/www/html/imspulse/bunch-box" + this.Request.Form["path"];
                ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                System.IO.File.Delete(filepaths);
                // Convert Spreadsheet data as Stream 
                Stream fileStream = Syncfusion.EJ2.Spreadsheet.Workbook.Save<Stream>(saveSettings);
                IWorkbook workbook = application.Workbooks.Open(fileStream);
                var filePath = filepaths;
                FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                workbook.SaveAs(outputStream);
                filepaths = string.Empty;
                fileStream.Close();
                workbook.Close();
                outputStream.Dispose();

                return new JsonResult("File saved successfully");
            }
            catch (Exception ex)
            {

                return new JsonResult(ex.Message);
            }
        
                     

        }
    
        public async Task<IActionResult> Protect([FromBody] ProtectModel payload)
        {
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box/");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string basepath = "/var/www/html/imspulse/bunch-box";
            var user = await _userManager.GetUserAsync(this.User);
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
                    using (var conn = new MySqlConnection("Server=92.205.25.31; Database=imspulse; Uid=manny; Pwd=@Paradice1;"))
                    {
                        await conn.OpenAsync();

                        // Insert some data
                        using (var cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "INSERT INTO farai_document_passwords (document_path, password, company, user_name) VALUES (@document_path, @password, @company, @user_name)";
                            cmd.Parameters.AddWithValue("@document_path", payload.path);
                            cmd.Parameters.AddWithValue("@password", payload.fullName);
                            cmd.Parameters.AddWithValue("@company", user.Company);
                            cmd.Parameters.AddWithValue("@user_name", user.UserName);
                            await cmd.ExecuteNonQueryAsync();
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

        public IActionResult Revision([FromBody] ProtectModel payload)
        {
            // Open an existing document.
            string path = "/var/www/html/imspulse/bunch-box/" + payload.path + "/" + payload.fullName;
            Document doc = new Document(path);
            string fieldValue = String.Empty;
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (string fieldName in doc.MailMerge.GetFieldNames())
            {

                builder.StartBookmark(fieldName);

                builder.MoveToMergeField(fieldName, true, false);

                builder.EndBookmark(fieldName);

            }

            // Trim trailing and leading whitespaces mail merge values
            doc.MailMerge.TrimWhitespaces = false;

            // Fill the fields in the document with user data.
            doc.MailMerge.Execute(
                new string[] { "Revision" },
                new object[] { Path.GetFileNameWithoutExtension(path) }
                );

            foreach (Bookmark bookmark in doc.Range.Bookmarks)
            {
                if(bookmark.Name == "Revision")
                {
                    fieldValue = bookmark.Text;
                }               
            }
            doc.Range.Replace(fieldValue, Path.GetFileNameWithoutExtension(path));
            doc.Save(path);
            
            
            return new JsonResult("Revision Successful");
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