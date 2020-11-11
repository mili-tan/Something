using System;
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

            Mat src = new Mat(@"./carp1.jpg");
            Cv2.ImShow("src", src);
            src = src.Resize(new Size(src.Width / 2, src.Height / 2));
            //src = src.Resize(new Size(src.Width / 3, src.Height / 3));
            //for (var y = 0; y < src.Height; y++)
            //{
            //    for (var x = 0; x < src.Width; x++)
            //    {
            //        var color = src.Get<Vec3b>(y, x);
            //        //if (color.Item2 < 175)
            //        if (color.Item2 < 225)
            //        {
            //            color.Item0 = 255;
            //            color.Item1 = 0;
            //            color.Item2 = 0;
            //        }
            //        src.Set(y, x, color);
            //    }
            //}
            var binary = BinarizationMat(src);
            Cv2.ImShow("src", src);
            Cv2.ImShow("bin", binary);
            //var line = binary.Canny(100, 200);
            //Cv2.ImShow("line", line);
            var fScreenMat = FindContoursMat(binary, src);
            fScreenMat = fScreenMat.Resize(new Size(fScreenMat.Width * 2, fScreenMat.Height * 2));
            fScreenMat = new Mat(fScreenMat,
                new Rect((int)(fScreenMat.Width * 0.05), (int)(fScreenMat.Height * 0.1),
                    fScreenMat.Width - (int)(fScreenMat.Width * 0.1), fScreenMat.Height - (int)(fScreenMat.Height * 0.2)));
            var fScreenBinaryMat = BinarizationMat(fScreenMat);
            Cv2.BitwiseNot(fScreenBinaryMat, fScreenBinaryMat, new Mat());
            var fCardMat = FindContoursMat(fScreenBinaryMat, fScreenMat);

            //Cv2.ImShow("fScreenMat", fScreenMat);
            //Cv2.ImShow("fCardMat", fCardMat);
            //dstImg = new Mat(dstImg,
            //    new Rect((int)(dstImg.Width * 0.15), (int)(dstImg.Height * 0.3),
            //        dstImg.Width - (int)(dstImg.Width * 0.3), dstImg.Height - (int)(dstImg.Height * 0.6)));
            //fCardMat = fCardMat.Resize(new Size(fCardMat.Width / 1.5, fCardMat.Height / 1.5));

            Cv2.ImShow("fCardMat", fCardMat);
            var dstImg = BinarizationMat(fCardMat);
            dstImg = dstImg.Threshold(50, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            Cv2.BitwiseNot(dstImg, dstImg, new Mat());
            Cv2.ImShow("dst", dstImg);
            dstImg = dstImg.Resize(new Size(dstImg.Width / 2.5, dstImg.Height / 2.5));
            var engine = new TesseractEngine("./tessdata", "din+eng+chi_sim", EngineMode.Default);
            var resProcess = engine.Process(Pix.LoadTiffFromMemory(dstImg.ToBytes(".tiff")));
            MessageBox.Show(resProcess.GetText());
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
            Cv2.Threshold(gray, binary, 100, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            binary = binary.Erode(element);
            binary = binary.MorphologyEx(MorphTypes.Close, element);
            //Cv2.ImShow("bin " + DateTime.Now, binary);
            return binary;
        }
    }
}
