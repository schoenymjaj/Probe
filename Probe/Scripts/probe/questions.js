
$(function () {
    // Reference the auto-generated proxy for the hub (SignalR CONNECTIONS)
    var notifyhub = $.connection.notifyHub; //THE REFERENCE MUST BE IN CAMEL CASE (SEE TUTUORIAL)
    // Create a function that the hub can call back to display messages.
    notifyhub.client.QuestionChangeNotification = function () {
        console.log('Notification Received!');
        SyncServerData();
    };
    // Start the connection.
    $.connection.hub.start().done(function () {
        console.log("Connection Hub Start");
    });
});

/*
FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

/*
Get question type text from the quetion type id
*/
function QuestionTypeText(questionTypeId) {
    return 'Multi-Select';
}

function TestEnabledText(testEnabledInd) {
    html = 'No';
    if (testEnabledInd) {
        html = 'Yes';
    }

    return html;
}

/*
function template for Grid - QuestionType
*/
function DisplayQuestionType(question) {
    return QuestionTypeText(question.QuestionTypeId);
}//function DisplayQuestionType(question) {

/*
function template for Grid - TextEnabled
*/
function DisplayTestEnabled(question) {
    html = TestEnabledText(question.TestEnabled);
    return html;
}//function DisplayPublished(question) {

function DisplayChoiceCount(question) {

    viewTypeDDL = $('#ViewDropdownList').data('kendoDropDownList');

    switch (viewTypeDDL.value()) {
        case 'Compact View':
            html = question.ChoicesCount;
            break;
        case 'Full View':
            //check to see if there is a question yet to display (could be in Add new question mode)
            if (question.Id != 0 && question.Name != "") {
                html = '<div class="backimageInCommon backCoverInCommon">' + DisplayQuestionDetails(question) + '</div>';
            }
            break;
    }

    return html;
}

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

/*
Called when QuestionAutoComplete control value has changed. This function
will get the value and then push a new filter into the MyQuestions grid. The filter will just show
every question that equals the autocompleted selected question name.
*/
function QuestionAutocompleteChange() {
    questionFilterValue = $("#QuestionAutoComplete").data("kendoAutoComplete").value();

    var gridListFilter = { filters: [] };
    var gridDataSource = $("#MyQuestionsGrid").data("kendoGrid").dataSource;

    gridListFilter.logic = "and";   // a different logic 'or' can be selected
    if ($.trim(questionFilterValue).length > 0) {
        gridListFilter.filters.push({ field: "Name", operator: "eq", value: questionFilterValue });
    }

    gridDataSource.filter(gridListFilter);
    gridDataSource.read();

}//function QuestionAutocompleteChange() {

/*
OnGridDataBound event for MyQuestions Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    //BIND CHOICES COUNT COLUMN CLICK WITH CHOICE UI
    $('.choicesDialogLink').click(function () {
        //NOT IMPLEMENTED YET
    });//$('#choicesDialogLink').click

    grid = $("#MyQuestionsGrid").data("kendoGrid");
    gridViewLevel = grid.dataSource.view();

    StyleGridView(gridViewLevel);

    //Repair the grid header, if it needs it. Hack for a grouping bug
    RepairGridHeader("MyQuestionsGrid");

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
    //decide to show or hide a number of question stats

    e.model.bind("change", function (v) {
        console.log('e.model.bind event begin');

        StyleGridCommandRow(this.uid);
        console.log('e.model.bind event end');
    });

    if (e.model.isNew()) {

        //Set Popup title, action button, and Fields
        $(".k-window-title").text("Add Question");
        $('.k-grid-update').text("Create");

        //Set Popup initial values
        questionTypeDDL = e.container.find('#QuestionTypeId').data('kendoDropDownList');
        questionTypeDDL.value("1"); //multi-select
        e.container.find('#QuestionTypeId').data('kendoDropDownList').trigger("change");


    } else {

        //Set Popup title, action button, and Fields
        $(".k-window-title").text("Edit Question");
        $('.k-grid-update').text("Update");

    }

}//function OnGridEdit(e) {

/*
View Dropdown Change Handler. First we change the 5th column's name to 
an appropriate title; then we will refresh the Question Grid
*/
function OnViewDrpDownChange(e) {
    grid = $("#MyQuestionsGrid").data("kendoGrid");

    gridOptions = grid.getOptions();

    viewTypeDDL = $('#ViewDropdownList').data('kendoDropDownList');
    switch (viewTypeDDL.value()) {
        case 'Compact View':
            gridOptions.columns[4].title = "Choice Count";
            break;
        case 'Full View':
            gridOptions.columns[4].title = "Question Preview";
            break;
    }

    grid.setOptions(gridOptions);
    grid.refresh();
}

