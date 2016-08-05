### General usage:

The main file is JSONParser.py

You will need to add the PyYAML lib

Before you run the script, fill all the nodes in the config.yml

* The access token can be created from the Cloud Administration Console
* The source needs to be a PUSH-type source
* The url node is to find the item. If you need the url to be dynamic, you will need to add your logic to the Parser.readDocument method. Please be nice and add a new class for this logic :)
* The array is the key which should contain all the documents to be indexed

This is still an alpha project. There is error handling and/or any type of scaling strategy.