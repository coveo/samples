class SCRAPER_PARAM():
    FILE_TYPE = ".json"
    ENCODING = "ISO-8859-1"
    UNIQUE_DOCUMENT_ID_NODE = "DocumentId"

class PUSH_API_PARAM():
    PUSH_API_URL = "push.cloud.coveo.com"
    PUSH_API_URL_VERSION = "/v1/"
    PUSH_API_URL_ORG = "organizations/"
    PUSH_API_URL_SOURCE = "/sources/"
    PUSH_API_URL_STATUS = "/status?statusType="
    PUSH_API_URL_DOCUMENT_ID = "/documents?documentId="
    HEADER_CONTENT_TYPE = "Content-Type"
    HEADER_CONTENT_TYPE_VALUE = "application/json"
    HEADER_AUTHORIZATION = "Authorization"
    REQUEST_TYPE_POST = "POST"
    REQUEST_TYPE_PUT =  "PUT"

class SOURCE_STATUS():
    REBUILD = "REBUILD"
    IDLE = "IDLE"

class MESSAGES():
    SOURCE_STATUS_CHANGE_SUCCESS = "Source status change: "
    SOURCE_STATUS_CHANGE_FAILED = "Source status change failed: "
    DOCUMENT_PUSH = "Document URI: "
    DOCUMENT_PUSH_FAILED = "Document push failed: "
    DOCUMENT_PUSH_SUCCESS = "Document pushed: "
    START_BATCH = "┌───────────────── Starting Batch Upload ────────────────┐"
    END_BATCH = "└──────── Batch upload completed in "
    BATCH_TIME_UNIT = " seconds ────────┘"