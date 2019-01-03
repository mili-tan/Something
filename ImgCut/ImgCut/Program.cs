using System.Drawing;
using System.Drawing.Imaging;

namespace ImgCut
{
    static class Program
    {
        static void Main(string[] args)
        {
            var sorceB = new Bitmap(args[0]);
            var b0 = CutImage(sorceB, 0, 0, 452, 1920);
            var b1 = CutImage(sorceB, 452, 0, 904, 1920);
            var b2 = CutImage(sorceB, 904, 0, 1356, 1920);

            Bitmap imgStep1 = new Bitmap(1356, 1920);
            var g0 = Graphics.FromImage(imgStep1);
                g0.DrawImage(b1, new Point(0, 0));
                g0.DrawImage(b0, new Point(452, 0));
                g0.DrawImage(b2, new Point(904, 0));
                g0.Save();
            

            var bS0 = CutImage(imgStep1, 0, 0, 1356, 640);
            var bS1 = CutImage(imgStep1, 0, 640, 1356, 1920);

            Bitmap imgStep2 = new Bitmap(1356, 1920);
            var g1 = Graphics.FromImage(imgStep2);
                g1.DrawImage(bS1, new Point(0, 0));
                g1.DrawImage(bS0, new Point(0, 1280));
                g1.Save();
            

            imgStep2.Save("new.png",ImageFormat.Png);
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
