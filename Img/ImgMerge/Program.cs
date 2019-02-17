using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImgMerge
{
    static class Program
    {
        static void Main()
        {
            Bitmap aBitmap = new Bitmap(1, 1);

            bool mBool = false;
            if (!Directory.Exists("./newpic-merge"))
                Directory.CreateDirectory("./newpic-merge");

            foreach (var itemFile in Directory.GetFiles("pics", "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine(itemFile);

                if (mBool)
                {
                    var bBitmap = new Bitmap(itemFile);
                    Bitmap img = new Bitmap(aBitmap.Width + bBitmap.Width, Math.Max(aBitmap.Height, bBitmap.Height),
                        Graphics.FromImage(aBitmap));
                    using (var g0 = Graphics.FromImage(img))
                    {
                        g0.DrawImage(aBitmap, new Point(0, 0));
                        g0.DrawImage(bBitmap, new Point(aBitmap.Width - 1, 0));
                        g0.Save();
                    }

                    img.Save("./newpic-merge/" + itemFile.Replace("pics\\", "").Replace(".", "-") + "-1.png", ImageFormat.Png);
                    mBool = false;
                }
                else
                {
                    aBitmap = new Bitmap(itemFile);
                    mBool = true;
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    
    }
}
