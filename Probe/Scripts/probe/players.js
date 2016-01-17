var PLAYER_NOT_NAMED = "NO-NAME";

/*
FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

function ActiveText(ActiveInd) {
    html = 'No';
    if (ActiveInd) {
        html = 'Yes';
    }

    return html;
}

/*
function template for Grid - Active
*/
function DisplayActive(player) {
    return ActiveText(player.Active);
}//function DisplayActive(player) {

/*
Get sex type text from the sex
*/
function SexTypeText(sexTypeId) {

    sexTypeText = "";
    switch (sexTypeId) {
        case 0:
            sexTypeText = 'UNKNOWN';
            break;
        case 1:
            sexTypeText = 'MALE';
            break;
        case 2:
            sexTypeText = 'FEMALE';
            break;
    }

    return sexTypeText;
}

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

/*
Called when PlayerAutoComplete control value has changed. This function
will get the value and then push a new filter into the MyPlayers grid.
*/
function PlayerAutocompleteChange() {
    playerFilterValue = $("#PlayerAutoComplete").data("kendoAutoComplete").value();

    filterArray = playerFilterValue.split("-");

    var gridListFilter = { filters: [] };
    var gridDataSource = $("#MyPlayersGrid").data("kendoGrid").dataSource;

    /*
    We are going to look for a match between any of the names that represent the player
    */
    gridListFilter.logic = "or";   // a different logic 'or' can be selecteds
    if ($.trim(filterArray[0]).length > 0) {
        gridListFilter.filters.push({ field: "FirstName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[0]).length > 0) {
        gridListFilter.filters.push({ field: "NickName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[0]).length > 0) {
        gridListFilter.filters.push({ field: "LastName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[0]).length > 0) {
        gridListFilter.filters.push({ field: "EmailAddr", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[1]).length > 0) {
        gridListFilter.filters.push({ field: "FirstName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[1]).length > 0) {
        gridListFilter.filters.push({ field: "NickName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[1]).length > 0) {
        gridListFilter.filters.push({ field: "LastName", operator: "eq", value: filterArray[0] });
    }
    if ($.trim(filterArray[1]).length > 0) {
        gridListFilter.filters.push({ field: "EmailAddr", operator: "eq", value: filterArray[0] });
    }


    gridDataSource.filter(gridListFilter);
    gridDataSource.read();

}//function PlayerAutocompleteChange() {

/*
OnGridDataBound event for MyPlayers Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    grid = $("#MyPlayersGrid").data("kendoGrid");
    gridViewLevel = grid.dataSource.view();

    StyleGridView(gridViewLevel);

    //Repair the grid header, if it needs it. Hack for a grouping bug
    RepairGridHeader("MyPlayersGrid");

    saveGridOptions(grid);

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
        $(".k-window-title").text("Edit Player");
        $('.k-grid-update').text("Update");
    }

}//function OnGridEdit(e) {

/*
SUPPORT COMMAND EVENT HANDLERS FOR GRID
*/
function openDeleteConfirm(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    playerGrid = $('#MyPlayersGrid').data('kendoGrid');
    FirstName = playerGrid.dataItem('[data-uid="' + rowUID + '"]').FirstName;
    LastName = playerGrid.dataItem('[data-uid="' + rowUID + '"]').LastName;
    NickName = playerGrid.dataItem('[data-uid="' + rowUID + '"]').NickName;
    EmailAddr = playerGrid.dataItem('[data-uid="' + rowUID + '"]').EmailAddr;
    playerName = GetPlayerName(FirstName, LastName, NickName, EmailAddr);

    //prepare and open confirmation dialog
    message1 = 'You are about to delete the player <span style="font-style: italic;font-weight:bold">' + playerName + '.</span>';
    message2 = '<span style="font-weight:bold">Are you sure?</span>';
    ShowGeneralDialog(wndGen, 'Delete Player', message1, message2, true, 'OK', true, 'Cancel');
    $("#yesGen").click(function () {
        grid.removeRow(row);
        wndGen.close();
    });

    $("#noGen").click(function () {
        wndGen.close();
    });
}//function openDeleteConfirm(e)
function openDetails(e) {

    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    wndDetails.title("Player Details");
    wndDetails.content(detailsTemplate(dataItem));
    wndDetails.center().open();
}//function openDetails(e)

/*
 FUNCTIONS THAT WILL SUPPORT THE EVENT HANDLERS
*/

function saveGridOptions(grid) {
    localStorage["PlayersGridOptions"] = kendo.stringify(grid.getOptions());
    localStorage["PlayerAutoComplete"] = $("#PlayerAutoComplete").data("kendoAutoComplete").value();
};//function saveGridOptions(grid) {

function restoreGridOptions(grid) {
    var options = localStorage["PlayersGridOptions"];
    if (options) {

        optionsJSON = JSON.parse(options);

        //NEED TO ADD THIS
        //Need to refresh the commands because they are not persisted since they were serialized via the server (MVC helper)
        commandCol = {
            "attributes": { "class": "playersGridCommandColumn" }
            , "command":
               [{ "name": "edit", "buttonType": "ImageAndText", "text": "Edit" }
             ,{ "name": "Details", "buttonType": "ImageAndText", "text": "Details", "click": openDetails }
             , { "name": "Delete", "buttonType": "ImageAndText", "text": "Delete", "click": openDeleteConfirm }]
        };

        optionsJSON.columns.splice(6);
        optionsJSON.columns.push(commandCol);

        grid.setOptions(optionsJSON);

    }

    if (localStorage["PlayerAutoComplete"] != undefined) {
        if (localStorage["PlayerAutoComplete"] != "") {
            $("#PlayerAutoComplete").data("kendoAutoComplete").value(localStorage["PlayerAutoComplete"]);
        }
    }

};//function restoreGridOptions() {

function SyncServerData() {
    console.log('SyncServerData');
    gridDataSource = $("#MyPlayersGrid").data("kendoGrid").dataSource;
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

            editButton = $(currentRow).find(".k-grid-edit:eq(0)");
            detailsButton = $(currentRow).find(".k-grid-edit:eq(1)");
            deleteButton = $(currentRow).find(".k-grid-Delete");

            //Get rid of the pencil icon on the details button
            detailsButton.children().removeClass('k-icon');

            if (gameEditable != 'True') {
                editButton.hide();
                deleteButton.hide();
            }
        }//if (currentDataItem != undefined) {

    }//if (currentDataItem != undefined) {
}//StyleGridCommandRow

/*
get player name
*/
function GetPlayerName(firstName, nickName, lastName, email) {
    console.log('func GetPlayerName');

    //support backward compatibility. LastName and Email maybe undefined
    if (lastName == undefined) { lastName = "" };
    if (email == undefined) { email = "" };

    if (firstName != "" && lastName != "") {
        return firstName + '-' + lastName;
    } else if (firstName == "" && nickName != "" && lastName != "") {
        return nickName + '-' + lastName;
    } else if (firstName != "" && lastName == "" && nickName != "") {
        return firstName + '-' + nickName;
    } else if (firstName != "" && lastName == "" && nickName == "") {
        return firstName;
    } else if (firstName == "" && lastName != "" && nickName == "") {
        return lastName;
    } else if (firstName == "" && lastName == "" && nickName != "") {
        return nickName;
    } else if (firstName == "" && lastName == "" && nickName == "" && email != "") {
        return email; //last hope
    } else {
        return PLAYER_NOT_NAMED;
    }

    console.log('END app.GetPlayerName');
}//app.GetPlayerName


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

    var grid = $("#MyPlayersGrid").data("kendoGrid"); //NEED THIS GLOBAL GRID VAR

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
     Add bells and whistles to Players autocomplete textbox (including the search magnifying glass)
    */
    $('#PlayerAutoComplete').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#PlayerAutoComplete').parent().css('width', '300px'); //this will increase the Autocomplete Textbox size

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


    //Will restore Grid configuration/options/command button event handlers - every time the Players Index page is called
    restoreGridOptions(grid);

}); //$(document).ready(function () {

