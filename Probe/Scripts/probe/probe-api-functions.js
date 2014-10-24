/*
call probe api to get a question text based on the question id passed in
*/
ApiGetQuestionText = function (rootUrl, questionId) {

    var questionText = "";
    $.ajax({
        type: 'GET',
        async: false,
        url: rootUrl + 'api/Questions/GetQuestion/' + questionId,
        dataType: 'json',
        contentType: 'application/json'
    })
    .done(function (questionData) {
        questionText = questionData.Text;
    })
    .fail(function (jqxhr, textStatus, error) {
        questionText = "";
        throw error;
    }); //fail

    return questionText;
};