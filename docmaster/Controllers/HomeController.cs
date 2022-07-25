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
            string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";

            Stream stream = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream2 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream3 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream4 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream5 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream6 = new MemoryStream(Convert.FromBase64String(LData));
            Stream stream7 = new MemoryStream(Convert.FromBase64String(LData));

            stream.Seek(0, SeekOrigin.Begin);
            stream2.Seek(0, SeekOrigin.Begin);
            stream3.Seek(0, SeekOrigin.Begin);
            stream4.Seek(0, SeekOrigin.Begin);
            stream5.Seek(0, SeekOrigin.Begin);
            stream6.Seek(0, SeekOrigin.Begin);
            stream7.Seek(0, SeekOrigin.Begin);

            new Aspose.Cells.License().SetLicense(stream);
            new Aspose.Pdf.License().SetLicense(stream2);
            new Aspose.Words.License().SetLicense(stream3);
            new Aspose.Diagram.License().SetLicense(stream4);
            new Aspose.Html.License().SetLicense(stream5);
            new Aspose.Email.License().SetLicense(stream6);
            new Aspose.Slides.License().SetLicense(stream7);

            _logger = logger;
        }

        public IActionResult Index()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Specify font formatting
            Font font = builder.Font;
            font.Size = 16;
            font.Bold = true;
            font.Color = System.Drawing.Color.Blue;
            font.Name = "Arial";
            font.Underline = Underline.Dash;

            // Specify paragraph formatting
            ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            paragraphFormat.FirstLineIndent = 8;
            paragraphFormat.Alignment = ParagraphAlignment.Justify;
            paragraphFormat.KeepTogether = true;

            builder.Writeln("A whole paragraph.");
            doc.Save(@"/var/www/html/imspulse/myone.doc");
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