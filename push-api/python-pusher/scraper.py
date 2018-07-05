import os
import json
import parameters
import pusher
import config

SCRAPER_PARAM = parameters.SCRAPER_PARAM()
SCRAPER = config.SCRAPER()

class Scraper():
    # Loop through all the .json files of a local folder
    def pushAllDocs(self):
        root = SCRAPER.FOLDER
        filetype = SCRAPER_PARAM.FILE_TYPE
        for filename in os.listdir(root):
            if filename.endswith(filetype):
                file = open(root + filename, encoding = SCRAPER_PARAM.ENCODING)
                jsonFile = json.loads(file.read())
                pusher.Pusher.pushDocument(jsonFile, jsonFile[SCRAPER_PARAM.UNIQUE_DOCUMENT_ID_NODE])
                file.close()
