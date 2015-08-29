var PLAYER_NOT_NAMED = "NO-NAME";

/*
FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

function DateTypeGText(dataTypeG) {

    dataTypeGText = "";
    switch (dataTypeG) {
        case 0:
            dataTypeGText = 'TEXT';
            break;
        case 1:
            dataTypeGText = 'INTEGER';
            break;
        case 2:
            dataTypeGText = 'REAL';
            break;
        case 3:
            dataTypeGText = 'BOOLEAN';
            break;
    }

    return dataTypeGText;
}

/*
function template for Grid - GameType
*/
function DisplayDataTypeG(gameConfig) {
    return DateTypeGText(gameConfig.DataTypeG);
}//function displayGameType(game) {

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

/*
Called when GameConfigAutoComplete control value has changed. This function
will get the value and then push a new filter into the MyGameConfigs grid.
*/
function GameConfigAutocompleteChange() {
    gameConfigFilterValue = $("#GameConfigAutoComplete").data("kendoAutoComplete").value();

    var gridListFilter = { filters: [] };
    var gridDataSource = $("#MyGameConfigsGrid").data("kendoGrid").dataSource;

    /*
    We are going to look for a match between any of the names that represent the player
    */
    gridListFilter.logic = "and";   // a different logic 'or' can be selecteds
    if ($.trim(gameConfigFilterValue).length > 0) {
        gridListFilter.filters.push({ field: "Name", operator: "eq", value: gameConfigFilterValue });
    }

    gridDataSource.filter(gridListFilter);
    gridDataSource.read();

}//function GameConfigAutocompleteChange() {

/*
OnGridDataBound event for MyGameConfigs Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    grid = $("#MyGameConfigsGrid").data("kendoGrid");
    gridViewLevel = grid.dataSource.view();

    StyleGridView(gridViewLevel);

    //Repair the grid header, if it needs it. Hack for a grouping bug
    RepairGridHeader("MyGameConfigsGrid");

    //saveGridOptions(grid); //Save/Restore may have been causing Value column to be missing

}//function OnGridDataBound() {

/*
Setup Grid Editable Popup that supports both Create and Edit modes.
1. Set title
2. Set action button text
3. Show/Hide pivot controls based on create or edit mode
4. Set initial values
*/
function OnGridEdit(e) {
    console.log("OnGridEdit");
    //UI logic for Edit Popup
    //set the title based on add or delete
    //set the update button text (create or update)
    //decide to show or hide a number of game stats

    e.model.bind("change", function (v) {
        console.log('e.model.bind event begin');

        StyleGridCommandRow(this.uid);
        console.log('e.model.bind event end');
    });
    
    if (e.model.isNew()) {

    //THIS SCENARIO (NEW) SHOULD NOT HAPPEN

    } else {

        //Set Popup title, action button, and Fields
        $(".k-window-title").text("Edit Configuration");
        $('.k-grid-update').text("Update");

        $("#DataTypeG").kendoDropDownList({ 
            "dataSource": { "transport": { "read": { "url": "/GameConfigurations/GetDataTypeGs" }
                , "prefix": "" }
                , "schema": { "errors": "Errors" } }
            , "dataTextField": "Text"
            , "dataValueField": "Value"
            , "optionLabel": "Select Type..."
        });
    }

}//function OnGridEdit(e) {

/*
SUPPORT COMMAND EVENT HANDLERS FOR GRID
*/
function openDetails(e) {

    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    wndDetails.title("Configuration Details");
    wndDetails.content(detailsTemplate(dataItem));
    wndDetails.center().open();
}//function openDetails(e)
function openEdit(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gcGrid = $('#MyGameConfigsGrid').data('kendoGrid');
    gcId = gcGrid.dataItem('[data-uid="' + rowUID + '"]').Id;
    gcName = gcGrid.dataItem('[data-uid="' + rowUID + '"]').Name;

    url = root + 'GameConfigurations/GameConfiguration/' + gcId;
    //The GameConfigurations page requires the https:// for some reason
    if (url.indexOf('https') == -1) {
        url = 'https://' + url;
    }

    iframeHtml = '<iframe id="modalIframeId" width="100%" height="99%" marginWidth="0" marginHeight="0" ' +
                 'frameBorder="0" scrolling="yes"/>';

    dHeight = 620;

    $("#dialog-iframe").html(iframeHtml);
    wndIframe.title('Configuration - ' + gcName);
    wndIframe.setOptions({
        height: dHeight,
        width: 1300,
        position: {
            top: 25,
            left: 25
        }
    });

    wndIframe.bind("close", function () {
        SyncServerData();
    });

    wndIframe.open();

    $("#modalIframeId").attr("src", url);


}//function openEdit(e)


