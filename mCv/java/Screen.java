package mOpenCV;

import org.opencv.core.CvType;
import org.opencv.core.Mat;
import org.opencv.core.MatOfPoint;
import org.opencv.core.MatOfPoint2f;
import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;
import org.opencv.utils.Converters;

import java.util.ArrayList;
import java.util.List;

public class Screen {
    public static Mat Binarization(Mat src) {
        Mat gray = new Mat();
        Mat binary = new Mat();
        Imgproc.cvtColor(src, gray, Imgproc.COLOR_BGR2GRAY);
        //Imgproc.blur(gray, gray, new Size(5, 5));
        Imgproc.GaussianBlur(gray, gray, new Size(5, 5), 0);
        Imgproc.boxFilter(gray, gray, -1, new Size(15, 15));
        Imgproc.medianBlur(gray, gray, 5);
        Imgproc.threshold(gray, binary, 100, 255, 8);
        Mat element = Imgproc.getStructuringElement(0, new Size(3, 3));
        Imgproc.erode(binary, binary, element);
        Imgproc.morphologyEx(binary, binary, 3, element);
        return binary;
    }

    public static Mat InsightInsider(Mat binary, Mat src, Size size) {
        Imgproc.Canny(binary, binary, 50.0, 150.0);
        List<MatOfPoint> contours = new ArrayList<>();
        Mat hierarchy = new Mat();
        Imgproc.findContours(binary, contours, hierarchy, Imgproc.RETR_TREE, Imgproc.CHAIN_APPROX_SIMPLE);

        int index = 0;
        double maxim = Imgproc.contourArea(contours.get(0));
        for (int i = 0; i < contours.size(); i++) {
            double temp = Imgproc.contourArea(contours.get(i));
            if (maxim < temp) {
                maxim = temp;
                index = i;
            }
        }

        MatOfPoint2f approx = new MatOfPoint2f();
        double epsilon = 0.0075 * Imgproc.arcLength(new MatOfPoint2f(contours.get(index).toArray()), true);
        Imgproc.approxPolyDP(new MatOfPoint2f(contours.get(index).toArray()), approx, epsilon, true);

        List<Point> points = Points.PointSort(approx.toList());

        Mat drawing = Mat.zeros(src.size(), CvType.CV_8UC1);
        Imgproc.drawContours(drawing, contours, index, new Scalar(255, 0, 0), 1);
        for (Point i : points) {
            Imgproc.circle(drawing, i, 5, new Scalar(255, 0, 0), 10);
        }

        Mat quad = Mat.zeros(size, CvType.CV_8UC3);

        ArrayList<Point> resultPoints = new ArrayList<>();
        resultPoints.add(new Point(0.0, 0.0));
        resultPoints.add(new Point(quad.cols(), 0.0));
        resultPoints.add(new Point(quad.cols(), quad.rows()));
        resultPoints.add(new Point(0.0, quad.rows()));

        Mat cornerPts = Converters.vector_Point2f_to_Mat(points);
        Mat resultPts = Converters.vector_Point2f_to_Mat(resultPoints);

        Mat transformation = Imgproc.getPerspectiveTransform(cornerPts, resultPts);
        Imgproc.warpPerspective(src, quad, transformation, quad.size());

        return quad;
    }
}
