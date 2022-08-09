using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using Syncfusion.Presentation;
using Syncfusion.XlsIO;
using IShape = Syncfusion.Presentation.IShape;

namespace docmaster.Areas.Identity.Pages.Account.Manage
{
    public class ManagerModel : PageModel
    {
        private readonly UserManager<docmasterUser> _userManager;
        private readonly SignInManager<docmasterUser> _signInManager;

        public ManagerModel(
         UserManager<docmasterUser> userManager,
         SignInManager<docmasterUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
        public void OnPost(string path, string password)
        {
            try
            {
                if (path.Contains(".doc"))
                {

                    //Opens an existing document from stream through constructor of WordDocument class
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    WordDocument document = new WordDocument(fileStreamPath, Syncfusion.DocIO.FormatType.Automatic);
                    //Encrypts the Word document with a password
                    document.EncryptDocument(password);
                    //Saves the Word document to MemoryStream
                    FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    document.Save(outputStream, Syncfusion.DocIO.FormatType.Docx);
                    //Closes the document
                    document.Close();

                    ViewData["Message"] = path;
                }
                else if (path.Contains(".xls"))
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;

                        FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        IWorkbook workbook = application.Workbooks.Open(fileStreamPath);

                        //Encrypt the workbook with password
                        workbook.PasswordToOpen = password;

                        //Set the password to modify the workbook
                        workbook.SetWriteProtectionPassword("modify_password");

                        //Set the workbook as read-only
                        workbook.ReadOnlyRecommended = true;

                        //Saving the workbook as stream
                        FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        workbook.SaveAs(outputStream);
                        workbook.Close();
                        ViewData["Message"] = path;
                    }

                }
                else if (path.Contains(".ppt"))
                {
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    using (IPresentation presentation = Presentation.Open(fileStreamPath))
                    {
                        //Protects the file with password.
                        presentation.Encrypt(password);

                        //Save the PowerPoint Presentation as stream.

                        using (FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            presentation.Save(outputStream);
                        }

                        ViewData["Message"] = path;
                    }

                }
                else if (path.Contains(".pdf"))
                {
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    PdfLoadedDocument document = new PdfLoadedDocument(fileStreamPath, true);

                    //PDF document security 

                    PdfSecurity security = document.Security;

                    //Specifies encryption key size, algorithm and permission. 

                    security.KeySize = PdfEncryptionKeySize.Key256Bit;

                    security.Algorithm = PdfEncryptionAlgorithm.AES;

                    //Provide owner and user password.

                    security.UserPassword = password;

                    //Save the document into stream.

                    FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                    document.Save(outputStream);

                    document.Close(true);
                }
                else
                {
                    ViewData["Message"] = "File Does Not Support Encrypting.";
                }
            }
            catch (Exception ex)
            {
                if (path.Contains(".doc"))
                {

                    //Opens an existing document from stream through constructor of WordDocument class
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    WordDocument document = new WordDocument(fileStreamPath, Syncfusion.DocIO.FormatType.Automatic, password);
                    //Encrypts the Word document with a password
                    document.RemoveEncryption();
                    //Saves the Word document to MemoryStream
                    FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    document.Save(outputStream, Syncfusion.DocIO.FormatType.Docx);
                    //Closes the document
                    document.Close();

                    ViewData["Message"] = path;
                }
                else if (path.Contains(".xls"))
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;

                        FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        IWorkbook workbook = application.Workbooks.Open(fileStreamPath, ExcelParseOptions.Default, true, password);

                        //Encrypt the workbook with password
                        workbook.PasswordToOpen = password;

                        //Set the password to modify the workbook
                        workbook.SetWriteProtectionPassword("modify_password");

                        //Set the workbook as read-only
                        workbook.ReadOnlyRecommended = true;

                        //Saving the workbook as stream
                        FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        workbook.SaveAs(outputStream);
                        workbook.Close();
                        ViewData["Message"] = path;
                    }

                }
                else if (path.Contains(".ppt"))
                {
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    using (IPresentation presentation = Presentation.Open(fileStreamPath, password))
                    {
                        //Protects the file with password.
                        presentation.RemoveEncryption();

                        //Save the PowerPoint Presentation as stream.

                        using (FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            presentation.Save(outputStream);
                        }

                        ViewData["Message"] = path;
                    }

                }
                else
                {
                    ViewData["Message"] = ex.Message;
                }

               
            }

        }

    }
}
