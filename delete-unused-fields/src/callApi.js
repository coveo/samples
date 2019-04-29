module.exports = function callApi(settings, cb) {
    const { URL } = require('url');
    const h = require('https');

   let url = new URL (settings.url);

   const options = {
       host: url.host,
       port: 443,
       path: url.pathname + url.search,
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