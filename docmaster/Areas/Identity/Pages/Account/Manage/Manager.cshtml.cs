using Aspose.Cells;
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
        public void OnPost(string path, string password, string password2, string path2)
        {
            string basepath = "/var/www/html/imspulse/bunch-box";
            //string basepath = "C:/Testing";
            try
            {
                if(password2 == null)
                {
                    if (path.Contains(".doc"))
                    {
                        Aspose.Words.Saving.OoxmlSaveOptions opt = new Aspose.Words.Saving.OoxmlSaveOptions(Aspose.Words.SaveFormat.Docx);

                        opt.Compliance = Aspose.Words.Saving.OoxmlCompliance.Iso29500_2008_Transitional;

                        opt.Password = password;
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = password };
                        Aspose.Words.Document docu = new Aspose.Words.Document(path, getum12);

                        docu.Save(basepath + path, opt);
                    }
                    else if (path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getum3 = new Aspose.Cells.LoadOptions { Password = password };
                        Workbook workt = new Workbook(basepath + path, getum3);
                        workt.Settings.WriteProtection.Password = password;
                        workt.Save(basepath + path);
                    }
                }
                else
                {
                    if (path.Contains(".doc"))
                    {
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = password2 };
                        Aspose.Words.Document docu = new Aspose.Words.Document(basepath + path, getum12);
                        docu.Unprotect();
                        docu.Save(basepath + path2);
                    }
                    if (path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getums = new Aspose.Cells.LoadOptions { Password = password2 };
                        Workbook worsk = new Workbook(basepath + path, getums);

                        worsk.Settings.Password = null;

                        worsk.Save(basepath + path2);
                    }
                }

         
                //else if (path.Contains(".ppt"))
                //{
                //    FileStream fileStreamPath = new FileStream(basepath + path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //    using (IPresentation presentation = Presentation.Open(fileStreamPath))
                //    {
                //        //Protects the file with password.
                //        presentation.Encrypt(password);

                //        //Save the PowerPoint Presentation as stream.

                //        using (FileStream outputStream = new FileStream(basepath + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                //        {
                //            presentation.Save(outputStream);
                //        }

                //        ViewData["Message"] = path;
                //    }

                //}
                //else if (path.Contains(".pdf"))
                //{
                //    FileStream fileStreamPath = new FileStream(basepath + path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //    PdfLoadedDocument document = new PdfLoadedDocument(fileStreamPath, true);

                //    //PDF document security 

                //    PdfSecurity security = document.Security;

                //    //Specifies encryption key size, algorithm and permission. 

                //    security.KeySize = PdfEncryptionKeySize.Key256Bit;

                //    security.Algorithm = PdfEncryptionAlgorithm.AES;

                //    //Provide owner and user password.

                //    security.UserPassword = password;

                //    //Save the document into stream.

                //    FileStream outputStream = new FileStream(basepath + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                //    document.Save(outputStream);

                //    document.Close(true);
                //}
             
            }
            catch (Exception ex)
            {

                ViewData["Message"] = ex.Message;
            }

        }

    }
}
