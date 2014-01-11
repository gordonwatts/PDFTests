using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Linq;

namespace PDFTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = PdfReader.Open("testdoc.pdf");

            Console.WriteLine("Found {0} pages.", doc.PageCount);
            var font = new XFont("Verdana", 20, XFontStyle.Bold);

            foreach (var page in doc.Pages.Cast<PdfPage>())
            {
                var gfc = XGraphics.FromPdfPage(page);
                gfc.DrawString("Hello World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            }

            doc.Save("output.pdf");
        }
    }
}
