
$(function () {
    // Reference the auto-generated proxy for the hub (SignalR CONNECTIONS)
    var notifyhub = $.connection.notifyHub; //THE REFERENCE MUST BE IN CAMEL CASE (SEE TUTUORIAL)
    // Create a function that the hub can call back to display messages.
    notifyhub.client.GameChangeNotification = function () {
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
Get game type text from the game type id
*/
function GameTypeText(gameTypeId) {

    gameTypeText = "";
    switch (gameTypeId) {
        case 1:
            gameTypeText = 'Match';
            break;
        case 2:
            gameTypeText = 'Test';
            break;
        case 3:
            gameTypeText = 'Last Man Standing';
            break;
    }

    return gameTypeText;
}

function PublishedText(publishedInd) {
    html = 'No';
    if (publishedInd) {
        html = 'Yes';
    }

    return html;
}

function SuspendedText(SuspendedInd) {
    html = 'No';
    if (SuspendedInd) {
        html = 'Yes';
    }

    return html;
}

/*
function template for Grid - GameType
*/
function DisplayGameType(game) {
    return GameTypeText(game.GameTypeId);
}//function displayGameType(game) {

/*
function template for Grid - Published
*/
function DisplayPublished(game) {
    html = PublishedText(game.Published);

    if (game.SuspendMode) {
        html = html + '<br/>(game suspended)';
    }

    return html;
}//function DisplayPublished(game) {

/*
function template for Grid - player count
*/
function DisplayPlayerCount(game) {
    html = '<a class=\"playersDialogLink\" data-gameid=\"' + game.Id + '\" data-code=\"' +
            game.Code + '\" data-gametype=\"' + game.GameTypeId + '\" href=\"\\#\">' + game.PlayerCount + '</a>';

    if (game.GameTypeId == 3 && game.PlayerCount > 0) { //We will display this a little bit differently if it's LMS 
        html = html + '<br/>(' + game.PlayerActiveCount + ' standing)';
    }

    return html;

}//function DisplayPlayerCount(game) {

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

/*
Called when GamesAutoComplete control value has changed. This function
will get the value and then push a new filter into the MyGames grid. The filter will just show
every game that equals the autocompleted selected game name.
*/
function GameAutocompleteChange() {
    gameFilterValue = $("#GameAutoComplete").data("kendoAutoComplete").value();

    var gridListFilter = { filters: [] };
    var gridDataSource = $("#MyGamesGrid").data("kendoGrid").dataSource;

    gridListFilter.logic = "and";   // a different logic 'or' can be selected
    if ($.trim(gameFilterValue).length > 0) {
        gridListFilter.filters.push({ field: "Name", operator: "eq", value: gameFilterValue });
    }

    gridDataSource.filter(gridListFilter);
    gridDataSource.read();

}//function GameAutocompleteChange() {

/*
OnGridDataBound event for MyGames Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    //BIND PLAYERS COUNT COLUMN CLICK WITH THE PLAYERS DIALOG
    $('.playersDialogLink').click(function () {

        gameCode = $(this).attr('data-code');
        $.getJSON('/api/Players/GetPlayerByGameCode/' + gameCode, {},
        function (data) {

            $('select[name="Pplayers"]').empty();
            data.forEach(function (value, index, ar) {
                $('select[name="Pplayers"]').append('<option data-playerid="' + value.Id + '">' + value.PlayerGameName + '</option>');
            });

            wndPlayers.center().open();

            $("#playersClose").click(function () {
                wndPlayers.close();
            });

        });//post
    });//$('#playersDialogLink').click

    grid = $("#MyGamesGrid").data("kendoGrid");
    gridView = grid.dataSource.view();

    StyleGridView(gridView);

    //Repair the grid header, if it needs it. Hack for a grouping bug
    RepairGridHeader("MyGamesGrid");

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

        //Set Popup title, action button, and Fields
        $(".k-window-title").text("Add Game");
        $('.k-grid-update').text("Create");
        $('#GridEditPopupNbrPlayers').hide();
        $('#GridEditPopupNbrStanding').hide();
        $('#GridEditPopupNbrQuestions').hide();

        //Set Popup initial values
        startDateDP = e.container.find('#StartDate').data('kendoDateTimePicker');
        endDateDP = e.container.find('#EndDate').data('kendoDateTimePicker');

        currentDateWSecs = new Date();
        currentDate = new Date(currentDateWSecs.getTime() - (currentDateWSecs.getSeconds() * 1000));
        currentDatePlusDay = new Date(currentDate.getTime() + 86400000);
        startDateDP.value(currentDate);
        endDateDP.value(currentDatePlusDay);
        e.container.find('#StartDate').data('kendoDateTimePicker').trigger("change");
        e.container.find('#EndDate').data('kendoDateTimePicker').trigger("change");


    } else {

        //Set Popup title, action button, and Fields
        $(".k-window-title").text("Edit Game");
        $('.k-grid-update').text("Update");
        $('#GridEditPopupNbrPlayers').show();

        if (e.model.GameTypeId == 3) { //only show if LMS
            $('#GridEditPopupNbrStanding').show();
        } else {
            $('#GridEditPopupNbrStanding').hide();
        }
        $('#GridEditPopupNbrQuestions').show();

        //Cannot modify the game type if there are questions
        if (e.model.QuestionCount > 0) {
            gameTypeDDL = e.container.find("#GameTypeId").data("kendoDropDownList");
            gameTypeDDL.enable(false);
        }

        //Cannot modify the game name, or code.. if published or there are players
        if (e.model.Published || e.model.Players > 0) {
            e.container.find("#Name").prop("disabled", true).addClass("k-state-disabled");
            e.container.find("#Code").prop("disabled", true).addClass("k-state-disabled");
        }
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
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameName = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Name;

    //prepare and open confirmation dialog
    message1 = 'You are about to delete the game <span style="font-style: italic;font-weight:bold">' + gameName + '.</span>';
    if (gameGrid.dataItem('[data-uid="' + rowUID + '"]').QuestionCount > 0) {
        message1 = message1 + ' <span style="font-style:italic">Warning: This game has questions associated with it. The questions within the game will be removed from the game as well.</span>';
    }
    message2 = '<span style="font-weight:bold">Are you sure?</span>';
    ShowGeneralDialog(wndGen, 'Delete Game', message1, message2, true, 'OK', true, 'Cancel');
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
    wndDetails.title("Game Details");
    wndDetails.content(detailsTemplate(dataItem));
    wndDetails.center().open();
}//function openDetails(e)
function openConfig(e) {

    OpenProgressBarWindow();

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    window.location = 'GameConfigurations/Index/' + gameId;
}//function openConfig(e)
function openSchedules(e) {

    OpenProgressBarWindow();

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    window.location = 'Games/GameSchedules/' + gameId;
}//function openConfig(e)
function openQuestions(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;
    gameName = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Name;

    url = root + 'GameQuestions/GameQuestions/' + gameId;
    //The GameQuestions page requires the https:// for some reason
    if (url.indexOf('https') == -1) {
        url = 'https://' + url;
    }

    iframeHtml = '<iframe id="modalIframeId" width="100%" height="99%" marginWidth="0" marginHeight="0" ' +
                 'frameBorder="0" scrolling="yes"/>';

    dHeight = 620;

    $("#dialog-iframe").html(iframeHtml);
    wndIframe.title('Select Questions for Game - ' + gameName);
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


}//function openQuestions(e)
function PublishNow(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;
    publishedInd = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Published;

    url = 'Games/Publish/' + gameId + '/1';
    if (publishedInd) {
        url = 'Games/Publish/' + gameId + '/0';
    }

    $.getJSON(url, {},
    function (data) {

        //prepare and open informational dialog for clone action
        title = (publishedInd) ? 'Unpublish Game' : 'Publish Game';
        ShowGeneralDialog(wndGen, title, data.Message, '', false, '', true, 'Close');
        $("#noGen").click(function () {
            wndGen.close();
        });

    });//post
}//function PublishNow(e)
function openPlayers(e) {

    OpenProgressBarWindow();

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    window.location = 'Players/Index/' + gameId;

}//function openPlayers(e)
function openPreview(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameCode = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Code;

    ShowPreviewDialog(gameCode, 'TestName', 'TestName');

}//function openPreview(e)
function openResults(e) {

    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameCode = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Code;
    gameTypeId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').GameTypeId;
    gamePlayerCount = gameGrid.dataItem('[data-uid="' + rowUID + '"]').PlayerCount;
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    ShowReportDialog(gameCode, gameId, GameTypeText(gameTypeId), gamePlayerCount);

}
function CloneNow(e) {

    var grid = this;
    var row = $(e.currentTarget).closest("tr");

    rowUID = row.attr('data-uid');
    gameGrid = $('#MyGamesGrid').data('kendoGrid');
    gameId = gameGrid.dataItem('[data-uid="' + rowUID + '"]').Id;

    $.getJSON('Games/Clone/' + gameId, {},
    function (data) {

        //prepare and open informational dialog for clone action
        ShowGeneralDialog(wndGen, 'Clone Game', data.Message, '', false, '', true, 'Close');
        $("#noGen").click(function () {
            wndGen.close();
        });

    });//post
}//function CloneNow(e) {

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

function ShowReportDialog(code, gameId, gameType, playerCount) {

    $.getJSON('api/Players/GetPlayerByGameCode/' + code, {},
    function (data) {

        if (gameType == 'Match') {
            if (playerCount > 1) {
                $('#reportTypeDiv').show();
                $('#reportMessageDiv').hide();
                $('select[name="reportType"]').empty();
                $('select[name="reportType"]').append('<option>Game Match Summary</option><option>Player Match Summary</option><option>Player Match Stats</option>');
            } else {
                $('#reportTypeDiv').hide();
                $('#reportMessageDiv').html('At least two players must have submitted game plays to generate a report.');
            }
        } else if (gameType == 'Test') {
            $('#reportTypeDiv').show();
            $('#reportMessageDiv').hide();
            $('select[name="reportType"]').empty();
            $('select[name="reportType"]').append('<option>Game Test Scores</option><option>Game Test Stats</option>');
        } else if (gameType == 'Last Man Standing') {
            $('#reportTypeDiv').show();
            $('#reportMessageDiv').hide();
            $('select[name="reportType"]').empty();
            $('select[name="reportType"]').append('<option>Game LMS Summary</option><option>Player LMS Summary</option><option>Player LMS Detail</option>');
        }

        $('select[name="reportType"]').change(function () {
            //show players select menu when appropriate
            switch ($('select[name="reportType"]').val()) {
                case 'Game Match Summary':
                    $('#RplayersDiv').hide();
                    break;
                case 'Player Match Summary':
                    $('#RplayersDiv').show();
                    break;
                case 'Player Match Stats':
                    $('#RplayersDiv').hide();
                    break;
                case 'Game Test Scores':
                    $('#RplayersDiv').hide();
                    break;
                case 'Game Test Stats':
                    $('#RplayersDiv').hide();
                    break;
                case 'Game LMS Summary':
                    $('#RplayersDiv').hide();
                    break;
                case 'Player LMS Summary':
                    $('#RplayersDiv').hide();
                    break;
                case 'Player LMS Detail':
                    $('#RplayersDiv').show();
                    break;
            }

        });

        //fill the players selection menu
        $('select[name="Rplayers"]').empty();
        data.forEach(function (value, index, ar) {
            $('select[name="Rplayers"]').append('<option data-playerid="' + value.Id + '">' + value.PlayerGameName + '</option>');
        });

        wndResult.setOptions({
            width: 245,
        });
        wndResult.center().open();
        $('#RplayersDiv').hide(); //hide the players selection initially because a first selection doesnt need player selection

        $("#reportYes").click(function () {

            //get selected report name
            reportName = $('select[name="reportType"]').val();

            playerId = $('select[name="Rplayers"] option:selected').attr('data-playerId');

            reportURL = "";
            if (gameType == 'Match') {
                if (reportName == "Game Match Summary") {
                    reportURL = 'Reports/GamePlayerMatchMinMax/' + gameId + '/' + code;
                } else if (reportName == "Player Match Summary") {
                    reportURL = 'Reports/PlayerMatchSummary/' + gameId + '/' + code + '/' + playerId;
                }
            } else if (gameType == 'Test') {
                reportURL = 'Reports/PlayerTestSummary/' + gameId + '/' + code;
            } else if (gameType == 'Last Man Standing') {
                if (reportName == "Game LMS Summary") {
                    reportURL = 'Reports/GameLMSSummary/' + gameId + '/' + code;
                } else if (reportName == "Player LMS Summary") {
                    reportURL = 'Reports/PlayerLMSSummary/' + gameId + '/' + code + '/0';
                } else if (reportName == "Player LMS Detail") {
                    reportURL = 'Reports/PlayerLMSDetail/' + gameId + '/' + code + '/' + playerId;
                }
            }

            window.location = reportURL;

            wndResult.close();
        });

        $("#reportNo").click(function () {
            wndResult.close();
        });

    });//post
} //function ShowReportDialog

function ShowPreviewDialog(code, firstName, nickName) {
    url = 'Client/main.html'
        + '?code=' + code + '&firstname=' + firstName + '&nickname=' + nickName //the client doesn't pick up the firstname or lastname which is fine.
        + '&root=' + root; //i.e root = localhost/Probe/ OR localhost:4430/ OR .. (root is a global var)


    iframeHtml = '<iframe id="modalIframeId" width="100%" height="99%" marginWidth="0" marginHeight="0" ' +
                 'frameBorder="0" scrolling="no"/>';

    //responsive ui. mod the size of the jquery dialog to a certain point; then we have to use an instance of the browser
    if ($(window).height() > 720) {
        dHeight = 700;

        $("#dialog-iframe").html(iframeHtml);
        wndIframe.title('Game Preview');
        wndIframe.setOptions({
            height: dHeight,
            width: 500,
            position: {
                top: 25,
                left: "30%"
            }
        });
        wndIframe.open();

        $("#modalIframeId").attr("src", url);
    } else {
        browswerProps = 'titlebar=no,toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=no,width=500' +
                        ',height=' + Math.round($(window).height() * 0.80) +
                        ',left=' + (Math.round($(window).width() / 2) - 250) +
                        ',top=' + (Math.round($(window).height() * 0.10));

        window.open(url, '_blank', browswerProps);
    }

    return false;
}//function ShowPreviewDialog

function saveGridOptions(grid) {
    localStorage["GamesGridOptions"] = kendo.stringify(grid.getOptions());
    localStorage["GameAutoComplete"] = $("#GameAutoComplete").data("kendoAutoComplete").value();
};//function saveGridOptions(grid) {

function restoreGridOptions(grid) {
    var options = localStorage["GamesGridOptions"];
    if (options) {

        optionsJSON = JSON.parse(options);

        //need to refresh the toolbar because they are not persisted since they are serialized via the server (MVC helper)
        optionsJSON.toolbar = [
            { template: $("#toolbarTemplate").html() }
        ];
        optionsJSON.columns[0].headerTemplate = $("#headerTemplate").html();

        //Need to refresh the commands because they are not persisted since they were serialized via the server (MVC helper)
        commandCol = {
            "attributes": { "class": "gamesGridCommandColumn" }
            , "command":
                [{ "name": "edit", "buttonType": "ImageAndText", "text": "Edit" }
             , { "name": "Details", "buttonType": "ImageAndText", "text": "Details", "click": openDetails }
             , { "name": "Clone", "buttonType": "ImageAndText", "text": "Clone", "click": CloneNow }
             , { "name": "Config", "buttonType": "ImageAndText", "text": "Config", "click": openConfig }
             , { "name": "Schedule", "buttonType": "ImageAndText", "text": "Schedule", "click": openSchedules }
             , { "name": "Questions", "buttonType": "ImageAndText", "text": "Questions", "click": openQuestions }
             , { "name": "Publish", "buttonType": "ImageAndText", "text": "Publish", "click": PublishNow }
             , { "name": "Players", "buttonType": "ImageAndText", "text": "Players", "click": openPlayers }
             , { "name": "Preview", "buttonType": "ImageAndText", "text": "Preview", "click": openPreview }
             , { "name": "Results", "buttonType": "ImageAndText", "text": "Results", "click": openResults }
             , { "name": "Delete", "buttonType": "ImageAndText", "text": "Delete", "click": openDeleteConfirm }]
        };

        optionsJSON.columns.splice(7);
        optionsJSON.columns.push(commandCol);

        grid.setOptions(optionsJSON);

    }

    if (localStorage["GameAutoComplete"] != undefined) {
        if (localStorage["GameAutoComplete"] != "") {
            $("#GameAutoComplete").data("kendoAutoComplete").value(localStorage["GameAutoComplete"]);
        }
    }

};//function restoreGridOptions() {

/* Supporting Message Summary (for EDIT Popup) - Top of Create/Edit popup*/
function MyErrorHandler(args) {
    console.log('MyErrorHandler');
    if (args.errors) {
        var grid = $("#MyGamesGrid").data("kendoGrid");
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

/*Supports the Inline Messages for MyGames Edit Popup attached to the Fields of the Edit Popup*/
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
    gridDataSource = $("#MyGamesGrid").data("kendoGrid").dataSource;
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

        editButton = $(currentRow).find(".k-grid-edit:eq(0)");
        detailsButton = $(currentRow).find(".k-grid-edit:eq(1)");
        deleteButton = $(currentRow).find(".k-grid-Delete");
        publishButton = $(currentRow).find(".k-grid-Publish");
        playersButton = $(currentRow).find(".k-grid-Players");
        previewButton = $(currentRow).find(".k-grid-Preview");
        resultsButton = $(currentRow).find(".k-grid-Results");
        scheduleButton = $(currentRow).find(".k-grid-Schedule");


        //Get rid of the pencil icon on the details button
        detailsButton.children().removeClass('k-icon');

        if (currentDataItem.PlayerCount == 0) {
            playersButton.hide();
            resultsButton.hide();
        }
        if (currentDataItem.IsActive || currentDataItem.PlayerCount != 0) {
            deleteButton.hide();
        }
        if (!currentDataItem.IsActive) {
            previewButton.hide();
        }

        if (currentDataItem.QuestionCount != 0) {
            if (currentDataItem.Published) {
                publishButton.html('<span class=" "></span>Unpublish');
            } else {
                publishButton.html('<span class=" "></span>Publish');
            }
        } else {
            publishButton.hide();
        }
    }
}//StyleGridCommandRow

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
/*
MNS DEBUG
*/
function Oncancel() {
    console.log('Oncancel event');

    //This is a hack; because when the Grid popup is canceled; it appears to update the command row to its initial state
    setTimeout(function () { grid.refresh() },50);
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

    grid = $("#MyGamesGrid").data("kendoGrid");

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
    Supporting the Players Dialog from Players Count Column
    */
    wndPlayers = $("#dialog-players").kendoWindow({
        title: "Game Players",
        modal: true,
        visible: false,
        resizable: false,
        width: 300
    }).data("kendoWindow");

    /*
    Supporting the Report Dialog
    */
    wndResult = $("#dialog-report").kendoWindow({
        title: "Game Results",
        modal: true,
        visible: false,
        resizable: false,
        width: 300,
        height: "inherit"
    }).data("kendoWindow");

    /*
    Supporting the Preview and Questions Dialog
    */
    wndIframe = $("#dialog-iframe").kendoWindow({
        title: "Game Preview",
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

    //    var gridData = $("#MyGamesGrid").data("kendoGrid")._data;

    //});//$('#infoGet').click(function () {

    ////for debugging purposes - looking at grid datasource
    //$('#infoSync').click(function () {

    //    SyncServerData();

    //});//$('#infoGet').click(function () {

    /*
     Add bells and whistles to Games autocomplete textbox (including the search magnifying glass)
    */
    $('#GameAutoComplete').closest(".k-widget").addClass("k-textbox k-space-right").append('<span class="k-icon k-i-search"></span>');
    $('#GameAutoComplete').parent().css('width', '300px'); //this will increase the Autocomplete Textbox size

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


    //Will restore Grid configuration/options/command button event handlers - every time the Games Index page is called
    restoreGridOptions(grid);

}); //$(document).ready(function () {

