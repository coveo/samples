import yaml

class yamlParser(object):
    def __inti__(self):
        self.configFilePath = 'config.yml'

    def readConfig():
        configFilePath = 'config.yml'
        stream = open(configFilePath)
        dataMap = yaml.safe_load(stream)
        stream.close()
        return dataMap


