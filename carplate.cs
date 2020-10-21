using System;
using System.IO;
using System.Windows;
using OpenCvSharp;
using Tesseract;
using Point = OpenCvSharp.Point;
using Rect = OpenCvSharp.Rect;
using Size = OpenCvSharp.Size;

namespace mOpenCV.CarplateWPF
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 
    {
        public Window1()
        {
            InitializeComponent();

            Mat src = new Mat(@"./carp.jpg");
            src = src.Resize(new Size(src.Width / 2, src.Height / 2));
            Cv2.ImShow("src", src);
            Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.RGB2GRAY);
            gray = gray.GaussianBlur(new Size(5, 5), 0);
            Cv2.Threshold(gray, binary, 50, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            binary = binary.Erode(element);
            binary = binary.MorphologyEx(MorphTypes.Close, element);
            Cv2.ImShow("bin", binary);
            //var line = binary.Canny(100, 200);
            //Cv2.ImShow("line", line);
            //Cv2.WaitKey();
            //建立轮廓接受数组
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
            //最小外接矩形接收数组
            MessageBox.Show(contours.Length.ToString());
            RotatedRect[] rotateRect = new RotatedRect[contours.Length];
            Point[][] contours_poly = new Point[contours.Length][];
            for (int i = 0; i < contours.Length; i++)
            {
                contours_poly[i] = Cv2.ApproxPolyDP(contours[i], 30, true);//返回凸包，单线长大于30过滤

                rotateRect[i] = Cv2.MinAreaRect(contours_poly[i]);//最小外接矩形集合

                Point2f[] pot = new Point2f[4];//新建点集合接收点集合

                //for (int i = 0; i < rotateRect.Length; i++)
                //{
                var angle = rotateRect[i].Angle;//矩形角度
                pot = rotateRect[i].Points();//矩形的4个角
                var line1 = Math.Sqrt((pot[0].X - pot[1].X) * (pot[0].X - pot[1].X) + (pot[0].Y - pot[1].Y) * (pot[0].Y - pot[1].Y));
                var line2 = Math.Sqrt((pot[0].X - pot[3].X) * (pot[0].X - pot[3].X) + (pot[0].Y - pot[3].Y) * (pot[0].Y - pot[3].Y));
                //if (line1 * line2 < 1000)//过滤，太小的矩形直接pass
                //{
                //    continue;
                //}
                if (line1 > line2)//依据实际情况进行判断
                {
                    angle += 90;
                }

                Mat Roi = new Mat(src.Size(), MatType.CV_8UC3);
                Roi.SetTo(0);//全黑
                Cv2.DrawContours(binary, contours, -1, Scalar.White, -1);//在二值图像中圈出轮廓区域并染白
                Cv2.ImShow("bin", binary);
                src.CopyTo(Roi, binary);//将原图通过mask抠图到Roi
                Cv2.ImShow("Roi", Roi);
                Mat afterRotato = new Mat(src.Size(), MatType.CV_8UC3);
                afterRotato.SetTo(0);
                Point2f center = rotateRect[i].Center;
                Mat M = Cv2.GetRotationMatrix2D(center, angle, 1);//计算变换矩阵
                Cv2.WarpAffine(Roi, afterRotato, M, Roi.Size(), InterpolationFlags.Linear, BorderTypes.Transparent);//得到变换后的图像，滤除其他信息
                Cv2.ImShow("旋转后", afterRotato);

                Mat bin2 = new Mat();
                Cv2.ImShow("after", afterRotato);
                Cv2.CvtColor(afterRotato, bin2, ColorConversionCodes.RGB2GRAY);
                Cv2.Threshold(bin2, bin2, 20, 255, ThresholdTypes.Binary);
                Point[][] con;
                HierarchyIndex[] temp;//接收矫正后的轮廓信息
                Cv2.FindContours(bin2, out con, out temp, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                for (int j = 0; j < con.Length; j++)
                {
                    Rect rect = Cv2.BoundingRect(con[j]);//直接使用矫正矩形，因为矫正后不需要再旋转
                    if (rect.Height * rect.Width < 8000)//过滤干扰信息
                    {
                        continue;
                    }
                    Mat dstImg = new Mat(afterRotato, rect);

                    Cv2.ImShow("dst", dstImg);
                    dstImg.SaveImage("dst.jpg");
                    ////string name = "dst" + i;//主要看调试的时候有几个结果
                    //dstImg = dstImg.CvtColor(ColorConversionCodes.RGB2GRAY);
                    //dstImg = dstImg.Threshold(10, 255, ThresholdTypes.Otsu);
                    //Cv2.ImShow("chan", dstImg.Canny(100, 200));

                    //dstImg.FindContours(out var con1, out var hie1, RetrievalModes.External,
                    //    ContourApproximationModes.ApproxNone);
                    //dstImg.DrawContours(con1, -1, Scalar.Green, 3);
                    //Cv2.ImShow("dst2", dstImg);
                }
            }
            Cv2.WaitKey();
            Console.ReadLine();
        }
    }
}
