var quesDatasource;
var gameQuesDatasource;
var gameQuesSelect;
var quesDetailsModel
var showInstructions = true;
var isReadyToAddGQItem = true;
var MAX_LISTVIEW_ITEM_LENGTH = 60;
var LISTVIEW_ITEM_HEIGHT = 38;
var existingContainsFilterValue = 'NoContainsFilterBluePrint';
var draggablePadding = 10;

var wndGen; //make the general dialog handler global

$(document).ready(function () {
 
    var browserObj = $.detectBrowser.detect(); //detect properties of the browser in use.

    /*
    Supporting the General Dialog
    */
    wndGen = $("#dialog-general").kendoWindow({
        title: "A title",
        modal: true,
        visible: false,
        resizable: false,
        width: 300
    }).data("kendoWindow");

    /*
    QUESTIONS FOR GAME LISTVIEW
    */

    gameQuesDatasource = new kendo.data.DataSource({
        "type": (function () { if (kendo.data.transports['aspnetmvc-ajax']) { return 'aspnetmvc-ajax'; } else { throw new Error('The kendo.aspnetmvc.min.js script is not included.'); } })()
                           , "transport": {
                                "read": { "url": "/GameQuestions/GetGameQuestions/" + GAME_ID_PASSED_IN }
                                , "create": { "url": "/GameQuestions/Create" }
                                , "update": { "url": "/GameQuestions/Update" }
                                , "destroy": { "url": "/GameQuestions/Delete" }
                                ,"prefix": ""
                            }
                            , "pageSize": 10
                            , "page": 1
                            , "total": 0
                            , "filter": []
                            , "sort": { field: "OrderNbr", dir: "asc" }
                            , "schema": {
                                "data": "Data"
                                , "total": "Total"
                                , "errors": "Errors"
                                , "model": {
                                    "id": "Id"
                                    , "fields": {
                                        "Id": { "type": "number" }
                                        , "GameId": { "type": "number" }
                                        , "QuestionId": { "type": "number" }
                                        , "OrderNbr": { "type": "number" }
                                        , "Weight": { "type": "number" }
                                        , "Name": { "type": "string" }
                                        , "Text": { "type": "string" }
                                    }//"fields": {
                                }//"model": {
                            }//"schema": {
                            ,sync: function(e) { //sync event
                                console.log("sync complete");
                                isReadyToAddGQItem = true;
                            }
    });//new kendo.data.DataSource({

    gameQuesDatasource.bind("error", GameQuesDataSourceErrorHandler);


    /* 8/29/15 - IE doesn't support multiple select */
    $("#gameQuesListview").kendoListView({
        dataSource: gameQuesDatasource,
        template: kendo.template("#= DisplayListViewItem(data) #")
        , "pageable": { "autoBind": false, "pageSizes": [10, 20, 50, 100], "buttonCount": 10, "pagerId": "gameQuesListviewPager" } //need the extra div to exist (quesListviewPager)
        , selectable: (browserObj.browser != 'ie') ? "multiple" : "single"
        , change: function () {
            console.log('gameQues change event');
        }
        , edit: function () {
            //console.log('gameQues edit event');
        }
        , remove: function () {
            //console.log('gameQues remove event');
        }
        , cancel: function () {
            //console.log('gameQues cancel event');
        }
        , dataBound: function () {
            console.log('gameQues databound event');

            ReSelectGameQuesItems();

            AdjustListViewsHeight();

            SetGameQuesButtonsVisibility();
        }
        , dataBinding: function () {
            //console.log('gameQues databinding event');
        }
        , save: function () {
            //console.log('gameQues save event');
        }
    });

    //we only can drag and drop if the game is editable
    if (GAME_EDITABLE == 'True') {

        $("#gameQuesListview").kendoDropTarget({
            group: "GameQuestion",
            dragenter: function (e) {
                e.draggable.hint.css("opacity", 0.6);
                e.draggable.hint.css("background-color", "blue");
            },
            dragleave: function (e) {
                e.draggable.hint.css("opacity", 1);
                e.draggable.hint.css("background-color", "black");
            },
            drop: function (e) {
                draggedItemList = e.draggable.hint.children();
                OrderNbrToRecord = GetGameQuestionDropOrderNbrByItemPosition(e);

                AddItemsToGameQuestion(draggedItemList, OrderNbrToRecord);
            }//drop

        });//$("#gameQuesListview").kendoDropTarget({

        $("#gameQuesListview").kendoDraggable({
            group: "Question",
            filter: ".move",
            hint: function (element) {

                var hint = $("#gameQuesListview").clone();

                //need to set the width of the hint (draggable area) so when there are 
                //multiple items selected; they will be stacked on top of each other
                gameQuesListviewWidth = $("#gameQuesListview").css('width');
                hint.css('width', gameQuesListviewWidth);
                hint.css('max-width', gameQuesListviewWidth);

                hint.children().not(".k-state-selected").remove();
                hint.css('min-height', '0'); //we set this to zero; so the hint is ONLY as high as it needs to be to hold the selections.
                hint.css('height', hint.children().length * LISTVIEW_ITEM_HEIGHT + 'px');
                return hint;
            }
        });//$("#gameQuesListview").kendoDraggable({

    }

    function gqlvOnSortChange() {
        console.log('gqlvOnSortChange');
    }
    function gqlvSortHint() {
        console.log('gqlvSortHint');
    }
    function gqlvSortPlaceholder() {
        console.log('gqlvSortPlaceholder');
    }

    /*
    QUESTIONS LISTVIEW
    */

    quesDatasource = new kendo.data.DataSource({

        "type": (function () { if (kendo.data.transports['aspnetmvc-ajax']) { return 'aspnetmvc-ajax'; } else { throw new Error('The kendo.aspnetmvc.min.js script is not included.'); } })()
                            , "transport": {
                                "read": {
                                    "url": "/ChoiceQuestions/GetGameQuestions/" 
                                    ,"data": {
                                                gameid: GAME_ID_PASSED_IN
                                                , aclid: ($('#ACLDropdownList').val() == "") ? 0 : parseInt($('#ACLDropdownList').val())
                                                , questionsearch: $('#QuestionSearchFilter').val()
                                    }//data
                                }//read
                                , "prefix": ""
                            }
                            , "pageSize": 10
                            , "page": 1
                            , "total": 0
                            , "filter": { field: "Visible", operator: "equals", value: true }
                            , "sort": { field: "Name", dir: "asc" }
                            , "schema": {
                                "data": "Data"
                                , "total": "Total"
                                , "errors": "Errors"
                                , "model": {
                                    "id": "Id"
                                    , "fields": {
                                        "Id": { "type": "number" }
                                        , "QuestionTypeId": { "type": "number" }
                                        , "Name": { "type": "string" }
                                        , "Text": { "type": "string" }
                                        , "TestEnabled": { "type": "boolean" }
                                        , "ChoicesCount": { "type": "number" }
                                        , "Visible": { "type": "boolean" }
                                    }//"fields": {
                                }//"model": {
                            }//"schema": {


    });

    quesDatasource.bind("error", QuesDataSourceErrorHandler);

    /* 8/29/15 - IE doesn't support multiple select. Attention: paging does NOT trigger an automatic round-trip for this control */
    $("#quesListview").kendoListView({
        dataSource: quesDatasource
        , template: kendo.template("#= DisplayListViewItem(data) #")
        , "pageable": { "autoBind": false, "pageSizes": [10, 20, 50, 100], "buttonCount": 10, "pagerId": "quesListviewPager" } //need the extra div to exist (quesListviewPager)
        , selectable: (browserObj.browser != 'ie') ? "multiple" : "single"
        , change: function (e) {
            console.log('ques change event')
            UpdateQuestionDetailsView(e);
        }
        , dataBound: function (e) {
            console.log('ques databound event');

            //if from some event the ques and game ques list views are at a different height, then we make their heights the same
            AdjustListViewsHeight();

            //always select the first quest item (if it's visible)
            gqListView = $("#quesListview").data("kendoListView");
            if (gqListView.element.children().length > 0) {
                lvItem = gqListView.element.children()[0];
                if (lvItem.length != 0) {
                    gqListView.select(lvItem);
                } 
            }

            UpdateQuestionDetailsView(e);
            AdjustDraggable();

            //When the question autocomplete is not displayed because of window size then clear this prompt
            if ($('#QuestionAutoCompleteDiv').css('display') == 'none') {
                qListViewDataSource = $("#quesListview").data("kendoListView").dataSource;
                if (qListViewDataSource.filter().filters.length > 1) {
                    ClearQuestionAutocomplete();
                }
            }

        } //dataBound
        , dataBinding: function () {
            //console.log('ques databinding event');
        }
        , save: function () {
            //console.log('ques save event');
        }
    });//$("#quesListview").kendoListView({

    //we only can drag and drop if the game is editable
    if (GAME_EDITABLE == 'True') {

        $("#quesListview").kendoDropTarget({
            group: "Question",
            dragenter: function (e) {
                e.draggable.hint.css("opacity", 0.6);
                e.draggable.hint.css("background-color", "blue");
            },
            dragleave: function (e) {
                e.draggable.hint.css("opacity", 1);
                e.draggable.hint.css("background-color", "black");
            },
            drop: function (e) {

                //support for multiselect - let's iterate thru each selection and ADD to GAME QUES
                draggedItemList = e.draggable.hint.children();
                RemoveItemsFromGameQuestion(draggedItemList);

            }//drop
        });//$("#quesListview").kendoDropTarget({

        $("#quesListview").kendoDraggable({
            group: "GameQuestion",
            filter: ".move",
            hint: function (element) {

                var hint = $("#quesListview").clone();

                //need to set the width of the hint (draggable area) so when there are 
                //multiple items selected; they will be stacked on top of each other
                quesListviewWidth = $("#quesListview").css('width');
                hint.css('width', quesListviewWidth);
                hint.css('max-width', quesListviewWidth);

                hint.children().not(".k-state-selected").remove();
                hint.css('min-height', '0'); //we set this to zero; so the hint is ONLY as high as it needs to be to hold the selections.
                hint.css('height', hint.children().length * LISTVIEW_ITEM_HEIGHT + 'px');
                return hint;
            }
        });//$("#quesListview").kendoDraggable({
    }

    /*
    Question Library Filter TextBox Support
    */

    $('#QuestionSearchFilter').keyup(function () {

        OnQuestionSearchFilterChange(this.value);

    });//$('#QuestionSearchFilter').keyup(function () {

    /*
    GAME QUESTION SORT UP/DOWN ARROW SUPPORT
    */

    $("#gameQuesUpButton").kendoButton({
        icon: "arrow-n"
        , click: function (e) {
                
            ///we need to wait unti the GQ item can be added (sync from last sort is completed)
            if (!isReadyToAddGQItem) return;

            gqListView = $("#gameQuesListview").data("kendoListView");
            gameQuesView = gameQuesDatasource.view();


            if (gqListView.select().length > 0) {
                gameQuesSelect = gqListView.select();

                uid = $(gqListView.select()[0]).attr('data-uid');
                itemPosition = GetViewItemPositionByUID(gameQuesView, uid);

                if (itemPosition > 0) {
                    OrderNbrOfItemDestination = gameQuesView[itemPosition - 1].OrderNbr;
                    uidOfItemDestination = gameQuesView[itemPosition - 1].uid;
                    //iterate through selected items
                    orderNbr = OrderNbrOfItemDestination;

                    for (i = 0; i < gameQuesSelect.length; i++) {

                        uid = $(gameQuesSelect[i]).attr('data-uid');
                        item = gameQuesDatasource.getByUid(uid);
                        item.set('OrderNbr', orderNbr);
                        orderNbr++;
                    }

                    item = gameQuesDatasource.getByUid(uidOfItemDestination);
                    item.set('OrderNbr', orderNbr);

                }//if (itemPosition > 0) {
            }//if (gqListView.select().length > 0) {

            gameQuesDatasource.sync(); //remote sync - will call create transport
        }//click

    });//$("#gameQuesUpButton").kendoButton({

    $("#gameQuesDnButton").kendoButton({
        icon: "arrow-s"
        , click: function (e) {

            ///we need to wait unti the GQ item can be added (sync from last sort is completed)
            if (!isReadyToAddGQItem) return;

            gqListView = $("#gameQuesListview").data("kendoListView");
            gameQuesView = gameQuesDatasource.view();


            if (gqListView.select().length > 0) {
                gameQuesSelect = gqListView.select();

                uid = $(gqListView.select()[0]).attr('data-uid');
                itemPositionStart = GetViewItemPositionByUID(gameQuesView, uid);

                uid = $(gqListView.select()[gqListView.select().length - 1]).attr('data-uid');
                itemPositionEnd = GetViewItemPositionByUID(gameQuesView, uid);

                if (itemPositionEnd + 1 < gameQuesView.length) {
                    OrderNbrOfItemStart = gameQuesView[itemPositionStart].OrderNbr;
                    uidOfItemDestination = gameQuesView[itemPositionEnd + 1].uid;

                    //iterate through selected items
                    for (i = 0; i < gameQuesSelect.length; i++) {

                        uid = $(gameQuesSelect[i]).attr('data-uid');
                        item = gameQuesDatasource.getByUid(uid);
                        item.set('OrderNbr', item.OrderNbr + 1);
                    }

                    item = gameQuesDatasource.getByUid(uidOfItemDestination);
                    item.set('OrderNbr', OrderNbrOfItemStart);

                }//if (itemPosition > 0) {
            }//if (gqListView.select().length > 0) {

            gameQuesDatasource.sync(); //remote sync - will call create transport
        }//click

    });////$("#gameQuesDownButton").kendoButton({

    /*
    GAME QUESTION ADD/REMOVE ITEM - LEFT/RIGHT  SUPPORT
    */

    $("#gameQuesLtButton").kendoButton({ //remove question from game
        icon: "arrow-w"
        , click: function (e) {

            ///we need to wait unti the GQ item can be added (sync from last remove is completed)
            if (!isReadyToAddGQItem) return;

            gqListView = $("#gameQuesListview").data("kendoListView");
            selectedItems = gqListView.select();
            if (selectedItems.length > 0) {
                RemoveItemsFromGameQuestion(selectedItems);
            }

        }//click

    });//$("#gameQuesUpButton").kendoButton({

    $("#gameQuesRtButton").kendoButton({ //Add question to game
        icon: "arrow-e"
        , click: function (e) {

            ///we need to wait unti the GQ item can be added (sync from last add is completed)
            if (!isReadyToAddGQItem) return;

            gListView = $("#quesListview").data("kendoListView");
            selectedItems = gListView.select();
            if (selectedItems.length > 0) {

                isReadyToAddGQItem = false;
                OrderNbrToRecord = GetGameQuestionDropOrderNbrByLastPosition();

                AddItemsToGameQuestion(selectedItems, OrderNbrToRecord)
            }

        }//click

    });////$("#gameQuesDownButton").kendoButton({

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
            ,"serverPaging": true
            , "serverSorting": true
            , "serverFiltering": true
            , "serverGrouping": true
            , "serverAggregates": true
            ,batch: true,
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
        AdjustListViewsHeight(); //when question details height changes we check
        //$('#questionDetailsView input').css('margin-right', '5px'); //this works only dynamically. Not via style in markup
    });

    //Add bells and whistles to question search textbox (including the search magnifying glass)
    $('#QuestionSearchFilter').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#QuestionSearchFilter').attr('placeholder', 'Enter search words ...');

    /*
     Add bells and whistles to Questions autocomplete textbox (including the search magnifying glass)
    */
    $('#QuestionAutoComplete').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#QuestionAutoComplete').parent().css('width', '270px'); //this will increase the Autocomplete Textbox size

    $('#gameTitle').html((GAME_NAME_PASSED_IN.length <= 35) ? GAME_NAME_PASSED_IN : GAME_NAME_PASSED_IN.substr(0,35) + '...');

    //Can only do this once - add instructionsC:\Devo\VS13 Solutions\Probe\Probe\Content/bootstrap.min.css
    AddInstructions();

    //sets the padding when window is resized. Not going to happen on a phone.
    $(window).resize(function () {
        console.log('resize triggered');

        AdjustDraggable();

        //When the question autocomplete is not displayed because of window size then clear this prompt
        if ($('#QuestionAutoCompleteDiv').css('display') == 'none') {

            //We need to clear the filters to the base (just Visible). Only if the filters are Visible and Name (2 filters)
            qListViewDataSource = $("#quesListview").data("kendoListView").dataSource;
            if (qListViewDataSource.filter().filters.length > 1) {
                console.log('Reset filter to Base (just Visible)');

                ClearQuestionAutocomplete();
                qListViewDataSource.filter(GetQuesFilter('Base'));
                qListViewDataSource.read();
            }//if (qListViewDataSource.filter().filters.length > 1) {
        }
    });

});//$(document).ready(function () {

