const getFields = require('./src/getFields');
const platform = 'https://platform.cloud.coveo.com';

let settings = {};

if (!process.argv[3]) {
    console.log('Missing API key and/or org id. The syntax is "node deleteUnusedFields.js api-key org-id".');
    return;
}

settings = {
    apiKey: process.argv[2],
    orgId: process.argv[3],
    platform: platform
}

let currentPage = 0;
let unusedFields = '';

getFields(settings, currentPage, unusedFields);