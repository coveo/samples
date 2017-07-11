#Constants
FIELD_VALUE_EQUAL_TRUE = '1'
FIELDS_TO_HIDE_DELIMITER = ';'
REMOVED_PREVIEW_HTML_REMPLACEMENT = '<html><head></head><body><h1 style="color:rgb(150,0,0); font-family:sans-serif">Restricted</h1><p style="font-family:sans-serif; color:rgb(150,0,0)">Please log in to access the full document</p></body></html>'
REMOVE_PREVIEW_METADATA_KEY = 'remove preview'
REMOVE_FIELDS_METATADA_KEY = 'remove fields'
ENABLE_SEARCH_IN_REMOVED_FIELD_METADATA_KEY = 'enable search in removed fields'
ENABLE_SEARCH_IN_REMOVED_PREVIEW_METADATA_KEY = 'enable search in removed preview'
REMOVED_FIELDS_NAME_METADATA_KEY = 'removed fields name'
ITEM_TITLE_METADATA_KEY = 'title'
ITEM_IS_A_COPY_METADATA_KEY = 'item is a copy'

#Variables
IsFieldRemoverEnabled = ''
EnableSearchInRemovedField = ''
IsPreviewRemoverEnabled = ''
EnableSearchInRemovedPreview = ''
FieldsToHide = ''
HiddenButSearchableFieldsValue = ''

def GetMetaDataStringValue( meta_data_key ):
    return ''.join(document.get_meta_data_value(meta_data_key))

def GetParametersFromItemMetaData():
    global IsFieldRemoverEnabled
    global EnableSearchInRemovedField
    global IsPreviewRemoverEnabled
    global EnableSearchInRemovedPreview
    global FieldsToHide
    IsFieldRemoverEnabled = GetMetaDataStringValue(REMOVE_FIELDS_METATADA_KEY) == FIELD_VALUE_EQUAL_TRUE
    EnableSearchInRemovedField = GetMetaDataStringValue(ENABLE_SEARCH_IN_REMOVED_FIELD_METADATA_KEY) == FIELD_VALUE_EQUAL_TRUE
    IsPreviewRemoverEnabled = GetMetaDataStringValue(REMOVE_PREVIEW_METADATA_KEY) == FIELD_VALUE_EQUAL_TRUE
    EnableSearchInRemovedPreview = GetMetaDataStringValue(ENABLE_SEARCH_IN_REMOVED_PREVIEW_METADATA_KEY) == FIELD_VALUE_EQUAL_TRUE
    FieldsToHide = GetMetaDataStringValue(REMOVED_FIELDS_NAME_METADATA_KEY)
    log('This item is a copy and will be restricted: {} ' .format(GetMetaDataStringValue(ITEM_TITLE_METADATA_KEY)))

def RemoveFieldsFromItem():
    log('Removing fields')
    FieldsToHideList = FieldsToHide.split(FIELDS_TO_HIDE_DELIMITER)
    HiddenButSearchableFieldsValue = []
    for FieldToHide in FieldsToHideList:
        if EnableSearchInRemovedField :
            log('Set the field {} as searchable' .format(FieldsToHide))
            HiddenButSearchableFieldsValue.extend(GetMetaDataStringValue(FieldToHide))
        document.add_meta_data({FieldToHide:''})
    document.add_meta_data({'removedfieldcontent': ' '.join(HiddenButSearchableFieldsValue)})

def RemovePreviewFromItem():
    log('Removing HTML preview')
    html = document.DataStream('body_html')
    html.write(REMOVED_PREVIEW_HTML_REMPLACEMENT)
    document.add_data_stream(html)
    if not EnableSearchInRemovedPreview:
        text = document.DataStream('body_text')
        text.write('')
        document.add_data_stream(text)
    else:
        log('Keeping the item\'s body text for search purposes')

ItemIsACopy = GetMetaDataStringValue(ITEM_IS_A_COPY_METADATA_KEY) == FIELD_VALUE_EQUAL_TRUE

if ItemIsACopy:    
    GetParametersFromItemMetaData()
    if IsFieldRemoverEnabled:
        RemoveFieldsFromItem()
    if IsPreviewRemoverEnabled:
        RemovePreviewFromItem()