/*
 FUNCTIONS THAT WILL SUPPORT THE EVENT HANDLERS
*/
function ShowGeneralDialog(aWnd, aTitle, message1, message2, okInd, okText, closeInd, closeText) {

    aWnd.title(aTitle);

    $('#dialog-generalMessage').html(message1);
    $('#dialog-generalMessage2').html(message2);

    aWnd.center().open();

    if (okInd) {
        $("#yesGen").show();
        $("#yesGen").html(okText);
    } else {
        $("#yesGen").hide();
    }

    if (closeInd) {
        $("#noGen").show();
        $("#noGen").html(closeText);
    } else {
        $("#noGen").hide();
    }

}//function ShowGeneralDialog

function saveGridOptions(grid) {
    localStorage["GameConfigsGridOptions"] = kendo.stringify(grid.getOptions());
    localStorage["GameConfigAutoComplete"] = $("#GameConfigAutoComplete").data("kendoAutoComplete").value();
};//function saveGridOptions(grid) {

function restoreGridOptions(grid) {
    var options = localStorage["GameConfigsGridOptions"];
    if (options) {

        optionsJSON = JSON.parse(options);

        //NEED TO ADD THIS
        //Need to refresh the commands because they are not persisted since they were serialized via the server (MVC helper)
        commandCol = {
            "attributes": { "class": "gameconfigsGridCommandColumn" }
            , "command":
               [{ "name": "edit", "buttonType": "ImageAndText", "text": "Edit" }
             ,{ "name": "Details", "buttonType": "ImageAndText", "text": "Details", "click": openDetails }]
        };

        optionsJSON.columns.splice(4);
        optionsJSON.columns.push(commandCol);

        grid.setOptions(optionsJSON);

    }

    if (localStorage["GameConfigAutoComplete"] != undefined) {
        if (localStorage["GameConfigAutoComplete"] != "") {
            $("#GameConfigAutoComplete").data("kendoAutoComplete").value(localStorage["GameConfigAutoComplete"]);
        }
    }

};//function restoreGridOptions() {

/* Supporting Message Summary (for EDIT Popup) - Top of Create/Edit popup*/
function MyErrorHandler(args) {
    console.log('MyErrorHandler');
    if (args.errors) {
        var grid = $("#MyGameConfigsGrid").data("kendoGrid");
        var validationTemplate = kendo.template($("#SummaryValidationMessageTemplate").html());
        grid.one("dataBinding", function (e) {
            e.preventDefault();

            grid.editable.element.find(".errors").html(''); //let's clear the validation summary

            if (IsGeneralMessage(args.errors)) {
                var renderedTemplate = validationTemplate({ messages: args.errors[""].errors });
                grid.editable.element.find(".errors").append(renderedTemplate);
            }
        });

        PopulateInlineMessages(grid, args);

    }//if (args.errors)
}//function MyErrorHandler(args)

function IsGeneralMessage(errors) {
    isGeneralMessage = false;

    if (errors[""] != undefined) isGeneralMessage = true;
    return isGeneralMessage;
}

/*Supports the Inline Messages for MyGameConfigs Edit Popup attached to the Fields of the Edit Popup*/
var validationMessageTmpl = kendo.template($("#InLineMessage").html());

function PopulateInlineMessages(grid, args) {
    for (var error in args.errors) {
        showMessage(grid.editable.element, error, args.errors[error].errors);
    }
}//function PopulateInlineMessages(grid,args)

function showMessage(container, name, errors) {
    //add the validation message to the form
    container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]")
    .replaceWith(validationMessageTmpl({ field: name, message: errors[0] }))

    container.find("[data-valmsg-for=" + name + "],[data-val-msg-for=" + name + "]").click(function () {
        $(this).hide();
    });

}

/*End of Support for Inline Messages*/

function SyncServerData() {
    console.log('SyncServerData');
    gridDataSource = $("#MyGameConfigsGrid").data("kendoGrid").dataSource;
    gridDataSource.read();

}//SyncServerData()

/*
Recursive function that styles the grid based on conditions of the
row data. The reason its recursive is because when the grid is grouped
the datasource becomes hierarchical just like the grid. That's just the
way it is.
*/
function StyleGridView(gridViewLevel) {

    for (var i = 0; i < gridViewLevel.length; i++) {

        if (gridViewLevel[i].field == undefined) {
            //if field property is undefined than we have reached the level
            //of view data items.And we can go style the row
            currentUid = gridViewLevel[i].uid;
            StyleGridCommandRow(currentUid);
        } else {
            childGridViewLevel = gridViewLevel[i].items;
            StyleGridView(childGridViewLevel);
        }

    }//for (var i = 0; i < gridViewLevel.length; i++) {

}//function StyleGridView(gridViewLevel) {