/*
SUPPLEMENTARY FUNCTIONS TO SUPPORT GAMEQUESTION
*/

function DisplayListViewItem(data) {
    itemStr = '';
    maxlistviewItemLength = MAX_LISTVIEW_ITEM_LENGTH

    //data.Visible tells us if its the ques or gameQues Listview we are working on
    if (data.Visible == undefined) {
        currentQuesView = gameQuesDatasource.view();
        itemStr = (((gameQuesDatasource.page() - 1) * gameQuesDatasource.pageSize()) +
                    GetViewItemPositionByUID(currentQuesView, data.uid) + 1) + '. ';
        maxlistviewItemLength = maxlistviewItemLength - 4;

        draggableWidth = convertPxStrToFloat($('#gameQuesListview').css('width')) - draggablePadding;
    } else {
        draggableWidth = convertPxStrToFloat($('#quesListview').css('width')) - draggablePadding;
    }

    if (data.Name.length <= maxlistviewItemLength) {

        if (data.Name.length + data.Text.length <= maxlistviewItemLength) {
            itemStr += data.Name + ' - ' + data.Text;
        } else {
            tmpStr = data.Name + ' - ' + data.Text;
            itemStr += tmpStr.substr(0, maxlistviewItemLength) + '...';
        }

    } else {
        itemStr += data.Name.substr(0, maxlistviewItemLength) + '...';
    }

    return '<div class="draggable move k-block" style="width:' + draggableWidth + 'px">' + itemStr + '</div>';
}//function DisplayListViewItem(data)

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

