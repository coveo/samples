const getFields = require('./src/getFields');
const platform = 'https://platform.cloud.coveo.com';

if (!process.argv[3]) {
    console.log('Missing API key and/or org id. The syntax is "node deleteUnusedFields.js api-key org-id".');
    return;
}

const settings = {
    apiKey: process.argv[2],
    orgId: process.argv[3],
    platform: platform
}

const currentPage = 0;
const unusedFields = [];

getFields(settings, currentPage, unusedFields);