package mOpenCV;

import org.opencv.core.Point;

import java.util.ArrayList;
import java.util.List;

public class Points {
    public static Point GetCenterPoint(List<Point> points) {
        List<Double> x = new ArrayList<>();
        List<Double> y = new ArrayList<>();
        for (Point i : points) {
            x.add(i.x);
            y.add(i.y);
        }

        double xAge = 0.0;
        for (double b : x)
            xAge += b;
        xAge = xAge / x.size();

        double yAge = 0.0;
        for (double b : y)
            yAge += b;
        yAge = yAge / y.size();

        return new Point(xAge, yAge);
    }
    public static List<Point> PointSort(List<Point> corners) {

        ArrayList<Point> points = new ArrayList<>();
        Point topLeft = new Point();
        Point topRight = new Point();
        Point bottomLeft = new Point();
        Point bottomRight = new Point();

        int centerX = 0;
        int centerY = 0;

        for (int i = 0; i < corners.size(); i++) {
            centerX += corners.get(i).x / corners.size();
            centerY += corners.get(i).y / corners.size();
        }

        for (int i = 0; i < corners.size(); i++) {
            Point point = corners.get(i);
            if (point.y < centerY && point.x > centerX) {
                topRight.x = point.x;
                topRight.y = point.y;
            } else if (point.y < centerY && point.x < centerX) {
                topLeft.x = point.x;
                topLeft.y = point.y;
            } else if (point.y > centerY && point.x < centerX) {
                bottomLeft.x = point.x;
                bottomLeft.y = point.y;
            } else if (point.y > centerY && point.x > centerX) {
                bottomRight.x = point.x;
                bottomRight.y = point.y;
            }
        }

        points.add(topLeft);
        points.add(topRight);
        points.add(bottomRight);
        points.add(bottomLeft);

        return points;
    }
}
