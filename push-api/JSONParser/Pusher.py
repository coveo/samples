import ssl
import json
import urllib
import http.client

class Pusher():

    # Change the source status
    def toggleStatus(status, organisation, apiKey, sourceName):
        headers = { 'Content-Type' : 'application/json', 'Authorization' : apiKey}
        params = {}
        ssl._create_default_https_context = ssl._create_unverified_context
        h = http.client.HTTPSConnection('push.cloud.coveo.com')
        h.request('POST', '/v1/organizations/' + organisation + '/sources/' + sourceName + '/status?statusType=' + status, params, headers)
        response = h.getresponse()
        print(response.status, response.reason, 'Source status change:', status)
        h.close();

    # Push a single element in the source
    def pushDocument(jsonFile, url, organisation, apiKey, sourceName):
        headers = { 'Content-Type' : 'application/json', 'Authorization' : apiKey}
        params = json.dumps(jsonFile)
        h = http.client.HTTPSConnection('push.cloud.coveo.com')
        h.request('PUT', '/v1/organizations/' + organisation + '/sources/' + sourceName + '/documents?documentId=' + url, params, headers)
        response = h.getresponse()
        print(response.status, response.reason, 'Document:', url)
        h.close();
