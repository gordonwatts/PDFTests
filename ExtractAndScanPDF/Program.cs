﻿using PdfSharp.Pdf;
using PdfSharp.Internal;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf.Advanced;
using System.IO;
using System.Drawing;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing.Imaging;
using ZXing;

namespace ExtractAndScanPDF
{
    class Program
    {
        /// <summary>
        /// Attempt to extract images from a PDF from our big copy machine, and look for the
        /// QR code on it.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            foreach (var f in new string[] { "Scan300dpi.pdf", "Scan600dpi.pdf" })
            {
                Console.WriteLine("Scanning file {0}.", f);
                foreach (var image in ScanDocument(f))
                {
                    ScanImageForQRCode(image);
                }
            }
        }

        /// <summary>
        /// Scan a single image for a QR code and print out the results.
        /// </summary>
        /// <param name="image"></param>
        private static void ScanImageForQRCode(string imageFile)
        {
            var orig = Bitmap.FromFile(imageFile) as Bitmap;

            const double fraction = 0.30;
            var crop = CropImage(orig, fraction);
            {
                crop.Save("bogus.jpg", ImageFormat.Jpeg);
            }

            ScanSingleImageByMethod(imageFile, crop, b => QRUseQRLib(b));
            ScanSingleImageByMethod(imageFile, crop, b => QRUseZXing(b));
        }

        /// <summary>
        /// Wrapper to measure the performance of a QR scanner guy
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="orig"></param>
        /// <param name="scanner"></param>
        private static void ScanSingleImageByMethod(string imageFile, Bitmap orig, Func<Bitmap,string> scanner)
        {
            var start = DateTime.Now;
            string result = "";
            try
            {
                result = scanner(orig);
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            var end = DateTime.Now;
            Console.WriteLine("  {0} ({2}x{3}): {1} ({4} sec)", imageFile, result, orig.Width, orig.Height, (end - start).ToString("ss\\.ff"));
        }

        /// <summary>
        /// Uses the QR code library to do a scan.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string QRUseQRLib(Bitmap orig)
        {
            var dc = new QRCodeDecoder();
            var img = new QRCodeBitmapImage(orig);
            return dc.Decode(img);
        }

        /// <summary>
        /// Use the ZXing library to do the scanning
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static string QRUseZXing(Bitmap b)
        {
            var dc = new BarcodeReader() { Options = new ZXing.Common.DecodingOptions() { TryHarder = true } };
            var result = dc.Decode(b);
            if (result == null)
            {
                return "failed";
            }
            else
            {
                return result.Text;
            }
        }

        /// <summary>
        /// Crop an image by some fraction
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="fraction"></param>
        /// <returns></returns>
        private static Bitmap CropImage(Bitmap orig, double fraction)
        {
            var cWidth = (int)(orig.Width * fraction);
            var cHeight = (int)(orig.Height * fraction);

            var cRect = new Rectangle(orig.Width - cWidth, 0, cWidth, cHeight);

            return orig.Clone(cRect, orig.PixelFormat);
        }

        /// <summary>
        /// Extract the images.
        /// </summary>
        /// <param name="fname"></param>
        private static IEnumerable<string> ScanDocument(string fname)
        {
            var doc = PdfReader.Open(fname);
            foreach (var page in doc.Pages.Cast<PdfPage>())
            {
                var resources = page.Elements.GetDictionary("/Resources");
                if (resources != null)
                {
                    var xObjects = resources.Elements.GetDictionary("/XObject");
                    if (xObjects != null)
                    {
                        var items = xObjects.Elements.Values;
                        foreach (var item in items)
                        {
                            var reference = item as PdfReference;
                            if (reference != null)
                            {
                                var xObject = reference.Value as PdfDictionary;
                                if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                {
                                    yield return ExportImage(xObject);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given a reference to a PDF image, export it.
        /// </summary>
        /// <param name="xObject"></param>
        /// <returns></returns>
        private static string ExportImage(PdfDictionary image)
        {
            string filter = image.Elements.GetName("/Filter");
            if (filter == "/DCTDecode")
            {
                return ExtractJpegImage(image);
            }
            else
            {
                throw new NotImplementedException(string.Format("Can't go after {0}", filter));
            }
        }

        static int g_imageIndex = 0;

        /// <summary>
        /// Do a jpeg like image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static string ExtractJpegImage(PdfDictionary image)
        {
            byte[] stream = image.Stream.Value;
            var fname = string.Format("Image{0}.jpg", g_imageIndex++);
            using (var fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(stream);
                    bw.Close();
                }
            }
            return fname;
        }
    }
}
