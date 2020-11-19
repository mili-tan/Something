using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace mOpenCV.TrafficLight
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Mat src = new Mat(@"./Y.jpg");
            src = src.Resize(new Size(src.Width / 4, src.Height / 4));
            Cv2.ImShow("src", src);
            var sp = new Stopwatch();
            sp.Start();

            var h1 = SaturationGain(src, 255);
            Cv2.ImShow("Saturation++", h1);

            Mat m1 = new Mat();
            Cv2.MedianBlur(h1, m1, 11); //  ksize必须大于1且是奇数
            m1 = ReplaceMatColor(m1, Color.Yellow);
            m1 = m1.GaussianBlur(new Size(11, 11), 0);
            Cv2.ImShow("Blur Decolor", m1);
            Mat m2 = new Mat();
            Cv2.CvtColor(m1, m2, ColorConversionCodes.BGR2GRAY);

            //Cv2.CvtColor(m1,m1, ColorConversionCodes.RGB2HSV);
            //Cv2.InRange(m1, new Scalar(0, 125, 43), new Scalar(10, 180, 43), m1);
            //Cv2.ImShow("hsv", m1);


            //3：霍夫圆检测：使用霍夫变换查找灰度图像中的圆。
            /*
                 * 参数：
                 *      1：输入参数： 8位、单通道、灰度输入图像
                 *      2：实现方法：目前，唯一的实现方法是HoughCirclesMethod.Gradient
                 *      3: dp      :累加器分辨率与图像分辨率的反比。默认=1
                 *      4：minDist: 检测到的圆的中心之间的最小距离。(最短距离-可以分辨是两个圆的，否则认为是同心圆-                            src_gray.rows/8)
                 *      5:param1:   第一个方法特定的参数。[默认值是100] canny边缘检测阈值低
                 *      6:param2:   第二个方法特定于参数。[默认值是100] 中心点累加器阈值 – 候选圆心
                 *      7:minRadius: 最小半径
                 *      8:maxRadius: 最大半径
                 * 
                 */
            CircleSegment[] cs = Cv2.HoughCircles(m2, HoughMethods.Gradient, 1, 80, 60, 30, 20, 50);
            Console.WriteLine("FoundCircle:" + cs.Length);
            var dst = src;
            for (int i = 0; i < cs.Count(); i++)
            {
                Console.WriteLine(cs[i].Radius);
                Cv2.Circle(dst, (int)cs[i].Center.X, (int)cs[i].Center.Y, (int)cs[i].Radius, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias);
                Cv2.Circle(dst, (int)cs[i].Center.X, (int)cs[i].Center.Y, 3, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias);
            }
            sp.Stop();
            Console.WriteLine(sp.ElapsedMilliseconds);
            Cv2.ImShow("OutputImage", dst);
            Cv2.WaitKey();
        }

        public static Mat SaturationGain(Mat mat, int saturation)
        {
            var h1 = mat.CvtColor(ColorConversionCodes.RGB2HSV);
            var channel = Cv2.Split(h1);

            for (int i = 0; i < channel[1].Rows; i++) //遍历所有像素
            {
                for (int j = 0; j < channel[2].Cols; j++)
                {
                    channel[1].Set(i, j, saturation); //改变饱和度
                }
            }

            Cv2.Merge(channel, h1);
            return h1.CvtColor(ColorConversionCodes.HSV2RGB);
        }

        public static Mat ReplaceMatColor(Mat m1, Color targetColor)
        {
            for (var y = 0; y < m1.Height; y++)
            {
                for (var x = 0; x < m1.Width; x++)
                {
                    var color = m1.Get<Vec3b>(y, x);
                    //BGR
                    bool b = false;
                    if (targetColor == Color.Red)
                        b = (color.Item2 == 255 && color.Item1 < 100 && color.Item0 == 0); //for R
                    else if (targetColor == Color.Green)
                        b = (color.Item1 > 100 && color.Item1 == 255 && color.Item2 == 0); //for G;
                    else if (targetColor == Color.Yellow)
                        b = (color.Item2 == 255 && color.Item1 > 100 && color.Item0 == 0); //for Y

                    if (b)
                    {
                        color.Item0 = 0;
                        color.Item1 = 0;
                        color.Item2 = 0;
                    }
                    else
                    {
                        color.Item0 = 255;
                        color.Item1 = 255;
                        color.Item2 = 255;
                    }
                    m1.Set(y, x, color);
                }
            }

            return m1;
        }
    }
}
