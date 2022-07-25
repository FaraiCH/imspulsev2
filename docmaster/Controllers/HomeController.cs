using Aspose.Words;
using docmaster.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace docmaster.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Document doc = new Document();
            //DocumentBuilder builder = new DocumentBuilder(doc);

            //// Specify font formatting
            //Font font = builder.Font;
            //font.Size = 16;
            //font.Bold = true;
            //font.Color = System.Drawing.Color.Blue;
            //font.Name = "Arial";
            //font.Underline = Underline.Dash;

            //// Specify paragraph formatting
            //ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            //paragraphFormat.FirstLineIndent = 8;
            //paragraphFormat.Alignment = ParagraphAlignment.Justify;
            //paragraphFormat.KeepTogether = true;

            //builder.Writeln("A whole paragraph.");
            //doc.Save(@"/var/www/html/imspulse/myone.doc");
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
    }
}