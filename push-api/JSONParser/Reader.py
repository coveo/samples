import json
import os

class Reader():

# Parse every file and create a JSON
    def readFolder(config):
        root = config['basicInformation']['folder']
        filetype = config['basicInformation']['filetype']

        if filetype:
            docs = []
            result = {}

            print('Scanning folder ' + root)

            for filename in os.listdir(root):
                if filename.endswith('.' + filetype):
                    tags = {}
                    tags['url'] = 'http://' + filename
                    tags['title'] = filename
                    f = open(root + filename, encoding = 'ISO-8859-1')
                    tags['data'] = f.read()
                    f.close()
                    docs.append(tags)
            result['documents'] = docs

            with open('docs.json', 'w') as outfile:
                json.dump(result, outfile)

            print('Scan completed - JSON created')
