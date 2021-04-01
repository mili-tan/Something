package mOpenCV;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;

import org.opencv.android.Utils;
import org.opencv.core.Mat;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.concurrent.atomic.AtomicReference;

public class Tools {

    public static Bitmap GetBitmap(String url) {
        AtomicReference<Bitmap> bitmap = new AtomicReference<>();
        Thread thread = new Thread(() -> {
            URL imageurl = null;
            try {
                imageurl = new URL(url);
            } catch (MalformedURLException e) {
                e.printStackTrace();
            }
            try {
                HttpURLConnection conn = (HttpURLConnection) imageurl.openConnection();
                conn.setDoInput(true);
                conn.connect();
                InputStream is = conn.getInputStream();
                bitmap.set(BitmapFactory.decodeStream(is));
                is.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        });
        thread.start();
        try {
            thread.join(500);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        return bitmap.get();
    }

    public static Mat GetMat(String url) {
        return Bitmap2Mat(GetBitmap(url), 4);
    }

    public static Mat GetMat(String url, int resize) {
        return Bitmap2Mat(GetBitmap(url), resize);
    }

    public static void SaveMat(Mat mat, String filename, Activity activity) {
        try {
            Bitmap res = Bitmap.createBitmap(mat.width(), mat.height(), Bitmap.Config.RGB_565);
            Utils.matToBitmap(mat, res);
            FileOutputStream outputStream = new FileOutputStream(new File(activity.getBaseContext().getExternalFilesDir(Environment.DIRECTORY_PICTURES),
                    System.currentTimeMillis() + "-" + filename + ".png"));     //构建输出流
            res.compress(Bitmap.CompressFormat.PNG, 100, outputStream);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static Mat Bitmap2Mat(Bitmap bm) {
        Mat mat = new Mat();
        Utils.bitmapToMat(bm, mat);
        return mat;
    }

    public static Mat Bitmap2Mat(Bitmap bm, int resize) {
        Mat mat = new Mat();
        Utils.bitmapToMat(bm, mat);
        Imgproc.resize(mat, mat, new Size(mat.width() / resize, mat.height() / resize));
        return mat;
    }

    public static Bitmap Mat2Bitmap(Mat mat) {
        Bitmap bm = Bitmap.createBitmap(mat.width(), mat.height(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(mat, bm);
        return bm;
    }
}
