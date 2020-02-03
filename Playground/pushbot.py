from flask import Flask, request
from telegram import Bot

app = Flask(__name__)

TOKEN = "636174812:AAEZ6d2ZbxeD4r7pIENWj6cJ3-A0FX1Gg90"
bot = Bot(token = TOKEN)

@app.route('/<chat_id>/<text>')
def alarm(chat_id,text):
    content = text
    try:
        bot.send_message(chat_id, content)
        return "ok"
    except Exception as e:
        return str(e)

if __name__ == '__main__':
    app.run(port = 8000)