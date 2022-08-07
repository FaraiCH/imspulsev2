using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;

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

            if (path.Contains(".doc"))
            {

                //Opens an existing document from stream through constructor of WordDocument class
                FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(fileStreamPath, FormatType.Automatic);
                //Encrypts the Word document with a password
                document.EncryptDocument(password);
                //Saves the Word document to MemoryStream
                FileStream outputStream = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                document.Save(outputStream, FormatType.Docx);
                //Closes the document
                document.Close();

                ViewData["Message"] = path;
            }
            else if(path.Contains(".xls"))
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2013;
                    FileStream fileStreamPath = new FileStream("/var/www/html/imspulse/bunch-box" + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    IWorkbook workbook = application.Workbooks.Open(fileStreamPath);

  
                    //Set the workbook as read-only
                    workbook.Protect(true, true, password);

                    //Saving the workbook as stream
                    workbook.SaveAs(fileStreamPath);

                    ViewData["Message"] = path;
                }
               
            }
            else
            {
                ViewData["Message"] = "File Cannot be encrypted";
            }
        }

    }
}
