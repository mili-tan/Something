import cv2
import numpy as np

prototxt_path = "dnn/deploy.prototxt"
model_path = "dnn/res10_300x300_ssd_iter_140000.caffemodel"
model = cv2.dnn.readNetFromCaffe(prototxt_path, model_path)
cap = cv2.VideoCapture(0)

face_cascade = cv2.CascadeClassifier("haarcascade_frontalface_default.xml")

while True:
    _, image = cap.read()

    h, w = image.shape[:2]

    image_gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(image_gray, 1.3, 5)

    h, w = image.shape[:2]

    blob = cv2.dnn.blobFromImage(image, 1.0, (300, 300), (104.0, 177.0, 123.0))
    model.setInput(blob)
    output = np.squeeze(model.forward())
    font_scale = 0.5
    for i in range(0, output.shape[0]):
        confidence = output[i, 2]
        if confidence > 0.5:
            box = output[i, 3:7] * np.array([w, h, w, h])
            start_x, start_y, end_x, end_y = box.astype(np.int32)
            cv2.rectangle(image, (start_x, start_y), (end_x, end_y), color=(255, 0, 0), thickness=2)
            cv2.putText(image, f"DNN:{confidence * 100:.2f}%", (start_x, start_y - 5), cv2.FONT_HERSHEY_SIMPLEX,
                        font_scale, (255, 0, 0), 1)

    for x, y, width, height in faces:
        cv2.putText(image, "HAAR", (x, y - 5), cv2.FONT_HERSHEY_SIMPLEX, font_scale, (0, 255, 0), 1)
        cv2.rectangle(image, (x, y), (x + width, y + height), color=(0, 255, 0), thickness=2)

    cv2.imshow("image", image)
    if cv2.waitKey(1) == ord("q"):
        break
cv2.destroyAllWindows()
cap.release()
