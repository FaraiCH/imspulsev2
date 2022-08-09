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
        public void OnPost(string path, string password, string state)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string basepath = "/var/www/html/imspulse/bunch-box";
            //string basepath = "C:/Testing";
            try
            {
                if (state == "encrypt")
                {
                    if (path.Contains(".doc"))
                    {
                        Aspose.Words.Saving.OoxmlSaveOptions opt = new Aspose.Words.Saving.OoxmlSaveOptions(Aspose.Words.SaveFormat.Docx);

                        opt.Compliance = Aspose.Words.Saving.OoxmlCompliance.Iso29500_2008_Transitional;

                        opt.Password = password;
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = password };
                        Aspose.Words.Document docu = new Aspose.Words.Document(basepath + path, getum12);

                        docu.Save(basepath + path, opt);
                    }
                    else if (path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getum3 = new Aspose.Cells.LoadOptions { Password = password };
                        Workbook workt = new Workbook(basepath + path, getum3);
                        workt.Settings.Password = password;
                        workt.Save(basepath + path);
                    }
                }
                else
                {
                    if (path.Contains(".doc"))
                    {
                        Aspose.Words.LoadOptions getum12 = new Aspose.Words.LoadOptions { Password = password };
                        Aspose.Words.Document docu = new Aspose.Words.Document(basepath + path, getum12);
                        docu.Unprotect();
                        docu.Save(basepath + path);
                    }
                    if (path.Contains(".xls"))
                    {
                        Aspose.Cells.LoadOptions getums = new Aspose.Cells.LoadOptions { Password = password };
                        Workbook worsk = new Workbook(basepath + path, getums);

                        worsk.Settings.Password = null;

                        worsk.Save(basepath + path);
                    }
                }
            }
            catch (Exception ex)
            {

                ViewData["Message"] = ex.Message;
            }

        }

    }
}
