
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System;
using System.Drawing;
using System.Linq;
namespace DecodeQRCodeFromImage
{
    class Program
    {
        /// <summary>
        /// Try to decode an image that exists in a graphics file that
        /// is a perfect "picture" of one of our exams.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            foreach (var f in new string[] { @"TestCopy.png" }.Concat(Enumerable.Range(1, 8).Select(i => string.Format("Page{0}.jpg", i))))
            {
                ScanImage(f);
            }
        }

        private static void ScanImage(string f)
        {
            try
            {
                var x = Bitmap.FromFile(f) as Bitmap;
                if (x == null)
                {
                    Console.WriteLine("No image");
                }
                else
                {
                    var dc = new QRCodeDecoder();
                    var img = new QRCodeBitmapImage(x);
                    var result = dc.Decode(img);
                    Console.WriteLine("{0}: {1}", f, result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: failed: {1} - {2}", f, e.Message, e.StackTrace);
            }
        }
    }
}
