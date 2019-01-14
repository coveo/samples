const webpack = require("webpack");
const webpackConfig = require('./webpack.config.js');

const componentsConfig = webpackConfig[0];

componentsConfig.optimization = {
    noEmitOnErrors: true
};

componentsConfig.externals.push({
    "coveo-search-ui-tests": "CoveoJsSearchTests"
});

componentsConfig.plugins = [
    new webpack.SourceMapDevToolPlugin({
        filename: null, // if no value is provided the sourcemap is inlined
        test: /\.(ts|js)($|\?)/i // process .js and .ts files only
    })
];

module.exports = webpackConfig;