function OnQuestionSearchFilterChange(value) {
    console.log('func OnQuestionSearchFilterChange start');

    if (existingContainsFilterValue != value) {

        //A change with question search filter - we have to check if question auto complete is being used.
        if ($("#QuestionAutoComplete").data("kendoAutoComplete").value() != '') {
            ClearQuestionAutocomplete(); ////anything entered in question search filter will make question autocomplete empty
            $("#quesListview").data("kendoListView").dataSource.filter(GetQuesFilter('Base')); //set filter to Base
        }

        gListView = $("#quesListview").data("kendoListView");

        //We check if the page index of the listview is greater than 1. if
        //so then we want to page back. We want to avoid this because when dynamic paging
        //it does a round trip
        if (gListView.pager.page() > 1) {
            gListView.pager.page(1);
        }

        qListViewDataSource = $("#quesListview").data("kendoListView").dataSource;
        dataToPass = {
            gameid: GAME_ID_PASSED_IN,
            aclid: ($('#ACLDropdownList').val() == "") ? 0 : parseInt($('#ACLDropdownList').val()),
            questionsearch: $('#QuestionSearchFilter').val()
        };
        qListViewDataSource.read(dataToPass);

    }//if (existingFilterValue != value) {

    existingContainsFilterValue = $('#QuestionSearchFilter').val();

}//function OnQuestionSearchFilterChange(value) {

