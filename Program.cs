using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WordClockGifConverter {

    // usage: dotnet run "c:\path\to\gif\mygif.gif" "c:\path\to\result\myresult.bin"

    class Program {
        static void Main(string[] args) {
            List<byte> bytes = new List<byte>();
            var image = Image.FromFile(args[0]);
            var dimension = new FrameDimension(image.FrameDimensionsList[0]);
            var frameCount = (Int16) image.GetFrameCount(dimension);
            bytes.Add((byte) frameCount);
            bytes.Add((byte) (frameCount >> 8));
            int index = 0;
            for (int i = 0; i < frameCount; i++) {
                image.SelectActiveFrame(dimension, i);
                var frame = image.Clone() as Image;
                var delay = BitConverter.ToInt16(image.GetPropertyItem(20736).Value, index) * 10;
                index += 4;

                Bitmap bitmap = new Bitmap(frame);
                if (i == 0) {
                    bytes.Add((byte) bitmap.Width);
                    bytes.Add((byte) (bitmap.Width >> 8));
                    bytes.Add((byte) bitmap.Height);
                    bytes.Add((byte) (bitmap.Height >> 8));
                }
                bytes.Add((byte) delay);
                bytes.Add((byte) (delay >> 8));
                for (int y = 0; y < bitmap.Height; y++) {
                    for (int x = 0; x < bitmap.Width; x++) {
                        bytes.Add(bitmap.GetPixel(x, y).R);
                        bytes.Add(bitmap.GetPixel(x, y).G);
                        bytes.Add(bitmap.GetPixel(x, y).B);
                    }
                }
            }
            File.WriteAllBytes(args[1], bytes.ToArray());
            Console.WriteLine("Done!");
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn) {
            using(var ms = new MemoryStream()) {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}