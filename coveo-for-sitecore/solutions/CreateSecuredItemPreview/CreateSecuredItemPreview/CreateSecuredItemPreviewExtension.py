#Const
FIELD_VALUE_EQUAL_TRUE = '1'
FIELDS_TO_HIDE_DELIMITER = ';'
STRING_EMPTY = ''
REMOVED_PREVIEW_HTML_REMPLACEMENT = '<html><head></head><body><h1 style="color:rgb(150,0,0); font-family:sans-serif">Restricted</h1><p style="font-family:sans-serif; color:rgb(150,0,0)">Please login to access the full document</p></body></html>'

#Check if the current item is a copy
ItemIsACopy = STRING_EMPTY.join(document.get_meta_data_value('itemisacopy'))

if ItemIsACopy == FIELD_VALUE_EQUAL_TRUE:
    log('This item is a copy and will be restricted')

    #Get CreateSecuredItemPreview parameters
    ActivateFieldRemover = STRING_EMPTY.join(document.get_meta_data_value('activatefieldremover'))
    ActivateRemovedFieldSearch = STRING_EMPTY.join(document.get_meta_data_value('activateremovedfieldsearch'))
    ActivatePreviewRemover = STRING_EMPTY.join(document.get_meta_data_value('activatepreviewremover'))
    ActivateRemovedPreviewSearch = STRING_EMPTY.join(document.get_meta_data_value('activateremovedpreviewsearch'))
    FieldsToHide = STRING_EMPTY.join(document.get_meta_data_value('fieldstoremove'))
    HiddenButSearchableFieldsValue = STRING_EMPTY
    
    #Field Remover
    if ActivateFieldRemover == FIELD_VALUE_EQUAL_TRUE:
        log('Removing fields')
        FieldsToHideList = FieldsToHide.split(FIELDS_TO_HIDE_DELIMITER)
        for FieldToHide in FieldsToHideList:
            if ActivateRemovedFieldSearch == FIELD_VALUE_EQUAL_TRUE:
                log('Make the field: ' + FieldToHide + ' searchable')
                HiddenButSearchableFieldsValue += STRING_EMPTY.join(document.get_meta_data_value(FieldToHide)) + ' '
            document.add_meta_data({FieldToHide:STRING_EMPTY})
        document.add_meta_data({'removedfieldcontent':HiddenButSearchableFieldsValue})

    #Preview Remover
    if ActivatePreviewRemover == FIELD_VALUE_EQUAL_TRUE:
        log('Removing preview')
        html = document.DataStream('body_html')
        html.write(REMOVED_PREVIEW_HTML_REMPLACEMENT)
        document.add_data_stream(html)
        if ActivateRemovedPreviewSearch != FIELD_VALUE_EQUAL_TRUE:
            text = document.DataStream('body_text')
            text.write('')
            document.add_data_stream(text)
        else:
            log('Making preview content searchable')            