module.exports = function getFields(settings, currentPage, unusedFields) {
    const request = require('./callApi');
    getOptions = {
        url: settings.platform + '/rest/organizations/' + settings.orgId + '/sources/page/fields?includeMappings=true&page=' + currentPage + '&perPage=100',
        headers: {
            Authorization: 'Bearer ' + settings.apiKey
        }
    }
    request(getOptions, function (err, response, body) {
        if (err) {
            console.log('Err: ' + err);
            return;
        }
        if (response.statusCode != 200) {
            console.log(response.statusCode + ' - ' + body);
            return;
        }

        JSON.parse(body).items.forEach(field => {
            if (field.sources.length < 1) {
                unusedFields = unusedFields +  field.name +',';
            }
        });
        if (JSON.parse(body).items.length == 100) {
            currentPage++;
            getFields(settings, currentPage, unusedFields);
        } else {
            if (unusedFields.length == 0) {
                console.log("No unused fields detected.");
                return;
            }
            unusedFields = unusedFields.slice(0, -1);
            let deleteOptions = {
                url: settings.platform + '/rest/organizations/' + settings.orgId + '/sources/fields/batch/delete?fields=' + unusedFields,
                method: 'DELETE',
                headers: {
                    Authorization: 'Bearer ' + settings.apiKey
                }
            }
            request(deleteOptions, function(deleteErr, deleteResponse, deleteBody) {
                if (deleteErr) {
                    console.log('Error deleting fields: ' + deleteErr);
                    return;
                }
                if (deleteResponse != 204) {
                    console.log(deleteResponse.statusCode + ' - ' + deleteBody);
                    return;
                }
                console.log('All unused fields deleted.');
            })
        }
    })
}