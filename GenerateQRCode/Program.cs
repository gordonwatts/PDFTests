using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateQRCode
{
    class Program
    {
        /// <summary>
        /// Generate a QR code with the proper info in it.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var guid = Guid.NewGuid();
            int page = 10;
            string data = string.Format("{0} P{1}", guid, page);

            var encoder = new QRCodeEncoder();
            var map = encoder.Encode(data);
            if (map == null)
                Console.WriteLine("Couldn't encode it");

            map.Save("junk.png");
        }
    }
}