/*
Called when QuestionAutoComplete control value has changed. This function
will get the value and then push a new filter into the quesListview Listview. The filter will just show
every question that equals the autocompleted selected question name.
*/
function QuestionAutocompleteChange() {
    questionFilterValue = $("#QuestionAutoComplete").data("kendoAutoComplete").value();

    ClearQuestionSearchFilter();

    qListViewDataSource = $("#quesListview").data("kendoListView").dataSource;
    var listViewFilter = { filters: [] };

    if ($.trim(questionFilterValue).length > 0) {
        listViewFilter = GetQuesFilter('Name');
    } else {
        listViewFilter = GetQuesFilter('Base');
    }

    qListViewDataSource.filter(listViewFilter);

    dataToPass = {
        gameid: GAME_ID_PASSED_IN,
        aclid: ($('#ACLDropdownList').val() == "") ? 0 : parseInt($('#ACLDropdownList').val()),
        questionsearch: $('#QuestionSearchFilter').val()
    };
    qListViewDataSource.read(dataToPass);

}//function QuestionAutocompleteChange() {

function OnACLDrpDownChange(e) {
    console.log('start OnACLDrpDownChange');

    qListViewDataSource = $("#quesListview").data("kendoListView").dataSource;
    dataToPass = {
        gameid: GAME_ID_PASSED_IN,
        aclid: ($('#ACLDropdownList').val() == "") ? 0 : parseInt($('#ACLDropdownList').val()),
        questionsearch: $('#QuestionSearchFilter').val()
    };
    qListViewDataSource.read(dataToPass);
}

