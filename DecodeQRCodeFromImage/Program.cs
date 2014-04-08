
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
#if false
                    if (x.Width > 1000)
                    {
                        x = x.GetThumbnailImage(1000, (int)(1000.0 * ((float)x.Height) / ((float)x.Width)), () => false, IntPtr.Zero) as Bitmap;
                    }
#endif
                    var dc = new QRCodeDecoder();
                    var img = new QRCodeBitmapImage(x);
                    var result = dc.Decode(img);
                    Console.WriteLine("{0} ({2}x{3}): {1}", f, result, x.Width, x.Height);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}: failed: {1}", f, e.Message);
            }
        }
    }
}