function StyleGridCommandRow(uid) {
    currentRow = grid.table.find("tr[data-uid='" + uid + "']");
    currentDataItem = grid.dataSource.getByUid(uid);

    //We don't style the current row if there is nothing selected. Current row = []
    if (currentDataItem != undefined) {

        currentRow = grid.table.find("tr[data-uid='" + uid + "']");
        currentDataItem = grid.dataSource.getByUid(uid);

        //We don't style the current row if there is nothing selected. Current row = []
        if (currentDataItem != undefined) {

            //We only have the edit button to manage with game config
            editButton = $(currentRow).find(".k-grid-edit:eq(0)");

            if (gameEditable != 'True') {
                editButton.hide();
            }
        }//if (currentDataItem != undefined) {

    }//if (currentDataItem != undefined) {
}//StyleGridCommandRow

/*
MNS DEBUG
*/
function Oncancel() {
    console.log('Oncancel event');
}
function Onchange() {
    console.log('Onchange event');
}
function OncolumnHide() {
    console.log('OncolumnHide event');
}
function OncolumnMenuInit() {
    console.log('OncolumnMenuInit event');
}
function OncolumnReorder() {
    console.log('OncolumnReorder event');
}
function OncolumnResize() {
    console.log('OncolumnResize event');
}
function OncolumnShow() {
    console.log('OncolumnShow event');
}
function OndataBinding(e) {
    console.log('OndataBinding event');
}
function OndetailExpand() {
    console.log('OndetailExpand event');
}
function OndetailCollapse() {
    console.log('OndetailCollapse event');
}
function OndetailInit() {
    console.log('OndetailInit event');
}
function OnfilterMenuInit() {
    console.log('OnfilterMenuInit event');
}
function Onremove() {
    console.log('Onremove event');
}
function Onsave() {
    console.log('Onsave event');
}
function Onsavechanges() {
    console.log('Onsavechanges event');
}

var wndGen;

$(document).ready(function () {

    grid = $("#MyGameConfigsGrid").data("kendoGrid");

    /*
    Supporting the Delete Confirmation
    */
    wndGen = $("#dialog-general").kendoWindow({
        title: "A title",
        modal: true,
        visible: false,
        resizable: false,
        width: 300
    }).data("kendoWindow");

    wndProgress = $("#dialog-progress").kendoWindow({
        title: "In Progress. Please Wait ...",
        modal: true,
        visible: false,
        resizable: false,
        width: 250
    }).data("kendoWindow");

    $("#progressbar").kendoProgressBar({
        type: "chunk",
        max: 100,
        chunkCount: 10,
        value: 50
    });

    /*
    Supporting the iFrame Dialog
    */
    wndIframe = $("#dialog-iframe").kendoWindow({
        title: "IFrame Preview",
        modal: true,
        visible: false,
        resizable: true,
        draggable: true
    }).data("kendoWindow");

    /*
    Supporting the Details Dialog
    */
    wndDetails = $("#dialog-details").kendoWindow({
        title: "Details",
        modal: true,
        visible: false,
        resizable: true,
        draggable: true,
        minWidth: 160,
        maxWidth: 500
    }).data("kendoWindow");
    detailsTemplate = kendo.template($("#detailsTemplate").html());

    /*
     Add bells and whistles to MyGameConfigs autocomplete textbox (including the search magnifying glass)
    */
    $('#GameConfigAutoComplete').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#GameConfigAutoComplete').parent().css('width', '300px'); //this will increase the Autocomplete Textbox size

    grid.bind("cancel", Oncancel);
    grid.bind("change", Onchange);
    grid.bind("columnHide", OncolumnHide);
    grid.bind("columnMenuInit", OncolumnMenuInit);
    grid.bind("columnReorder", OncolumnReorder);
    grid.bind("columnResize", OncolumnResize);
    grid.bind("columnShow", OncolumnShow);
    grid.bind("dataBinding", OndataBinding);
    grid.bind("detailExpand", OndetailExpand);
    grid.bind("detailCollapse", OndetailCollapse);
    grid.bind("detailInit", OndetailInit);
    grid.bind("filterMenuInit", OnfilterMenuInit);
    grid.bind("remove", Onremove);
    grid.bind("save", Onsave);
    grid.bind("savechanges", Onsavechanges);

    //Will restore Grid configuration/options/command button event handlers - every time the MyGameConfigs Index page is called
    //restoreGridOptions(grid); //Save/Restore may have been causing Value column to be missing

}); //$(document).ready(function () {

