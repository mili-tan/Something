from flask import Flask
from flask import request,jsonify,make_response
import os,subprocess

app = Flask(__name__)

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app

eth = "eth0"
addcmd = "iptables -I INPUT 1 -s {}/32 -i {} -j ACCEPT"
delcmd = "iptables -D INPUT -s {}/32 -i {} -j ACCEPT"
checkcmd = "iptables -C INPUT -s {}/32 -i {} -j ACCEPT"

@app.route('/')
@app.route('/myip')
@app.route('/s2p')
def index():
    return request.remote_addr

@app.route('/s2p/add')
def add():
    ip = request.remote_addr
    if getIpExist(ip) == 1:
        if os.geteuid() == 0:
            os.popen(addcmd.format(ip))
        else:
            os.popen('sudo ' + addcmd.format(ip,eth))
        return jsonify({'status':"success",'info':"Success! Now ip {} is added to the rules.".format(ip)})
    else:
        return jsonify({'status':"repeat",'info':"Repeat! Your ip {} already exists in the rules.".format(ip)})

@app.route('/s2p/add/<ip>')
def addIp(ip):
    if getIpExist(ip) == 1:
        if os.geteuid() == 0:
            os.popen(addcmd.format(ip))
        else:
            os.popen('sudo ' + addcmd.format(ip,eth))
        return jsonify({'status':"success",'info':"Success! Now ip {} is added to the rules.".format(ip)})
    else:
        return jsonify({'status':"repeat",'info':"Repeat! Your ip {} already exists in the rules.".format(ip)})

@app.route('/s2p/del/<ip>')
def delIp(ip):
    if getIpExist(ip) == 0:
        if os.geteuid() == 0:
            os.popen(delcmd.format(ip))
        else:
            os.popen('sudo ' + delcmd.format(ip,eth))
        return jsonify({'status':"success",'info':"Success! Now ip {} is removed from the rules.".format(ip)})
    else:
        return jsonify({'status':"notexists",'info':"Error! Your ip {} not exists in the rules.".format(ip)})

@app.route('/s2p/get/<ip>')
def getIp(ip):
    return jsonify({'value':getIpExist(ip)})

def getIpExist(ip):
    code = 1
    if os.geteuid() == 0:
        #code = os.popen(checkcmd.format(ip,eth) + '; echo $?').read()
        code = os.system(checkcmd.format(ip,eth))/255
    else:
        #code = os.popen('sudo ' + checkcmd.format(ip,eth) + '; echo $?').read()
        code = os.system('sudo ' + checkcmd.format(ip,eth))/255
    return code

if __name__ == '__main__':
    print('S2P')
    HOST = os.environ.get('SERVER_HOST', '0.0.0.0')
    try:
        PORT = int(os.environ.get('SERVER_PORT', '5555'))
    except ValueError:
        PORT = 5555
    app.run(HOST, PORT)