function OnQuestionSearchFilterChange(value) {

    existingContainsFilterValue = 'NoContainsFilterBluePrint';

    if (existingContainsFilterValue != value) {
        grid = $("#MyQuestionsGrid").data("kendoGrid");

        grid.dataSource.read();
        grid.dataSource.page(1);;

    }//if (existingFilterValue != value) {

}//function OnQuestionSearchFilterChange(value) {

/*
SUPPORT COMMAND EVENT HANDLERS FOR GRID
*/
function openDeleteConfirm(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    questionGrid = $('#MyQuestionsGrid').data('kendoGrid');
    questionName = questionGrid.dataItem('[data-uid="' + rowUID + '"]').Name;

    //prepare and open confirmation dialog
    message1 = 'You are about to delete the question <span style="font-style: italic;font-weight:bold">' + questionName + '.</span>';
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
function openDetails(e) {

    e.preventDefault();

    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    wndDetails.title("Question Details");
    wndDetails.content(detailsTemplate(dataItem));
    wndDetails.center().open();
}//function openDetails(e)
function CloneNow(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    questionGrid = $('#MyQuestionsGrid').data('kendoGrid');
    questionId = questionGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    $.getJSON('ChoiceQuestions/Clone/' + questionId, {},
    function (data) {

        if (data.MessageId == 23) SyncServerData(); //successful clone - we need to sync question data
        //prepare and open informational dialog for clone action
        ShowGeneralDialog(wndGen, 'Clone Question', data.Message, '', false, '', true, 'Close');
        $("#noGen").click(function () {
            wndGen.close();
        });

    });//post
}//function CloneNow(e) {
function openChoices(e) {

    OpenProgressBarWindow();

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid'); 
    questionGrid = $('#MyQuestionsGrid').data('kendoGrid');
    questionId = questionGrid.dataItem('[data-uid="' + rowUID + '"]').Id;
    url = root + 'Choices/Index/' + questionId;
    if (url.indexOf('https') == -1) { //this url needs an https prefix for some reason
        url = 'https://' + url;
    }

    window.location = url;
}
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
    localStorage["QuestionsGridOptions"] = kendo.stringify(grid.getOptions());
    localStorage["QuestionAutoComplete"] = $("#QuestionAutoComplete").data("kendoAutoComplete").value();
    localStorage["QuestionSearchFilter"] = $("#QuestionSearchFilter").val();

    viewTypeDDL = $('#ViewDropdownList').data('kendoDropDownList');
    localStorage["QuestionsGridView"] = viewTypeDDL.value();

};//function saveGridOptions(grid) {

function restoreGridOptions(grid) {
    var options = localStorage["QuestionsGridOptions"];
    if (options) {

        optionsJSON = JSON.parse(options);

        //need to refresh the toolbar because they are not persisted since they are serialized via the server (MVC helper)
        optionsJSON.toolbar = [
            { template: $("#toolbarTemplate").html() }
        ];
        optionsJSON.columns[0].headerTemplate = $("#headerTemplate").html();

        //NEED TO ADD THIS
        //Need to refresh the commands because they are not persisted since they were serialized via the server (MVC helper)
        commandCol = {
            "attributes": { "class": "questionsGridCommandColumn" }
            ,"command":
                [{ "name": "edit", "buttonType": "ImageAndText", "text": "Edit" }
             , { "name": "Details", "buttonType": "ImageAndText", "text": "Details", "click": openDetails }
             , { "name": "Choices", "buttonType": "ImageAndText", "text": "Choices", "click": openChoices }
             , { "name": "Clone", "buttonType": "ImageAndText", "text": "Clone", "click": CloneNow }
             , { "name": "Delete", "buttonType": "ImageAndText", "text": "Delete", "click": openDeleteConfirm }]
        };

        optionsJSON.columns.splice(5);
        optionsJSON.columns.push(commandCol);

        grid.setOptions(optionsJSON);

    }

    //restore the question autocomplete control
    if (localStorage["QuestionAutoComplete"] != undefined) {
        if (localStorage["QuestionAutoComplete"] != "") {
            $("#QuestionAutoComplete").data("kendoAutoComplete").value(localStorage["QuestionAutoComplete"]);
        }
    }

    //restore the question search control
    if (localStorage["QuestionSearchFilter"] != undefined) {
        if (localStorage["QuestionSearchFilter"] != "") {
            questionSearchFilterValue = localStorage["QuestionSearchFilter"];
            $("#QuestionSearchFilter").val(questionSearchFilterValue);
            OnQuestionSearchFilterChange(questionSearchFilterValue);
        }
    }


    //restore the question grid view
    viewTypeDDL = $('#ViewDropdownList').data('kendoDropDownList');
    viewTypeDDL.value(localStorage["QuestionsGridView"]);

};//function restoreGridOptions() {

