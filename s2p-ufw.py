from flask import Flask
from flask import request,jsonify,make_response
import os

app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

addcmd = "ufw allow from {}"

@app.route('/')
@app.route('/myip')
@app.route('/s2p')
def index():
    return request.remote_addr

@app.route('/s2p/add')
def s2p():
    ip = request.remote_addr
    os.popen(addcmd.format(ip))
    return "Success! Now your ip " + ip + " is added to the rules."

@app.route('/s2p/add/<ip>')
def s2pip(ip):
    os.popen(addcmd.format(ip))
    return "Success! Now ip " + ip + " is added to the rules."

if __name__ == '__main__':
    print('mS2p')
    import os
    HOST = os.environ.get('SERVER_HOST', '0.0.0.0')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run(HOST, PORT)
