from flask import Flask
from flask import request,jsonify,make_response
import os

app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

addcmd = "iptables -A INPUT -p tcp -s {} -j ACCEPT"
delcmd = "iptables -D INPUT -p tcp -s {} -j ACCEPT"

@app.route('/')
@app.route('/myip')
@app.route('/s2p')
def index():
    return request.remote_addr

@app.route('/s2p/add')
def add():
    ip = request.remote_addr
    if os.geteuid() != 0:
        os.popen(addcmd.format(ip))
    else:
        os.popen('sudo ' + addcmd.format(ip))
    return "Success! Now your ip " + ip + " is added to the rules."

@app.route('/s2p/add/<ip>')
def addip(ip):
    if os.geteuid() != 0:
        os.popen(addcmd.format(ip))
    else:
        os.popen('sudo ' + addcmd.format(ip))
    return "Success! Now ip " + ip + " is added to the rules."

@app.route('/s2p/del/<ip>')
def delip(ip):
    if os.geteuid() != 0:
        os.popen(addcmd.format(ip))
    else:
        os.popen('sudo ' + delcmd.format(ip))
    return "Success! Now ip " + ip + " is removed from the rules."


if __name__ == '__main__':
    print('mS2p')
    import os
    HOST = os.environ.get('SERVER_HOST', '0.0.0.0')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run(HOST, PORT)
