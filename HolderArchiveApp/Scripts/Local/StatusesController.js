(function () {
	'use strict';

	kendo.culture("ru-RU");

	angular.module("HolderArchiveApp")
        .controller("StatusesController", StatusesController);

    StatusesController.$inject = ['$scope', 'toastr'];

    function StatusesController($scope, toastr) {
        var vm = this;
        $scope.vm = vm;

        vm.mainGridOptions = {
            dataSource: {
                type: "aspnetmvc-ajax",
                transport: {
                    read: {
                        url: "/Statuses/Get",
                        dataType: "json",
                        type: "POST"
                    },
                    update: {
                        url: "/Statuses/Update",
                        dataType: "json",
                        type: "POST"
                    },
                    create: {
                        url: "/Statuses/Create",
                        dataType: "json",
                        type: "POST",
                        complete: function (e) {
                        	updateGrid();
                        }
                    },
                    parameterMap: function (data, type) {
                    	return kendo.stringify(data);
                    }
                },
                //filter: [
				//   // leave data items which are "Beverage" and not "Coffee"
				//   { field: "Id", operator: "eq", value: "Beverages" },
				//   { field: "Workflow", operator: "contains", value: "Coffee" }
                //],
                pageSize: 10,
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                schema: {
                    data: "Data",
                    total: "Total",
                    errors: "Errors",
                    model: {
                        id: "Id",
                        fields: {
                        	Id: { editable: false, nullable: false }, //, type: "number" },
                        	Workflow: { editable: true, type: "string", validation: { required: true } },
                            ContainerTypes: { editable: true, defaultValue: [], validation: { required: true } },
                            Statuses: { editable: true, defaultValue: [], validation: { required: true } }
                        }
                    }
                },
                error: function (e) {
                	showErrorMessage(e);

	                //if (e.status === "customerror") {
	                //    var grid = $("#MainGrid").data("kendoGrid");
	                //    $.each(e.errors, function (key, value) {
	                //        if (key === "" && value.errors && value.errors.length > 0) {
	                //            $.helixNotify(value.errors[0], { type: "Error" });
	                //        }
	                //        else {
	                //            if (value.errors && value.errors.length > 0) {
	                //                var gridElement = grid.editable.element;
	                //                gridElement.find("[data-container-for=" + key + "]")
	                //                    .append(validationMessageTemplate({ field: key, message: value.errors[0] }));
	                //            }
	                //        }
	                //    });
	                //    grid.one("dataBinding", function (e) {
	                //        e.preventDefault();
	                //    });
	                //}
                }
            },
            sortable: {
            	mode: "single",
            	allowUnsort: true
            },
            filterable: true,
            pageable: true,
            columns: [
	            {
		            field: "Id", title: "Id", width: "50px",
		            filterable: {
		            	extra: false,
		            	operators: {
		            		string: {
		            			eq: "Равно"		            		}
		            	}
		            }
	            	//filterable: {
	            	//	cell: {
	            	//		extra: false,
        			//		showOperators: false,
        			//		operator: "eq"
        			//	}
					//}
	            },
	            {
	            	field: "Workflow", title: "Рабочий поток", width: "120px", editor: workflowDropDownEditor,
	            	filterable: {
	            		extra: false,
	            		operators: {
	            			string: {
	            				eq: "Равно",
	            				contains: "Содержит"
	            			}
	            		}
	            	}
	            },
	            {
	            	field: "ContainerTypes",
	            	title: "Тип контейнера",
	            	width: "250px",
	            	template: "#= ContainerTypes != null ? ContainerTypes.join(', ') : '' #",
	            	editor: containerEditor,
	            	sortable: false,
	            	filterable: false
	            },
	            {
	            	field: "Statuses",
	            	title: "Статусы",
	            	width: "120px",
	            	template: "#= Statuses != null ? Statuses.join(', ') : '' #",
	            	editor: statusesEditor,
	            	sortable: false,
	            	filterable: {
	            		extra: false,
	            		operators: {
	            			string: {
	            				eq: "Равно",
	            				contains: "Содержит"
	            			}
	            		}
	            	}
	            },
                //{ command: ["edit", "destroy"], title: "&nbsp;", width: "250px" }
                { command: [{ name: "edit", text: "Изменить" }], title: "&nbsp;", width: "250px" }
            ],
            editable: "inline",
            toolbar: ["create"],
            messages: {
                commands: {
                    cancel: "Отмена",
                    create: "Добавить"
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

        function updateGrid() {
        	var grid = $("#MainGrid").data("kendoGrid");
        	grid.dataSource.read();
        	grid.refresh();
        }
   }

function containerEditor(container, options) {
    $('<select multiple="multiple" data-bind="value :' + options.field + '"/>')
            .appendTo(container)
            .kendoMultiSelect({
                autoBind: false,
                valuePrimitive: true,
                dataTextField: "Text",
                dataValueField: "Value",
                dataSource: {
                    type: "aspnetmvc-ajax",
                    transport: {
                        read: {
                            url: "/Service/GetContainerTypes",
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
                                Text: { type: "string" },
                                Value: { type: "string" }
                            }
                        }
                    }
                },
                filter: "contains"
            });
}

function statusesEditor(container, options) {
    $('<select multiple="multiple" data-bind="value :' + options.field + '"/>')
            .appendTo(container)
            .kendoMultiSelect({
                dataSource: ["A", "X", "C", "U", "I", "P"]
            });
}

function workflowDropDownEditor(container, options) {
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
                        url: "/Statuses/GetWorkflows",
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
            },
            filter: "contains"
        });
}

})();