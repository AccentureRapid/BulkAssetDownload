var showButton = true;
var currentListItem;
var selectedListItemCollection;
var items;
var web;
var url;
var itemCount;
function ShowAssetDownloadPopUp(param) {

    try {
        var buttonStatus = document.getElementById('Ribbon.Documents.New.DownloadAssetButton-Large').getAttribute('class');
        if (buttonStatus == 'ms-cui-ctl-large ms-cui-disabled') {
            return;
        }

        var clientContext = SP.ClientContext.get_current();
        this.items = SP.ListOperation.Selection.getSelectedItems(clientContext);

        var selectedItems = '';
        var item;

        this.itemCount = 0;

        for (item in items) {
            selectedItems += '|' + items[item].id;
            this.itemCount = this.itemCount + 1;
        }

        if (this.itemCount == 0) {
            alert('Please select an asset to Download');
        }
        else {
            var options =
            {
                url: param + '/_layouts/BulkAssetDownload/ConfirmDownload.aspx?items=' + selectedItems + '&location=' + param + '&source=' + SP.ListOperation.Selection.getSelectedList(),
                title: 'Download Assets',
                allowMaximize: false,
                showClose: true,
                width: 505,
                height: 600,
                dialogReturnValueCallback: demoCallback
            };

            SP.UI.ModalDialog.showModalDialog(options);
        }
    }
    catch (err) {

    }
}

function demoCallback(dialogResult, returnValue) {
    //SP.UI.Notify.addNotification('Operation Successful!');
    //SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK);
}

function EnableAssetDownloadButton() {
    try {
        //HideTranscodeButton();
        var clientContext = SP.ClientContext.get_current();
        var items = SP.ListOperation.Selection.getSelectedItems(clientContext);
        var currentListId = SP.ListOperation.Selection.getSelectedList();
        var oWebsite = clientContext.get_web();

        var currentList = oWebsite.get_lists().getById(currentListId);

        var count = 0;
        var item;


        var idArray = new Array();
        for (item in items) {
            idArray[count] = items[item].id;
            count = count + 1;
        }

        this.itemCount = count;


        var camlQueryString = CreateCAMLQuery(idArray);

        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml(camlQueryString);
        this.selectedListItemCollection = currentList.getItems(camlQuery);
        clientContext.load(selectedListItemCollection);
        clientContext.executeQueryAsync(Function.createDelegate(this, onQuerySuccess), Function.createDelegate(this, onQueryFailed));



        if (count == 0) {
            return false;
        }

        //return showButton;
        return true;
    }
    catch (err) {
        return true;
    }
}

function onQuerySuccess(sender, args) {

    try {
        var listItemEnumerator = selectedListItemCollection.getEnumerator();
        var enumeratorCount = 0;

        while (listItemEnumerator.moveNext()) {

            enumeratorCount = enumeratorCount + 1;
            var listItem = listItemEnumerator.get_current();
            var itemType = -1;
            itemType = listItem.get_fileSystemObjectType();
            if (itemType != 0) {
                HideBulkAssetDownloadButton();
                return;
            }
        }


        if (enumeratorCount < this.itemCount) {
            HideBulkAssetDownloadButton();
            return;
        }

        if (enumeratorCount == 0) {
            HideBulkAssetDownloadButton();
        }
        else {
            ShowBulkAssetDownloadButton();
        }
    }
    catch (err) {
    }
}

function onQueryFailed(sender, args) {
    HideBulkAssetDownloadButton();
}


function ShowBulkAssetDownloadButton() {
    var buttonElement = document.getElementById('Ribbon.Documents.New.DownloadAssetButton-Large').setAttribute('class', 'ms-cui-ctl-large');
}

function HideBulkAssetDownloadButton() {
    var buttonElement = document.getElementById('Ribbon.Documents.New.DownloadAssetButton-Large').setAttribute('class', 'ms-cui-ctl-large ms-cui-disabled');
}

function CreateCAMLQuery(parameters) {
    var camlQuery = '';
    if (parameters.length == 0) {
        return '';
    }

    var i = 0;

    for (i = 0; i < parameters.length; i++) {

        camlQuery = camlQuery + '<Eq>';
        camlQuery = camlQuery + '<FieldRef Name="ID"/>';
        camlQuery = camlQuery + '<Value Type="Counter">' + parameters[i] + '</Value>';
        camlQuery = camlQuery + '</Eq>';


        if (i > 0) {
            camlQuery = '<Or>' + camlQuery;
            camlQuery = camlQuery + '</Or>';

        }
    }

    camlQuery = '<View Scope="Recursive"><Query><Where>' + camlQuery;
    camlQuery = camlQuery + '</Where></Query></View>';
    return camlQuery;
}
