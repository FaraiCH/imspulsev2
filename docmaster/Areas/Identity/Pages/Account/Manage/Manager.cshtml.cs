using Aspose.Cells;
using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
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

            //Retrieve all rows
            using (var conn = new MySqlConnection("Server=92.205.25.31; Database=imspulse; Uid=manny; Pwd=@Paradice1;"))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT * FROM farai_document_revisions", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ViewData["first"] = reader.GetString(0);
                            ViewData["second"] = reader.GetString(1);
                            ViewData["third"] = reader.GetString(2);
                            ViewData["fourth"] = reader.GetString(3);
                        }
                    }
                }
                                             
            }

            return Page();
        }

        public void OnPost()
        {
  

        }

    }
}
