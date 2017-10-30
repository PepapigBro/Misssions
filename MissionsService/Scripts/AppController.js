var app = angular.module("ToDoList", []);



//Начальная инициализация
jQuery(function () {

    //Формат даты
    jQuery('#deadline').datetimepicker({
        format: 'DD-MM-YYYY HH:mm'        
    });



    //Очистка полей ввода
    $('#deadline input').val("");

    //Автоматическая установка фокуса при открытии модального окна создания
    $('#modalCreate').on('shown.bs.modal', function () {
        $('#newTaskName').focus();
    })

    //Запрет ввода вручную в поле Deadline (только через календарь)
    $('#deadline input').keydown(function (e) {
        e.preventDefault()
    });

});


app.controller("PageController", function ($scope, $timeout, $http) {
    $scope.UserTaskList = [];
    $scope.State = {
        Waiting: "Waiting",
        Changed: "Changed",
        Canceled: "Canceled",
        Finished: "Finished"
    }
    $scope.currentEditingTask;
  
    //Открывает диалоговое окно создания или редактирования новой задачи
    $scope.ShowModalCreate = function (headerName, usertask) {
        $('#modalHeader').text(headerName)
        $('#modalCreate').modal({ show: true });

       var newTaskName=$('#newTaskName');
       var newTaskDescription= $('#newTaskDescription');
       var deadlineInput= $('#deadline input');
       var createNewTaskBtn=$('#createNewTaskBtn');
       var editTaskBtn=$('#editTaskBtn')

        //Редактирование
        if (usertask != null) {
            newTaskName.val(usertask.Name);
            newTaskDescription.text(usertask.Description);
            if (usertask.Deadline != null) {
                deadlineInput.val($scope.FormatDate(usertask.Deadline));
            }
            else {
                deadlineInput.val("");
            }
            createNewTaskBtn.hide();
            editTaskBtn.show();
            $scope.currentEditingTask = usertask;
        }
            //Создание
        else {
            //Очистка всех полей ввода
            newTaskName.val("");
            newTaskDescription.text("");
            deadlineInput.val("");
            createNewTaskBtn.show();
            editTaskBtn.hide();
        }

       

    }

    //Принудительная очистка поля Deadline
    $scope.ClearDeadline = function (event) {
        $('#deadline input').val("");
        event.preventDefault();
    }
    
    //Валидация и отправка на сервер запроса на создание новой задачи
    $scope.CreateNewTask = function () {

        var createSaveTaskIndicator = $('.CreateSaveTaskIndicator');
        createSaveTaskIndicator.removeClass('hidden');
        createSaveTaskIndicator.addClass('EndlessSpin');

        //Проверка наличия имени задачи в поле ввода
        var inputName = $("#newTaskName");
        if (inputName.val().trim() == "") {
            inputName.css('border-color', 'darkred');
            return false;
        }
        else { inputName.css('border-color', ''); }
        


        var inputDeadline = $('#deadline input').val();
        var date = null;
        if (inputDeadline != "") {
            date = (moment(inputDeadline, 'DD-MM-YYYY HH:mm').toDate()).toISOString();
        }
        
        var description = $('#newTaskDescription').text().slice(0, 299);
        var usertask = {Id:0, Name: inputName.val(), Description: description, Deadline: date }
   
       
        $http({
            method: 'POST',
            url: '/Missions',
            data: JSON.stringify(usertask),
            headers: { 'Content-Type': 'application/json;odata.metadata=minimal' }            
        }).
        then(function (response) {
            $scope.UserTaskList.splice(0, 0, response.data); //долбавляется в начало списка
           
            $timeout(function () {                
                createSaveTaskIndicator.addClass('hidden');
                createSaveTaskIndicator.removeClass('EndlessSpin');
                $('#modalCreate').modal('hide');
            }, 500)
            

            //$scope.UserTaskList.push(response.data); //Добавление в конец списка
        }, function (error) {
            console.log(error)
            
        });
  
    }

    $scope.EditFieldsTask = function () {
       

        //Проверка наличия имени задачи в поле ввода
        var inputName = $("#newTaskName");
        if (inputName.val().trim() == "") {
            inputName.css('border-color', 'darkred');
            return false;
        }
        else { inputName.css('border-color', ''); }
        
        var inputDeadline=$('#deadline input').val();
        var date = null;
        if (inputDeadline != "") {
            date = moment(inputDeadline, 'DD-MM-YYYY HH:mm').toDate().toISOString();
        }

        var description = $('#newTaskDescription').text().slice(0, 299);
        //var usertask = { Name: inputName.val(), Description: $('#newTaskDescription').html(), Deadline: date }


        var clonedUserTask = $scope.clone($scope.currentEditingTask);
        clonedUserTask.isDeferFromInitial = true;
        clonedUserTask.Name = inputName.val();
        clonedUserTask.Description = description;
        clonedUserTask.Deadline = date;

        $scope.SendEditedTaskToServer(clonedUserTask, $scope.currentEditingTask)
        

    }

    $scope.SendEditedTaskToServer = function (clonedUserTask, usertask) {
        var createSaveTaskIndicator = $('.CreateSaveTaskIndicator');
        createSaveTaskIndicator.removeClass('hidden');
        createSaveTaskIndicator.addClass('EndlessSpin');
       
        $http({
            method: 'PUT',
            url: '/Missions('+usertask.Id+')',
            data: angular.toJson(clonedUserTask),
            headers: { 'Content-Type': 'application/json' }
        }).
    then(function (response) {     
        var index = $scope.UserTaskList.indexOf(usertask);
     //   console.log(response.config.data)
        $scope.UserTaskList[index] = JSON.parse(response.config.data);
           
           $timeout(function () {
             
            createSaveTaskIndicator.addClass('hidden');
            createSaveTaskIndicator.removeClass('EndlessSpin');
            $('#modalCreate').modal('hide');
        }, 500)
            
    }, function (error) {        
        console.log(error)

    });
    }

    $scope.FormatDate = function (date) {
        var s = moment(Date.parse(date), 'DD-MM-YYYY HH:mm').toDate();
       
        var formatted = moment(date).format('DD-MM-YYYY HH:mm');
        return formatted;
        // formatted will be 'Jul 8, 2014'
        
    }

    $scope.getDateTimeNow = function () {
        var datetime = moment(new Date(), 'DD-MM-YYYY HH:mm').toDate();
        return datetime;
    }

    //Валидация и отправка на сервер запроса на создание новой задачи
    $scope.ChangeState = function (usertask, status, dateOfCompletion) {
        var clonedUserTask = $scope.clone(usertask);
        clonedUserTask.TaskState = status;
        clonedUserTask.DateOfCompletion = dateOfCompletion;
        $scope.SendEditedTaskToServer(clonedUserTask, usertask)
    }

    $scope.clone=function(obj) {
        if (null == obj || "object" != typeof obj) return obj;
        var copy = obj.constructor();
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = obj[attr];
        }
        return copy;
    }


    $scope.GetListOfTasks = function () {

        var indicatorList = $('#indicatorList');
        indicatorList.removeClass('hidden');
        indicatorList.addClass('EndlessSpin');        
        
        $http({
            method: 'GET',
            url: '/Missions',            
            headers: { 'Content-Type': 'application/json' }

        }).then(function successCallback(response) {
            var usertaskList = response.data.value;
            usertaskList.forEach(function (item, i, arr) {
                //$scope.UserTaskList.splice(0, 0, item);
               // item.Deadline = 
                $scope.UserTaskList.push(item);
               
                $timeout(function () {
                    indicatorList.addClass('hidden');
                    indicatorList.removeClass('EndlessSpin');

                }, 500)
                
            });

        }, function errorCallback(response) {

        });
    }

    
    $scope.RefreshList = function (event) {
        $scope.UserTaskList.length = 0;
        $scope.GetListOfTasks();
    }
    




})