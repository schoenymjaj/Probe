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