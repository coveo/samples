import json
import Pusher
import urllib.request

class Parser():
    def __inti__(self):
        self

# Read documents.json
    def readDocument(config):
        filePath = config['basicInformation']['filePath']
        print('Reading JSON file')
        with open(filePath) as data_file:
            data = json.load(data_file)
            arrayKey = config['basicInformation']['array']
            organisation = config['headers']['organisation']
            apiKey = config['headers']['apiKey']
            sourceName = config['headers']['sourceName']
            urlValue = config['headers']['url']
            documentArray = data[arrayKey]
            Parser.parseArray(documentArray, organisation, apiKey, sourceName, urlValue)

# Push array in source
    def parseArray(array, organisation, apiKey, sourceName, urlValue):
        StaReb = 'REBUILD'
        StaIdl = 'IDLE'
        pusher = Pusher.Pusher
        pusher.toggleStatus(StaReb, organisation, apiKey, sourceName)
        for item in array:
            itmUrl = item[urlValue]
            pusher.pushDocument(item, itmUrl, organisation, apiKey, sourceName)
        pusher.toggleStatus(StaIdl, organisation, apiKey, sourceName)
