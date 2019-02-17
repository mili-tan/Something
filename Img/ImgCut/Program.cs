using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImgCut
{
    static class Program
    {
        static void Main()
        {
            foreach (var itemFile in Directory.GetFiles(@"pics", "*.*", SearchOption.AllDirectories))
            {
                var sourceB = new Bitmap(itemFile);
                int wCut = sourceB.Width / 3;
                int hCut = sourceB.Height / 3;

                var b0 = CutImage(sourceB, 0, 0, wCut, sourceB.Height);
                var b1 = CutImage(sourceB, wCut, 0, wCut * 2, sourceB.Height);
                var b2 = CutImage(sourceB, wCut * 2, 0, sourceB.Width, sourceB.Height);

                Bitmap imgStep1 = new Bitmap(sourceB.Width, sourceB.Height);
                using (var g0 = Graphics.FromImage(imgStep1))
                {
                    g0.DrawImage(b1, new Point(0, 0));
                    g0.DrawImage(b0, new Point(wCut, 0));
                    g0.DrawImage(b2, new Point(wCut * 2, 0));
                    g0.Save();
                }


                var bS0 = CutImage(imgStep1, 0, 0, sourceB.Width, hCut);
                var bS1 = CutImage(imgStep1, 0, hCut, sourceB.Width, sourceB.Height);

                Bitmap imgStep2 = new Bitmap(sourceB.Width, sourceB.Height);
                using (var g1 = Graphics.FromImage(imgStep2))
                {
                    g1.DrawImage(bS1, new Point(0, 0));
                    g1.DrawImage(bS0, new Point(0, sourceB.Height - hCut));
                    g1.Save();
                }

                Console.WriteLine(itemFile);
                imgStep2.Save("./newpic/" + itemFile.Replace("pics\\","").Replace(".","-") + ".png", ImageFormat.Png);
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        public static Bitmap CutImage(Bitmap bitmap, int beginX, int beginY, int getX, int getY)
        {
            Bitmap destBitmap = new Bitmap(getX, getY); //目标图 
            Rectangle destRect = new Rectangle(0, 0, getX, getY); //矩形容器 
            Rectangle srcRect = new Rectangle(beginX, beginY, getX, getY);

            Graphics g = Graphics.FromImage(destBitmap);
            g.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            g.Save();

            return destBitmap;
        }
    }

}
