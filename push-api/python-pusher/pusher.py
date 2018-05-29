import ssl
import json
import urllib
import http.client
import parameters
import config

PUSH_API_PARAM = parameters.PUSH_API_PARAM()
MESSAGES = parameters.MESSAGES()
ORG = config.COVEO_CLOUD_ORG()

class Pusher():
    # Push a single document to Coveo Cloud
    def pushDocument(jsonDocument, uri):
        headers = { PUSH_API_PARAM.HEADER_CONTENT_TYPE : PUSH_API_PARAM.HEADER_CONTENT_TYPE_VALUE, PUSH_API_PARAM.HEADER_AUTHORIZATION : ORG.API_KEY}
        params = json.dumps(jsonDocument)
        url =  PUSH_API_PARAM.PUSH_API_URL_VERSION + PUSH_API_PARAM.PUSH_API_URL_ORG + ORG.ORG_ID + PUSH_API_PARAM.PUSH_API_URL_SOURCE + ORG.SOURCE_NAME + PUSH_API_PARAM.PUSH_API_URL_DOCUMENT_ID + uri
        httpConnection = http.client.HTTPSConnection(PUSH_API_PARAM.PUSH_API_URL)
        httpConnection.request(PUSH_API_PARAM.REQUEST_TYPE_PUT, url, params, headers)
        response = httpConnection.getresponse()
        httpConnection.close();
        if (response.status == 202):
            print(MESSAGES.DOCUMENT_PUSH_SUCCESS, uri)
        else:
            print(MESSAGES.DOCUMENT_PUSH_FAILED, response.status, response.reason)

class Status():
    # Change the status of a source in Coveo Cloud
    def changeSourceStatus(self, status):
        headers = { PUSH_API_PARAM.HEADER_CONTENT_TYPE : PUSH_API_PARAM.HEADER_CONTENT_TYPE_VALUE, PUSH_API_PARAM.HEADER_AUTHORIZATION : ORG.API_KEY}
        params = {}
        url =  PUSH_API_PARAM.PUSH_API_URL_VERSION + PUSH_API_PARAM.PUSH_API_URL_ORG + ORG.ORG_ID + PUSH_API_PARAM.PUSH_API_URL_SOURCE + ORG.SOURCE_NAME + PUSH_API_PARAM.PUSH_API_URL_STATUS + status
        ssl._create_default_https_context = ssl._create_unverified_context
        httpConnection = http.client.HTTPSConnection(PUSH_API_PARAM.PUSH_API_URL)
        httpConnection.request(PUSH_API_PARAM.REQUEST_TYPE_POST, url, params, headers)
        response = httpConnection.getresponse()
        httpConnection.close();
        if (response.status == 201):
            print(MESSAGES.SOURCE_STATUS_CHANGE_SUCCESS, status)
        else:
            print(MESSAGES.SOURCE_STATUS_CHANGE_FAILED, response.status, response.reason)