var app = angular.module("ToDoList");


app.directive('showAllContentMouseover', function () {
    return function (scope, element, attrs) {
        $(element).on('mouseover', function () {
            var div = $(element);
            div.removeClass('trimmed');
            div.addClass('fullVersion');          
           
        })
        $(element).on('mouseout', function () {
            var div = $(element);
            div.removeClass('fullVersion');
            div.addClass('trimmed');

        })
    }
})