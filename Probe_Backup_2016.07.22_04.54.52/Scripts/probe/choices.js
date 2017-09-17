$(function () {
    // Reference the auto-generated proxy for the hub (SignalR CONNECTIONS)
    var notifyhub = $.connection.notifyHub; //THE REFERENCE MUST BE IN CAMEL CASE (SEE TUTUORIAL)
    // Create a function that the hub can call back to display messages.
    notifyhub.client.ChoiceChangeNotification = function () {
        console.log('Notification Received!');
        SyncServerData();
    };
    // Start the connection.
    $.connection.hub.start().done(function () {
        console.log("Connection Hub Start");
    });
});

/*
    COMMON VARIABLES/TYPES
*/
var PLAYER_NOT_NAMED = "NO-NAME";
var ACLType = { "All": 0, "Private": 1, "Global": 2 };

/*
FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

function CorrectText(CorrectInd) {
    html = 'No';
    if (CorrectInd) {
        html = 'Yes';
    }

    return html;
}

/*
function template for Grid - Active
*/
function DisplayCorrect(choice) {
    return CorrectText(choice.Correct);
}//function DisplayCorrect(player) {

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/


/*
OnGridDataBound event for MyChoices Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    grid = $("#MyChoicesGrid").data("kendoGrid");
    gridViewLevel = grid.dataSource.view();

    StyleGridView(gridViewLevel);

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
        console.log('e.model.bind change event begin');

        StyleGridCommandRow(this.uid);
        console.log('e.model.bind change event end');
    });

    //This makes the textbox inline as wide as the column for editing
    $('.k-textbox').css('max-width', '1000px');
    $('.k-textbox').css('width', '100%');

    if (e.model.isNew()) {

        e.model.ChoiceQuestionId = questionId;
        
        //let's set the order nbr to the max + 1
        orderNbrTB = e.container.find('#OrderNbr');
        orderNbrTB.val(grid.dataSource.aggregates().OrderNbr.max + 1);
        e.model.OrderNbr = grid.dataSource.aggregates().OrderNbr.max + 1;

    } else {

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
    choiceGrid = $('#MyChoicesGrid').data('kendoGrid');
    choiceName = choiceGrid.dataItem('[data-uid="' + rowUID + '"]').Name;

    //prepare and open confirmation dialog
    message1 = 'You are about to delete the choice <span style="font-style: italic;font-weight:bold">' + choiceName + '.</span>';
    message2 = '<span style="font-weight:bold">Are you sure?</span>';
    ShowGeneralDialog(wndGen, 'Delete Question', message1, message2, true, 'OK', true, 'Cancel');
    $("#yesGen").click(function () {
        grid.removeRow(row);
        wndGen.close();
    });

    $("#noGen").click(function () {
        wndGen.close();
    });
}//function openDeleteConfirm(e)

function openPreview() {

    quesDetailfilter = ({ field: "Id", operator: "equals", value: questionId });
    quesDetailsModel.dataSource.filter(quesDetailfilter);
    quesDetailsModel.dataSource.read(); //invoke the read transport of the DataSource

    wndDetails.center().open();
}//function openPreview(e)

function SyncServerData() {
    console.log('SyncServerData');
    gridDataSource = $("#MyChoicesGrid").data("kendoGrid").dataSource;
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

            addButton =  $('.k-grid-add');
            editButton = $(currentRow).find(".k-grid-edit:eq(0)");
            deleteButton = $(currentRow).find(".k-grid-Delete");

            if (currentDataItem.ACLId == ACLType.Global) {
                editButton.hide();
                deleteButton.hide();
                addButton.hide();
            }

        }//if (currentDataItem != undefined) {

    }//if (currentDataItem != undefined) {
}//StyleGridCommandRow

function DisplayQuestionDetails(data) {

    if (Object.keys(data).length > 0) {

        html = '<p style="font-weight:bolder">' + data.Text + '?</p>' +
               '<div style="position:relative;float:left">';

        for (i = 0; i < data.Choices.length; i++) {
            if (data.Choices[i].Correct) {
                html += '<input type="radio" name="choices" value="' + i +
                    ' style="text-align:left;width:30px" checked disabled /><span style="font-size:15px;margin-left:2px">' +
                    data.Choices[i].Text + '</span><br />';
            } else {
                html += '<input type="radio" name="choices" value="' + i +
                    ' style="text-align:left;width:30px" disabled /><span style="font-size:15px;margin-left:2px">' +
                    data.Choices[i].Text + '</span><br />';
            }
        }

        html += '</div>';
    } else {
        html = '<p style="font-weight:bolder">No question selected</p>'
    }
    return html;
}//function DisplayQuestionDetails(data)

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
function Onsave(e) {
    console.log('Onsave event');
}
function Onsavechanges() {
    console.log('Onsavechanges event');
}

var wndGen;

$(document).ready(function () {

    var grid = $("#MyChoicesGrid").data("kendoGrid"); //NEED THIS GLOBAL GRID VAR

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
    Supporting the Details Dialog
    */
    wndDetails = $("#dialog-preview").kendoWindow({
        title: "Question Preview",
        modal: true,
        visible: false,
        resizable: true,
        draggable: true,
        minWidth: 250,
        maxWidth: 500
    }).data("kendoWindow");

    //sets up handler for Question Preview button
    $('.k-grid-Preview').click(openPreview);

    /*
    QUESTION DETAIL VIEW
    */

    quesDetailsModel = kendo.observable({
        //create a dataSource
        dataSource: new kendo.data.DataSource({
            "type": (function () { if (kendo.data.transports['aspnetmvc-ajax']) { return 'aspnetmvc-ajax'; } else { throw new Error('The kendo.aspnetmvc.min.js script is not included.'); } })()
            , transport: {
                read: {
                    url: "/ChoiceQuestions/GetQuestionDetails/"
                },
            }
            , "serverPaging": true
            , "serverSorting": true
            , "serverFiltering": true
            , "serverGrouping": true
            , "serverAggregates": true
            , batch: true,
            pageSize: 20,
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Id: { "type": "number" }
                      , Name: { "type": "string" }
                      , Text: { "type": "string" }
                    }
                }
            }
        })//datasource
    });

    quesDetailsModel.bind("change", function (e) {
        var template = kendo.template("#= DisplayQuestionDetails(data) #"); //create a template
        var result = template(e.items[0]); //Pass the data to the compiled template
        $("#questionDetailsView").html(result);
    });

    /*
    END OF QUESTION DETAIL VIEW
    */


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

}); //$(document).ready(function () {