/*
Remove the Question from GameQuestion Listview and then
make Question visible again in the Question Listview
*/
function RemoveQuestionFromGame(id) {
    var item = gameQuesDatasource.getByUid(id);

    SetQuestionVisibility(true, item.Name);

    gameQuesDatasource.remove(item);
}//function RemoveQuestionFromGame(e, id) {

/*
Set the Visiblity field for a Question in the Question Listview Datasource using the question name
*/
function SetQuestionVisibility(visibleInd, questionName) {
    itemWithQName = undefined;
    itemWithQName = GetQuestionByName(quesDatasource, questionName);

    //If we can't find a question from is name than we will just ignore it. This
    //is a real scenario when after a question is used by a game, the question name is changed or
    //the question is flat out deleted.
    if (itemWithQName != undefined) {
        itemWithQName.set("Visible", visibleInd);
    }
}

/*
returns the item position that a question is dropped in the Game Question Listview. The item position
is relative to the datasource.view that is displayed to the user.
*/
function GetGameQuestionDropOrderNbrByItemPosition(e) {

    //find where we are going to put the drop payload
    gqListView = $("#gameQuesListview").data("kendoListView");
    orderNbr = 1;
    if (gqListView.element.children().length != 0) {
        positionYofDrop = e.pageY;
        findPosition = false;
        for (i = 0; i < gqListView.element.children().length; i++) { //iterate thru existing game questions
            if ($(gqListView.element.children()[i]).offset().top > positionYofDrop) {
                orderNbr = gameQuesDatasource.view()[i].OrderNbr;
                findPosition = true;
            } else if (i == gqListView.element.children().length - 1) {
                orderNbr = gameQuesDatasource.view()[i].OrderNbr + 1;
                findPosition = true;
            }

            if (findPosition) break;
        }
    }//if (gqListView.element.children().length != 0) {

    return orderNbr;
}//function GetGameQuestionDropItemPosition(e) {

