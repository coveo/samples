const config = require("./webpack.config");
const serve = require('webpack-serve');
const PagesFileWatcher = require("./webpack-serve.filewatcher.plugin");
const jslibRouter = require("./webpack-serve.jslib.middleware");

const serveConfig = {
    content: ["bin"],
    port: 3000,
    hot: {
        host: 'localhost',
        port: 3090
    },
    dev: {
        // Defines the path in which to provide the "hot" files.
        publicPath: "/js"
    },
    on: {
        listening(server) {
          new PagesFileWatcher(server, 3090);
        }
      },
      add: (app, middleware, options) => {
        // Required when using "add".
        middleware.webpack();
        middleware.content();

        // Registers the routes to provide a mocked API server.
        app.use(jslibRouter.routes());
    }
};

module.exports = config[0];
module.exports.serve = serveConfig;