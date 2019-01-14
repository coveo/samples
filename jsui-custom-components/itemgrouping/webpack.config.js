const webpack = require('webpack');
const minimize = process.argv.indexOf('--minimize') !== -1;
const path = require("path");
const colors = require('colors');
const failPlugin = require('webpack-fail-plugin');
if (minimize) {
  console.log('Building minified version of the library'.bgGreen.red);
} else {
  console.log('Building non minified version of the library'.bgGreen.red);
}

module.exports = [{
  mode: minimize ? 'production' : 'development',
  devtool: 'source-map',
  entry: {
    'coveo.extension': path.resolve(__dirname,'./src/Index')
  },
  output: {
    path: require('path').resolve('./bin/js'),
    // Output a filename based on the entry. This will generate a "coveo.extension.js" file.
    filename: minimize ? `[name].min.js` : `[name].js`,
    library: 'CoveoExtension'
  },
  externals: [{
    // Defines the module "coveo-search-ui" as external, "Coveo" is defined in the global scope. 
    // This requires you to load the original CoveoJsSearch.js file in your page.
    "coveo-search-ui":"Coveo"
  }],
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [{
      test: /\.ts$/, 
      loader: 'ts-loader',
      options: {}
    }]
  },
  bail: true
}]