/* Supporting Message Summary (for EDIT Popup) - Top of Create/Edit popup*/
function MyErrorHandler(args) {
    console.log('MyErrorHandler');
    if (args.errors) {
        var grid = $("#MyQuestionsGrid").data("kendoGrid");
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

/*Supports the Inline Messages for MyQuestions Edit Popup attached to the Fields of the Edit Popup*/
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
    gridDataSource = $("#MyQuestionsGrid").data("kendoGrid").dataSource;
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

        //THERE ARE NO CONDITIONALS FOR THE QUESTION COMMANDS IN THE COMMAND COLUMN

    }
}//StyleGridCommandRow

function DisplayQuestionDetails(data) {

    if (Object.keys(data).length > 0) {

        html = '<p style="font-weight:bolder">' + data.Text + '?</p>' +
               '<div style="position:relative;float:left">';

        if (data.Choices != null) {

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
        }//if (data.Choices != null) {
        html += '</div>';
    } else {
        html = '<p style="font-weight:bolder">No question selected</p>'
    }
    return html;
}//function DisplayQuestionDetails(data)

function OpenProgressBarWindow() {

    wndProgress.title("In Progress. Please Wait ...");
    wndProgress.center().open(); //open progress window

    progressBar = $('#progressbar').data('kendoProgressBar')
    setTimeout(function () {
        newProgressBarValue = progressBar.value() + 10;
        progressBar.value(newProgressBarValue);
    }
    , 500);
}

function ReturnQuestionSearchHandler() {
    return {
        questionSearch: $('#QuestionSearchFilter').val()
    };
}

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

    grid = $("#MyQuestionsGrid").data("kendoGrid");

    $('#QuestionSearchFilter').keyup(function () {

        OnQuestionSearchFilterChange(this.value);

    });//$('#QuestionSearchFilter').keyup(function () {


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
    Supporting the Preview and Questions Dialog
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

    ////for debugging purposes - looking at grid datasource
    //$('#infoGet').click(function () {

    //    var gridData = $("#MyQuestionsGrid").data("kendoGrid")._data;

    //});//$('#infoGet').click(function () {

    ////for debugging purposes - looking at grid datasource
    //$('#infoSync').click(function () {

    //    SyncServerData();

    //});//$('#infoGet').click(function () {

    /*
     Add bells and whistles to Questions Search textbox (including the search magnifying glass)
    */
    $('#QuestionSearchFilter').parent().addClass("k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#QuestionSearchFilter').parent().css('width', '300px'); //this will increase the Autocomplete Textbox size
    $('#QuestionSearchFilter').css('height', '28px');
    $('#QuestionSearchFilter').attr('placeholder', 'Enter search words for questions ...');


    /*
     Add bells and whistles to Questions autocomplete textbox (including the search magnifying glass)
    */
    $('#QuestionAutoComplete').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#QuestionAutoComplete').parent().css('width', '300px'); //this will increase the Autocomplete Textbox size

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


    //Will restore Grid configuration/options/command button event handlers - every time the Questions Index page is called
    restoreGridOptions(grid);

}); //$(document).ready(function () {

