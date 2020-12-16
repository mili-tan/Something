using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;

namespace mOpenCV.Shape
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Mat src = new Mat(@"./t2.png");
            src = src.Resize(new Size(src.Width / 4, src.Height / 4));
            Cv2.ImShow("src", src);

            var binary = BinarizationMat(src);
            Cv2.ImShow("bin", binary);
            var fScreenMat = FindContoursMat(binary, src);
            fScreenMat = new Mat(fScreenMat,
                new Rect((int)(fScreenMat.Width * 0.025), (int)(fScreenMat.Height * 0.05),
                    fScreenMat.Width - (int)(fScreenMat.Width * 0.05), fScreenMat.Height - (int)(fScreenMat.Height * 0.1)));
            Cv2.ImShow("Screen", fScreenMat);

            var m2 = SaturationGain(fScreenMat, 255);
            Cv2.ImShow("SaturationGain", m2);
            Cv2.CvtColor(m2, m2, ColorConversionCodes.BGR2GRAY);
            Mat b2 = m2.Threshold(100, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            Cv2.FindContours(b2, out var contours, out var hierarchy, RetrievalModes.Tree,
                ContourApproximationModes.ApproxSimple);
            Cv2.ImShow("b2", b2);
            var dst = fScreenMat;
            foreach (var itemPoint in contours)
            {
                Console.WriteLine("_________________");
                var epsilon = 0.0075 * Cv2.ArcLength(itemPoint, true);
                var approx = Cv2.ApproxPolyDP(itemPoint, epsilon, true);
                if (approx.FirstOrDefault().X == 0 || approx.FirstOrDefault().Y == 0) continue;

                Cv2.DrawContours(dst, new IEnumerable<Point>[] { approx }, -1, Scalar.Green, 3);

                Console.WriteLine("Approx Angle:" + approx.Length);
                if (approx.Length == 3)
                    Cv2.PutText(dst, "Triangle", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5, Scalar.Red);
                else if (approx.Length == 4)
                {
                    var rect = Cv2.BoundingRect(approx);
                    var rotatedRect = Cv2.MinAreaRect(approx);
                    var box = Cv2.BoxPoints(rotatedRect);
                    Console.WriteLine(rotatedRect.Angle);
                    Cv2.PutText(dst, rotatedRect.Angle.ToString("0.0"), approx.LastOrDefault(),
                        HersheyFonts.HersheyComplex, 0.5, Scalar.Yellow);
                    Cv2.Line(dst, new Point(box[2].X, box[2].Y), new Point(box[0].X, box[0].Y), Scalar.White, 2);

                    var aspectRatio = rect.Width / rect.Height;
                    if (aspectRatio >= 0.9 && aspectRatio <= 1.1)
                    {
                        if ((Math.Abs(rotatedRect.Angle) >= 80 && Math.Abs(rotatedRect.Angle) <= 100) ||
                            Math.Abs(rotatedRect.Angle) <= 10)
                            Cv2.PutText(dst, "Square", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5,
                                Scalar.Red);
                        else
                            Cv2.PutText(dst, "Diamond", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5,
                                Scalar.Red);
                    }
                    else
                    {
                        Cv2.PutText(dst, "Rectangle", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5,
                            Scalar.Red);
                    }
                }
                else if (approx.Length == 5)
                    Cv2.PutText(dst, "Pentagon", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5, Scalar.Red);
                else if (approx.Length == 10)
                    Cv2.PutText(dst, "Star", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5, Scalar.Red);
                else if (approx.Length > 10)
                    Cv2.PutText(dst, "Circle", approx.FirstOrDefault(), HersheyFonts.HersheyComplex, 0.5,
                        Scalar.Red);

                foreach (var item in approx)
                {
                    Console.WriteLine(item.X + " " + item.Y);
                    Cv2.Circle(dst, item.X, item.Y, 5, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias);
                    Cv2.ImShow("dst", dst);
                    //Cv2.WaitKey();
                }
                //foreach (var item in itemPoint) Console.WriteLine(item.X + " " + item.Y);
                //Console.WriteLine("_________________");
            }
            Cv2.ImShow("dst", dst);
            Cv2.WaitKey();

        }

        public static Mat FindContoursMat(Mat binary, Mat src)
        {
            Cv2.FindContours(binary, out var contours, out _, RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);
            var rotateRect = new RotatedRect[contours.Length];
            var contours_poly = new Point[contours.Length][];
            for (var i = 0; i < contours.Length; i++)
            {
                contours_poly[i] = Cv2.ApproxPolyDP(contours[i], 30, true); //返回凸包，单线长大于30过滤
                rotateRect[i] = Cv2.MinAreaRect(contours_poly[i]); //最小外接矩形集合
                var angle = rotateRect[i].Angle; //矩形角度
                var pot = rotateRect[i].Points();
                var line1 = Math.Sqrt((pot[0].X - pot[1].X) * (pot[0].X - pot[1].X) +
                                      (pot[0].Y - pot[1].Y) * (pot[0].Y - pot[1].Y));
                var line2 = Math.Sqrt((pot[0].X - pot[3].X) * (pot[0].X - pot[3].X) +
                                      (pot[0].Y - pot[3].Y) * (pot[0].Y - pot[3].Y));
                if (line1 * line2 < 1000) continue; //过滤，太小的矩形直接pass
                if (line1 > line2) angle += 90; //依据实际情况进行判断

                var Roi = new Mat(src.Size(), MatType.CV_8UC3);
                Roi.SetTo(0);
                Cv2.DrawContours(binary, contours, -1, Scalar.White, -1); //在二值图像中圈出轮廓区域并染白
                //Cv2.DrawContours(binary, contours, -1, Scalar.White);
                //Cv2.ImShow("bin", binary);
                src.CopyTo(Roi, binary); //将原图通过mask抠图到Roi
                //Cv2.ImShow("Roi", Roi);
                var afterRotato = new Mat(src.Size(), MatType.CV_8UC3);
                afterRotato.SetTo(0);
                Point2f center = rotateRect[i].Center;
                var M = Cv2.GetRotationMatrix2D(center, angle, 1); //计算变换矩阵
                Cv2.WarpAffine(Roi, afterRotato, M, Roi.Size(), InterpolationFlags.Linear,
                    BorderTypes.Wrap); //得到变换后的图像，滤除其他信息
                var bin2 = new Mat();
                //Cv2.ImShow("after", afterRotato);
                Cv2.CvtColor(afterRotato, bin2, ColorConversionCodes.RGB2GRAY);
                Cv2.Threshold(bin2, bin2, 50, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                Cv2.FindContours(bin2, out var con, out _, RetrievalModes.External,
                    ContourApproximationModes.ApproxSimple);
                foreach (var t in con)
                {
                    var rect = Cv2.BoundingRect(t); //直接使用矫正矩形，因为矫正后不需要再旋转
                    if (rect.Height * rect.Width < 8000) continue; //过滤干扰信息
                    return new Mat(afterRotato, rect);
                }
            }

            return new Mat();
        }

        public static Mat BinarizationMat(Mat src)
        {
            Mat gray = new Mat();
            Mat binary = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.RGB2GRAY);
            gray = gray.Blur(new Size(3, 3));
            gray = gray.GaussianBlur(new Size(5, 5), 0);
            gray = gray.BoxFilter(-1, new Size(10, 10), normalize: true);
            //Cv2.MedianBlur(gray, gray, 5);
            Cv2.Threshold(gray, binary, 100, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            binary = binary.Erode(element);
            binary = binary.MorphologyEx(MorphTypes.Close, element);
            //Cv2.ImShow("bin " + DateTime.Now, binary);
            return binary;
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
    }
}
