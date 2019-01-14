'use strict';
const fs = require('fs');
const ncp = require('ncp').ncp;

const directoriesToCreate = [
  "./bin",
  "./bin/css",
  "./bin/image",
  "./bin/js",
  "./bin/templates"
]

const filesToCopy = [{
  "src":'./node_modules/coveo-search-ui/bin/css/CoveoFullSearch.css',
  "dest":'./bin/css/CoveoFullSearch.css'
}];

const folderToCopy = [{
  "src": "./node_modules/coveo-search-ui/bin/js",
  "dest" : "./bin/js"
},{
  "src":'./node_modules/coveo-search-ui/bin/image',
  "dest":'./bin/image'
},{
  "src":'./templates',
  "dest":'./bin/templates'
},{
  "src":'./pages',
  "dest":'./bin'
}];

directoriesToCreate.filter(directory => !fs.existsSync(directory))
                   .forEach(directory => fs.mkdirSync(directory));

filesToCopy.forEach(file => fs.createReadStream(file.src).pipe(fs.createWriteStream(file.dest)))
folderToCopy.forEach(folder => ncp(folder.src, folder.dest));