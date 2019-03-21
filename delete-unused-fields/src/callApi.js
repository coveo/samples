module.exports = function callApi(settings, cb) {
    const h = require('https');
    const port = 443;

    let path = '';
    let shortUrlWithPath = settings.url.substring(8);
    let shortUrl = shortUrlWithPath.substring(0, shortUrlWithPath.indexOf('/'));

    if (shortUrlWithPath.indexOf('/') > -1) {
        path = shortUrlWithPath.substring(shortUrlWithPath.indexOf('/'));
    }
    const options = {
        host: shortUrl,
        port: port,
        path: path,
        method: settings.method,
        headers: settings.headers
    }

    const req = h.request(options, (res) => {
        let d = '';
        res.on('data', (chunk) => {
            if (chunk) {
                d += chunk
            }
        });
        res.on('end', () => {
            if (d === 'null') {
                d = null;
            }
            const err = null;
            if (typeof cb === "function") {
                cb(err, res, d);
            } else {
                console.error("Callback is not a function.");
            }
        })
    });

    req.on('error', (e) => {
        if (typeof cb === "function") {
            cb(e);
        } else {
            console.error("Callback is not a function.");
        }
    });

    if (settings.data) {
        const dataToSend = JSON.stringify(settings.data);
        req.write(dataToSend);
    }
    req.end();
}