function GetGameQuestionDropOrderNbrByLastPosition() {
    orderNbr = 0;
    maxOrderNbr = -1;
    //find where we are going to put the drop payload
    GQdata = gameQuesDatasource.data();
    if (GQdata.length != 0) {
        for (i = 0; i < GQdata.length; i++) {
            maxOrderNbr = Math.max(maxOrderNbr, GQdata[i].OrderNbr);
            orderNbr = maxOrderNbr;
        }
    }
    return orderNbr + 1;
}//function GetGameQuestionDropOrderNbrByLastPosition {

function GetViewItemPositionByUID(view, uid) {

    foundInd = false;
    itemPosition = 0;
    while (!foundInd && itemPosition < view.length) {
        if (view[itemPosition].uid == uid) {
            foundInd = true;
        } else {
            itemPosition++;
        }
    }
    return itemPosition;
}//function GetViewItemPositionByUID(view, uid) {

function ReSelectGameQuesItems() {

    if (gameQuesSelect != undefined) {
        //reselect the selections
        gqListView = $("#gameQuesListview").data("kendoListView");
        for (i = 0; i < gameQuesSelect.length; i++) {
            lvItem = gqListView.element.children("[data-uid='" + $(gameQuesSelect[i]).attr('data-uid') + "']");
            if (lvItem.length != 0) {
                gqListView.select(lvItem);
            }
        }
        gameQuesSelect = undefined; //now let's clear this thing; so it doesn't get in the way of another operation (used for sort up/down)
    }

}//ReSelectGameQuesItems()

function GetQuestionIdOfLatestSelection(e) {

    gListView = $("#quesListview").data("kendoListView");
    currentSelection = gListView.select();
    lastSelection = e.sender.selectable._lastActive;

    questionId = -1;
    uid = "NOUID";
    if (currentSelection.length == 1) {
        uid = $(currentSelection[0]).attr('data-uid');
        qItem = quesDatasource.getByUid(uid);
        questionId = qItem.Id;
    } 

    return questionId;

}//function GetQuestionIdOfLatestSelection(e) {

/*
Get item from Listview datasource using the question name
*/
function GetQuestionByName(theDataSource, questionName) {
    qItem = undefined;
    index = 0;
    data = theDataSource.data();
    while (index < data.length) {
        if (data[index].Name == questionName) {
            qItem = data[index];
            break;
        }
        index++;
    }
    return qItem;
}//function GetQuestionByName(theDataSource, questionName) {

