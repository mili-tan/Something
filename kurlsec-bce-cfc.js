const crypto = require('crypto');
const http = require('http');

const btoa = function (str) {
    return Buffer.from(str).toString('base64');
}

async function hexmd5(message) {
    return crypto.createHash('md5').update(message).digest("hex");
}


function getRequest(url) {
    return new Promise((resolve, reject) => {
        const req = http.get(url, res => {
            let rawData = '';

            res.on('data', chunk => {
                rawData += chunk;
            });

            res.on('end', () => {
                try {
                    resolve(JSON.parse(rawData));
                } catch (err) {
                    reject(new Error(err));
                }
            });
        });

        req.on('error', err => {
            reject(new Error(err));
        });
    });
}

exports.handler = async (event, context, callback) => {
    var appkey = '';
    var secret = '';

    var q = btoa(event.pathParameters.name);

    var timezone = 8;
    var offset_GMT = new Date().getTimezoneOffset();
    var nowDate = new Date().getTime();
    var targetDate = new Date(nowDate + offset_GMT * 60 * 1000 + timezone * 60 * 60 * 1000);

    var timee = String(Date.parse(new Date()));
    var timestamp = timee.substring(0, 10) + '.000';

    var sign = await hexmd5('/phish/?appkey=' + appkey + '&q=' + q + '&timestamp=' + timestamp + secret);
    var url = 'http://open.pc120.com/phish/?q=' + q + '&appkey=k-33356&timestamp=' + timestamp + '&sign=' + sign;

    context.succeed(await getRequest(url));
};
