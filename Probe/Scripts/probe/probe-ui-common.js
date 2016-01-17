function RepairGridHeader(gridName) {

    nbrOfGroupings = $('#' + gridName + ' .k-grouping-header').children().length;

    //If there is at least one grouping; let's see if there is a broken grid column header
    if (nbrOfGroupings > 0) {

        nbrOfColumnsInHeader = $('#' + gridName + ' thead th').length;
        nbrOfColumnsInData = $('#' + gridName + ' tbody tr.k-master-row:first').children().length;

        if (nbrOfColumnsInHeader < nbrOfColumnsInData) {
            //It's broken. Less columns in the header than in the data rows; so lets try and fix it
            $('#'+ gridName + ' thead th:first').after('<th>&nbsp;</th>'); //Hopefully we just added a column to the header
        }

    }//if (nbrOfGroupings > 0) {

} //function RepairGridHeader() {

function PrepareURL(url) {
    if (url.indexOf('https') == -1) {
        return 'https://' + url;
    } else {
        return url;
    }
}

/*
    MANAGE SCROLL BAR POSITION
*/

function saveScrollPosition(localStorageName, dontSavePositionNextInd) {
    console.log('func saveScrollPosition: ' + document.body.scrollTop + '  saveScrollPositionInd: ' + saveScrollPositionInd + '  dontSavePositionNextInd: ' + dontSavePositionNextInd);

    if (saveScrollPositionInd) {
        console.log('actually saving scroll position')
        localStorage[localStorageName] = document.body.scrollTop;
    } else {
        saveScrollPositionInd = true;
    }

    if (dontSavePositionNextInd != undefined) {
        saveScrollPositionInd = false;
    } 

};//function saveScrollPosition()

function restoreScrollPosition(localStorageName) {
    console.log('func estoreScrollPosition');

    if (localStorage[localStorageName] != "") {
        document.body.scrollTop = localStorage[localStorageName];
    }
};//function restoreScrollPosition()

function saveScrollPositionPrivate(localStorageName) {
    console.log('saveScrollPosition-Private: ' + document.body.scrollTop);

    localStorage[localStorageName + "-Private"] = document.body.scrollTop;
};//function saveScrollPosition()

function restoreScrollPositionPrivate(localStorageName) {
    console.log('restoreScrollPosition-Private');

    if (localStorage[localStorageName + "-Private"] != "") {
        document.body.scrollTop = localStorage[localStorageName + "-Private"];
    }
};//function restoreScrollPosition()

/*
    MANAGE THE PROGRESS BAR DIALOG 
*/
function OpenProgressBarWindow(startValue, interval) {

    kendo.ui.progress($('#progress-spinner'), true);

    //wndProgress.title("In Progress. Please Wait ...");
    //wndProgress.center().open(); //open progress window

    //progressBar = $('#progressbar').data('kendoProgressBar')
    //progressBar.value(startValue);
    //setInterval(function () {
    //    newProgressBarValue = progressBar.value() + 10;
    //    progressBar.value(newProgressBarValue);
    //}
    //, interval);

}//function OpenProgressBarWindow

function CloseProgressBarWindow() {

    kendo.ui.progress($('#progress-spinner'), false);

    //wndProgress.close();
}

/*
    SUPPORT GENERAL DIALOG
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


/*
    $.getJSON HELPER
*/
ajaxGetHelper = function (uri) {
    console.log('START ajaxGetHelper uri:' + uri);
    return $.getJSON(uri, {})
        .fail(function (jqxhr, textStatus, error) {
            CloseProgressBarWindow();
            ShowGeneralDialog(wndGen, 'Error', 'There was a connectivity issue. Please try again. If the  problem persists please connect In Common support.', '', false, '', true, 'Close');
            $("#noGen").click(function () {
                wndGen.close();
            });
        }) //$.getJSON.fail
}//function ajaxHelper

/*
    SUPPORT ERROR HANDLING
*/

/* Supporting Message Summary (for EDIT Popup) - Top of Create/Edit popup*/
function MyErrorHandler(args) {
    console.log('MyErrorHandler');
    if (args.errors) {
        //var grid = $("#MyGamesGrid").data("kendoGrid");
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

function PopulateInlineMessages(grid, args) {
    for (var error in args.errors) {
        if (error != "") { //we dont touch summary messages
            showMessage(grid.editable.element, error, args.errors[error].errors);
        }
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

/*
 Convert the pixel string to float (i.e. from the result of a jQuery.css('height') call)
*/
function convertPxStrToFloat(pixelString) {
    return parseFloat(pixelString.replace('px', ''));
}