/*
itemList - is the items to add to game question. 
OrderNbrToRecord - the order number of the first item to add to game question.
*/
function AddItemsToGameQuestion(itemList, OrderNbrToRecord) {

    gameQuesData = gameQuesDatasource.data();
    //Update OrderNbr for existing questions of the Game
    OrderNbrShift = itemList.length;
    for (i = 0; i < gameQuesData.length; i++) {
        if (gameQuesData[i].OrderNbr >= OrderNbrToRecord) {
            gameQuesData[i].set('OrderNbr', gameQuesData[i].OrderNbr + OrderNbrShift);
        }
    }//for (i = 0; i < gameQuesData.length; i++) {

    //support for multiselect - let's iterate thru each selection and ADD to GAME QUES
    //and Add the new dragged and dropped items into the Game
    for (i = 0; i < itemList.length; i++) {
        itemUID = $(itemList[i]).attr('data-uid');
        var qItem = quesDatasource.getByUid(itemUID);

            gameQuesItem = {
                GameId: GAME_ID_PASSED_IN
                , QuestionId: qItem.Id, OrderNbr: OrderNbrToRecord, Name: qItem.Name, Text: qItem.Text, Weight: 10
            };  
            console.log('adding question ' + qItem.Name);
            gameQuesDatasource.add(gameQuesItem);
            SetQuestionVisibility(false, qItem.Name); //hide question on Question Library LV

            OrderNbrToRecord++;

    }//for (i = 0; i < draggedItemList.length; i++) {

    //resync the backend, and refresh Game Ques LV
    gameQuesDatasource.sync(); //remote sync - will call create transport
    //refresh Ques LV
    qListView = $("#quesListview").data("kendoListView");
    qListView.refresh();

    RemoveInstructions();
}//AddItemsToGameQuestion(itemList)

function RemoveItemsFromGameQuestion(itemList) {
    for (i = 0; i < itemList.length; i++) {

        itemUID = $(itemList[i]).attr('data-uid');
        RemoveQuestionFromGame(itemUID);
    }

    qListView = $("#quesListview").data("kendoListView");
    qListView.refresh();

    gameQuesDatasource.sync();
    quesDatasource.sort({ field: "Name", dir: "asc" }); //resort the question listview

    RemoveInstructions();
}//function RemoveItemsFromGameQuestion(itemList)

function UpdateQuestionDetailsView(e) {
    questionIdOfCurrentSelection = GetQuestionIdOfLatestSelection(e);

    //check to see if there is a selection, if there is we go get question detail info; if not we
    //clear out question detail info
    if (questionIdOfCurrentSelection != -1) {
        quesDetailfilter = ({ field: "Id", operator: "equals", value: questionIdOfCurrentSelection });
        quesDetailsModel.dataSource.filter(quesDetailfilter);
        quesDetailsModel.dataSource.read(); //invoke the read transport of the DataSource
    } else {
        $("#questionDetailsView").html(DisplayQuestionDetails([])); //clear out question details view
        AdjustListViewsHeight(); //when question details height changes we check
    }
}//function UpdateQuestionDetailsView(e)

function IsQuestionDupforGame(questionName) {
    questionDup = false;

    GQdata = gameQuesDatasource.data();
    if (GQdata.length != 0) {
        for (i = 0; i < GQdata.length; i++) {
            if (GQdata[i].Name == questionName) {
                questionDup = true;
                break;
            }
            if (questionDup) break;
        }
    }

    return questionDup;
}

function AddInstructions() {

    if (GAME_EDITABLE == 'True') {
        $('#instructionsView').html('Please fill out your game with the questions available to you from the Question Library. Select your questions and use the Move button or Drag them over to your Game area.');
    } else {
        $('#instructionsView').html('Your game is active and read only. You can only view the questions in the game.');
    }

}

function RemoveInstructions() {
    quesTdHeight = $('#quesTd').css('height');

    $('#instructionsView').html('');
    $('#instructionsView').css('border', 'none');
    $('#instructionsView').css('padding','0');
    $('#instructionsView').css('margin', '0');
    $('#instructionsView').css('height', '0');
    $('#questionDetailsView').css('height', quesTdHeight)

}

