# JSON Document Scraper for Coveo Cloud Push-API
Scrape a local folder and push each document to Coveo Cloud

## Instruction
  - Configure the source and the organization in `config.py`
  - Run with the command `python3 run.py` when in this current folder in a terminal

## Required File Format
Each document must be a `.json`. The following `.json` template will result in a HTML file, fully searchable and previewable in Coveo Cloud:

```sh
{
    "DocumentId": "www.foo.com",
    "fieldMappedInCoveoCloud": "Foo Bar",
    "title": "Foo Is for the Bar",
    "Data": "<html><body><h1>My awesome HTML Page<\/h1><p class=\"my class\">This will be fully searchable in Coveo Cloud<\/p><\/body><\/html>",
    "FileExtension": ".html"
}
```
To add metadata, simply create nodes in your JSON file (like the `"fieldMappedInCoveoCloud"`). Create a field in Coveo Cloud Organization to store the data, and map it to the `%[fieldMappedInCoveoCloud]` metadata.