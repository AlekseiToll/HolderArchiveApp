(function () {
	'use strict';

	kendo.culture("ru-RU");

	angular.module("HolderArchiveApp")
        .controller("UsersController", UsersController);

    UsersController.$inject = ['$scope', 'toastr'];

    function UsersController($scope, toastr) {
        var vm = this;
        $scope.vm = vm;

        vm.mainGridOptions = {
            dataSource: {
                type: "aspnetmvc-ajax",
                transport: {
                    read: {
                        url: "/Users/Get",
                        dataType: "json",
                        type: "POST"
                    },
                    update: {
                        url: "/Users/Update",
                        dataType: "json",
                        type: "POST"
                    }
                },
                pageSize: 10,
                serverPaging: true,
                serverSorting: true,
                schema: {
                    data: "Data",
                    total: "Total",
                    errors: "Errors",
                    model: {
                        id: "Id",
                        fields: {
                            Id: { editable: false, nullable: false },
                            Login: { editable: false },
                            LastName: { editable: false },
                            FirstName: { editable: false },
                            LaboratoryName: { editable: true  }
                            //Role: { editable: false }
                        }
                    }
                },
                error: function (e) {
                    showErrorMessage(e);
                }
            },
            sortable: {
            	mode: "single",
            	allowUnsort: true
            },
            pageable: true,
            columns: [
                //{ field: "Id", title: "Id", width: "130px" },
                { field: "Login", title: "Логин", width: "100px" },
                { field: "LastName", title: "Фамилия", width: "90px" },
                { field: "FirstName", title: "Имя", width: "90px" },
                { field: "LaboratoryName", title: "Лаборатория", width: "80px", editor: laboratoryDropDownEditor },
                //{ field: "Role", title: "Роль", width: "80px" },
                { command: [{ name: "edit", text: "Изменить" }], title: "&nbsp;", width: "250px" }
            ],
            editable: "inline",
            messages: {
                commands: {
                    cancel: "Отмена"
                }
            }
        };

        function showErrorMessage(e) {
            if (e.errors && e.errors.ErrorMessage) {
                console.log('mainGridOptions  ' + e.errors.ErrorMessage.errors[0]);
                toastr.error(e.errors.ErrorMessage.errors[0], 'Ошибка');
            }
            else if (e.xhr.responseJSON && e.xhr.responseJSON.ErrorMessage) {
                console.log(e.xhr);
                toastr.error(e.xhr.responseJSON.ErrorMessage, 'Ошибка');
            }
            else {
                console.log(e);
                toastr.error('Неизвестная ошибка', 'Ошибка');
            }
        }
    }

    function laboratoryDropDownEditor(container, options) {
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                autoBind: false,
                dataTextField: "Text",
                dataValueField: "Value",
                dataSource: {
                    type: "aspnetmvc-ajax",
                    transport: {
                        read: {
                            url: "/Service/GetLabs",
                            dataType: "json",
                            type: "POST"
                        }
                    },
                    schema: {
                        data: "Data",
                        total: "Total",
                        model: {
                            id: "Value",
                            fields: {
                                Text: { editable: false, type: "string", nullable: false },
                                Value: { editable: false, type: "string", nullable: false }
                            }
                        }
                    }
                }//,
                //optionLabel: {
                //	dataTextField: "Не выбрано",
                //	dataValueField: ""
                //}
            });
    }
})();