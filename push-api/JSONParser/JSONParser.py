import Parser
import yamlParser
if __name__ == "__main__":
    config = yamlParser.yamlParser.readConfig()
    parser = Parser.Parser
    parser.readDocument(config)


