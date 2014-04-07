using MessagingToolkit.QRCode.Codec;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFAddQRCode
{
    class Program
    {
        /// <summary>
        /// Create a bit map of a QR code and add it to a PDF
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var doc = PdfReader.Open("testdoc.pdf");

            Console.WriteLine("Found {0} pages.", doc.PageCount);

            var guid = Guid.NewGuid();
            int i_page = 1;
            foreach (var page in doc.Pages.Cast<PdfPage>())
            {
                var gfc = XGraphics.FromPdfPage(page);
                var i = GenerateQRCode(guid, i_page);
                gfc.DrawImage(i, page.Width.Point - i.PointWidth, 0);
                i_page++;
            }

            doc.Save("output.pdf");
        }

        /// <summary>
        /// Generate our QR code
        /// </summary>
        /// <returns></returns>
        static XImage GenerateQRCode(Guid guid, int page)
        {
            string data = string.Format("{0} P{1}", guid, page);

            var encoder = new QRCodeEncoder();
            var bitm = encoder.Encode(data);
            var ximg = XImage.FromGdiPlusImage(bitm);
            return ximg;
        }
    }
}