function AdjustListViewsHeight() {
    minLVHeight = 50;

    //if from some event the ques and game ques list views are at a different height, then we make their heights the same
    quesLVParentHeight = convertPxStrToFloat($('#quesListview').parent().css('height'));
    quesLVHeight = convertPxStrToFloat($('#quesListview').css('height'));
    quesLVPagerHeight = convertPxStrToFloat($('#quesListviewPager').css('height'));
    gameQuesLVParentHeight = convertPxStrToFloat($('#gameQuesListview').parent().css('height'));
    gameQuesLVHeight = convertPxStrToFloat($('#gameQuesListview').css('height'));
    gameQuesLVPagerHeight = convertPxStrToFloat($('#gameQuesListviewPager').css('height'));
    quesDetailsDivHeight = convertPxStrToFloat($('#questionDetailsView').parent().css('height'));

    //To adjust column heights - we first check Question LV against Question Details Div
    if (quesLVParentHeight > quesDetailsDivHeight) {
        //Ques LV height should not change

        if (gameQuesLVParentHeight < quesLVParentHeight) {
            //Game Question LV is shorter than Question LV; Therefore we will make GQ LV the same height as Ques LV
            marginsVerticalPaddingTotal = 22;
            heightOfJustquesLV = quesLVParentHeight - quesLVPagerHeight - marginsVerticalPaddingTotal;
            $('#gameQuesListview').css('height', heightOfJustquesLV);
        }

    } else {
        //Ques LV should change to be the height of the Ques Details DIV

        $('#quesListview').css('height', quesDetailsDivHeight);

        if (gameQuesLVParentHeight < quesDetailsDivHeight) {
            $('#gameQuesListview').css('height', quesDetailsDivHeight);
        }
    }//if (quesLVParentHeight > quesDetailsDivHeight) {

    //After all that. if the Ques or GameQues LV has a height of nothing. We are going to give it a boost.
    if (quesLVHeight < minLVHeight) {
        $('#quesListview').css('height', minLVHeight);
    }
    if (gameQuesLVHeight < minLVHeight) {
        $('#gameQuesListview').css('height', minLVHeight);
    }

}

function AdjustDraggable() {

    draggableWidth = convertPxStrToFloat($('#quesListview').css('width')) - draggablePadding;
    console.log('draggableWidth=' + draggableWidth);
    //resize draggables (listview items) - using the width of the question listview minus some padding
    $('.draggable').css('width', draggableWidth);
}

function SetGameQuesButtonsVisibility() {
    if (GAME_EDITABLE == 'True') {
        $("#gameQuesUpButton").show();
        $("#gameQuesDnButton").show();
        $("#gameQuesLtButton").show();
        $("#gameQuesRtButton").show();
    } else {
        $("#gameQuesUpButton").hide();
        $("#gameQuesDnButton").hide();
        $("#gameQuesLtButton").hide();
        $("#gameQuesRtButton").hide();
    }
}

function ClearQuestionAutocomplete() {
    $("#QuestionAutoComplete").data("kendoAutoComplete").value('');
}

function ClearQuestionSearchFilter() {
    $('#QuestionSearchFilter').val(''); //anything entered in question autocomplete will make question search filter empty
}

function CommonErrorHandler(message) {
    ShowGeneralDialog(wndGen, 'Error', message, '', false, '', true, 'Close');
    $("#noGen").click(function () {
        wndGen.close();
    });
}//function CommonErrorHandler(message) {

function GameQuesDataSourceErrorHandler(e) {
    message = e.errors[""].errors[0];
    message = "Game Question Query: " + message;
    CommonErrorHandler(message);
}

function QuesDataSourceErrorHandler(e) {
    message = e.errors[""].errors[0];
    message = "Question Library Query: " + message;
    CommonErrorHandler(message);
}

function GetQuesFilter(filterType) {

    var listViewFilter = { filters: [] };
    listViewFilter.logic = "and";   // a different logic 'or' can be selected

    switch (filterType) {
        case 'Base':
            listViewFilter.filters.push({ field: "Visible", operator: "equals", value: true });
            break;
        case 'Name':
            listViewFilter.filters.push({ field: "Visible", operator: "equals", value: true });
            listViewFilter.filters.push({ field: "Name", operator: "eq", value: questionFilterValue });
            break;
    }

    return listViewFilter;
}//function GetQuesFilter(filterType) {
