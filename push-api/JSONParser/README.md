# Crawl and Push

Scanning a folder and push every item to the Push-API

### Usage

The main file is JSONParser.py
It will execute a 4 step process:

1: read the config file (yamlParser.py)
2: scan a folder and create a JSON (Reader.py) (optional if docs.json is already populated)
3: parse the JSON file (Parser.py)
4: loop through the array in the JSON file and push (Pusher.py) info to the Push-API

### Requirement

Python 3.*
You will need to add the PyYAML lib
Before you run the script, fill all the nodes in the config.yml

### Installation

* The access token can be created from the Cloud Administration Console
* The source needs to be a PUSH-type source
* The array node is the key of an array in a JSON file which should contain all the documents to be indexed
* The url node is the key to look for in an individual item of the JSON documents array. If you need the url to be dynamic, you will need to add your logic to the Parser.readDocument method. Please be nice and add a new class for this logic :)

This is still an alpha project. There is error handling and/or any type of scaling strategy.
