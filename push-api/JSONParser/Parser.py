import json
import Pusher
class Parser():
    def __inti__(self):
        self

    def readDocument(config):
        filePath = config['basicInformation']['filePath']
        with open(filePath) as data_file:
            data = json.load(data_file)
            arrayKey = config['basicInformation']['array']
            organisation = config['headers']['organisation']
            apiKey = config['headers']['apiKey']
            sourceName = config['headers']['sourceName']
            urlValue = config['headers']['url']
            documentArray = data[arrayKey]
            Parser.parseArray(documentArray, organisation, apiKey, sourceName, urlValue)

    def parseArray(array, organisation, apiKey, sourceName, urlValue):
        pusher = Pusher.Pusher
        pusher.toggleStatus('REBUILD', organisation, apiKey, sourceName)
        for item in array:
            url = item[urlValue]
            pusher.pushDocument(item, url, organisation, apiKey, sourceName)
        pusher.toggleStatus('IDLE', organisation, apiKey, sourceName)



