import http.client
import urllib
import json

class Pusher():
    def __init__(self):
        self

    def toggleStatus(status, organisation, apiKey, sourceName):
        headers = { 'Content-Type' : 'application/json', 'Authorization' : apiKey}
        params = {}
        h = http.client.HTTPSConnection('push.cloud.coveo.com')
        statusType = status
        h.request('POST', '/v1/organizations/' + organisation + '/sources/' + sourceName + '/status?statusType=' + statusType, params, headers)
        response = h.getresponse()
        print(response.status, response.reason)
        h.close();

    def pushDocument(jsonFile, url, organisation, apiKey, sourceName):
        headers = { 'Content-Type' : 'application/json', 'Authorization' : apiKey}
        params = json.dumps(jsonFile)
        h = http.client.HTTPSConnection('push.cloud.coveo.com')
        h.request('PUT', '/v1/organizations/' + organisation + '/sources/' + sourceName + '/documents?documentId=' + url, params, headers)
        response = h.getresponse()
        print(response.status, response.reason)
        h.close();