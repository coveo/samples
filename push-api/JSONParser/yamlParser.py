import yaml

class yamlParser(object):

# Read config documents
    def readConfig():
        configFilePath = 'config.yml'
        stream = open(configFilePath)
        dataMap = yaml.safe_load(stream)
        stream.close()
        return dataMap
