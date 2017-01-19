import Parser
import Reader
import yamlParser

if __name__ == "__main__":
    config = yamlParser.yamlParser.readConfig()
    reader = Reader.Reader
    reader.readFolder(config)
    parser = Parser.Parser
    parser.readDocument(config)
    print('Operation complete')
