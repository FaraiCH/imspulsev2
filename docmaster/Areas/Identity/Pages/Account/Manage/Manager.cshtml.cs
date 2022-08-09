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
using System.Diagnostics;
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
            Exec("sudo chmod 775 -R /var/www/html/imspulse/bunch-box/");
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
                    else if (path.Contains(".ppt"))
                    {
                        FileStream fileStreamPath = new FileStream(basepath + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (IPresentation presentation = Presentation.Open(fileStreamPath))
                        {
                            //Protects the file with password.
                            presentation.Encrypt(password);

                            //Save the PowerPoint Presentation as stream.

                            using (FileStream outputStream = new FileStream(basepath + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                presentation.Save(outputStream);
                            }
                        }

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
                    else if (path.Contains(".ppt"))
                    {
                        FileStream fileStreamPath = new FileStream(basepath + path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (IPresentation presentation = Presentation.Open(fileStreamPath, password))
                        {
                            //Protects the file with password.
                            presentation.RemoveEncryption();

                            //Save the PowerPoint Presentation as stream.

                            using (FileStream outputStream = new FileStream(basepath + path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                presentation.Save(outputStream);
                            }

                        }

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
