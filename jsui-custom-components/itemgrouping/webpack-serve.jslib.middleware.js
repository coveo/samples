const fs = require("fs");
const Router = require("koa-router");

const router = Router();
const jslibRoute = "/jslib/";
router.all(`${jslibRoute}*`, (ctx, next) => {
    const fileToLoad = ctx.request.url.substring(jslibRoute.length);
    const pathToLoad = `${__dirname}/node_modules/coveo-search-ui/bin/js/${fileToLoad}`;
    console.log(fileToLoad);
    return new Promise((resolve, reject) => fs.readFile(pathToLoad, 'utf-8', (err, data) => !err ? resolve(data) : reject(err))).then(data => {
        ctx.body = data;
    }).catch(err => {
        console.error(err);
    })
});

module.exports = router;