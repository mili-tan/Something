from flask import Flask, jsonify, request
from sentence_transformers import SentenceTransformer
import numpy as np

app = Flask(__name__)
model = SentenceTransformer('sentence-transformers/distiluse-base-multilingual-cased-v2')


@app.route("/embedding")
def index():
    sentence = request.args.get('s')
    embedding = model.encode(sentence)
    data = {
        "sentence": sentence,
        "embedding": np.array(embedding).tolist()
    }
    return jsonify(data)


if __name__ == '__main__':
    app.run(debug=True, port=5000)
