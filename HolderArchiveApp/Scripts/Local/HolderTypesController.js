(function () {
	'use strict';

	kendo.culture("ru-RU");

    //var expandedRowUid = undefined;
    var expandedRowId = undefined;
    var expanded = {};
    var idHolderTypeToWork = undefined;
    var idHolderToDelete = undefined;
    var idHolderTypeToAddNewHolders = undefined;

	angular.module("HolderArchiveApp")
        .controller("HolderTypesController", HolderTypesController);

    HolderTypesController.$inject = ['$scope', '$http', 'toastr'];

	//function expandPreviousRow(grid) {
	//	grid.tbody.find("tr[role='row']").each(function() {
	//		var id = grid.dataItem(this).EmployeeID;

	//		if (expanded.hasOwnProperty(id) && expanded[id]) {
	//			grid.expandRow(this);
	//		}
	//	};
	//}

	function HolderTypesController($scope, $http, toastr) {
        var vm = this;
        $scope.vm = vm;
		var maxCntNewHolders = 300;

        vm.allHoldersToWork = allHoldersToWork;
        vm.addNewHolders = addNewHolders;
        vm.addNewHoldersCancel = addNewHoldersCancel;
        vm.addNewHoldersApply = addNewHoldersApply;
        vm.confirmToWorkCancel = confirmToWorkCancel;
        vm.confirmToWorkApply = confirmToWorkApply;
        vm.confirmDeleteCancel = confirmDeleteCancel;
        vm.confirmDeleteApply = confirmDeleteApply;
        vm.deleteHolderButtonClick = deleteHolderButtonClick;
        vm.holderToWorkButtonClick = holderToWorkButtonClick;
        vm.printHolderButtonClick = printHolderButtonClick;
        vm.printAllNewHoldersButtonClick = printAllNewHoldersButtonClick;

        $(document).ready(function () {
	        vm.numericTextBox = $("#numericTextBox").kendoNumericTextBox({
	        	min: 1,
	        	max: maxCntNewHolders,
	        	value: 1,
	        	format: "n0",
	        	decimals: 0
	        }).data("kendoNumericTextBox");
	        $("#numericTextBox").attr("type", "number");
        });

        vm.detailGridOptions = function (dataItem) {
        	var currentColor = dataItem.ColorAsString.slice(1);
        	var holderTypeId = dataItem.Id;
        	return {
        		dataSource: {
        			type: "aspnetmvc-ajax",
        			transport: {
        				read: {
        					url: "/HolderTypes/GetHoldersByHolderType",
        					dataType: "json",
        					type: "POST",
        					data: { id: holderTypeId }
        				}
        			},
        			serverPaging: true,
        			serverSorting: true,
        			serverFiltering: true,
        			pageSize: 10,
        			schema: {
        				data: "Data",
        				total: "Total",
        				model: {
        					id: "Id",
        					fields: {
        						Id: { editable: false, nullable: false },
        						StatusAsString: { editable: false }
        					}
        				}
        			},
        			error: function (e) {
        				showErrorMessage(e);
        			}
        		},
        		scrollable: false,
        		pageable: true,
        		columns: [
                    { field: "Id", title: "Id", width: "110px" },
                    { field: "StatusAsString", title: "Статус", width: "110px" },
                    {
                    	template: function (dataItem) {
                    		if (dataItem.StatusAsString === 'Новый') {
                    			return "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.printHolderButtonClick(" + dataItem.Id + ", " + holderTypeId + ", '" + currentColor + "')\">Распечатать</button> " +
                                    "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.deleteHolderButtonClick(" + dataItem.Id + ")\">Удалить</button> " +
                                    "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.holderToWorkButtonClick(" + dataItem.Id + ")\">В работу</button>";
                    		} else {
                    			return "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.printHolderButtonClick(" + dataItem.Id + ", " + holderTypeId + ", '" + currentColor + "')\">Распечатать</button> " +
                                "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.deleteHolderButtonClick(" + dataItem.Id + ")\">Удалить</button>";
                    		}
                    	}
                    }
        		],
        		dataBound: function () {
        			$("#btnIncrease").kendoTooltip({
        				content: "Увеличить количество штативов",
        				position: "right"
        			});
        		},
        		toolbar: [
                    {
                    	template:
                            "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.allHoldersToWork(" + holderTypeId + ")\">Все в работу</button> " +
                            "<button type=\"button\" class=\"k-button\" ng-click=\"hvm.printAllNewHoldersButtonClick(" + holderTypeId + ", '" + currentColor + "')\">Печать новых</button>" +
                            "<button id=\"btnIncrease\" type=\"button\" class=\"k-button\" ng-click=\"hvm.addNewHolders(" + holderTypeId + ")\">Увеличить</button> "
                    }
        		]
        	}
        };

        vm.mainGridOptions = {
        	dataSource: {
        		type: "aspnetmvc-ajax",
        		transport: {
        			read: {
        				url: "/HolderTypes/Get",
        				dataType: "json",
        				type: "POST"
        			},
        			update: {
        				url: "/HolderTypes/Update",
        				dataType: "json",
        				type: "POST"
        			},
        			create: {
        				url: "/HolderTypes/Create",
        				dataType: "json",
        				type: "POST",
        				complete: function (e) {
        					updateGrid();
        				}
        			},
        			parameterMap: function (data, operation) {
        				// Здесь нужны все эти конвертации, т.к. по непонятной причине при передачи даты в момент конвертации
        				// из JSON в C# DateTime на разных машинах дата конвертируется по-разному (меняется местами день и месяц)
        				// надежнее передать строку
        				if (operation === "update" || operation === "create") {
        					data.CreatedOn = kendo.toString(data.CreatedOn, 'dd-MM-yyyy HH:mm:ss');
        					data.ChangedOn = kendo.toString(data.ChangedOn, 'dd-MM-yyyy HH:mm:ss');
        				}
        				if (operation === "read") {
        					data.CreatedOn = kendo.parseDate(data.CreatedOn, 'dd-MM-yyyy HH:mm:ss');
        					data.ChangedOn = kendo.parseDate(data.ChangedOn, 'dd-MM-yyyy HH:mm:ss');
        				}
        				//return data;
        				return kendo.stringify(data);
        			}
        		},
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
        					Id: { editable: false, nullable: false },
        					Name: { editable: true, validation: { required: true } },
        					ContainerTypes: { editable: true, defaultValue: [] },
        					CountCellsHorizontal: { editable: true, type: "number", format: "n0", decimals: 0, validation: { required: true, min: 1, max: 100 } },
        					CountCellsVertical: { editable: true, type: "number", format: "n0", decimals: 0, validation: { required: true, min: 1, max: 100 } },
        					TimeLimit: { editable: true, type: "number", format: "n0", decimals: 0, validation: { required: true, min: 1, max: 9125 } },
        					LaboratoryName: { editable: true },
        					ChangedOn: { editable: false, type: "string" },
        					CreatedOn: { editable: false, type: "string" },
        					ColorAsString: { editable: true, type: "string", defaultValue: "#FFFFFF" },
        					CountNewAndOtherHolders: { editable: false, type: "string" }
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
        	filterable: true,
        	pageable: true,
        	//detailExpand: function (e) {
        	//    expandedRowUid = e.masterRow.data('uid');
        	//},
        	detailExpand: function (e) {
        		expandedRowId = this.dataItem(e.masterRow).Id;
        		expanded[expandedRowId] = true;
        	},
        	detailCollapse: function (e) {
        		expandedRowId = this.dataItem(e.masterRow).Id;
        		expanded[expandedRowId] = false;
        	},
        	dataBound: function (e) {
        		var grid = this;
        		grid.tbody.find("tr[role='row']").each(function () {
        			var id = grid.dataItem(this).Id;

        			if (expanded.hasOwnProperty(id) && expanded[id]) {
        				grid.expandRow(this);
        			}
        		});
        	},
        	columns: [
		        {
		        	field: "Id", title: "Id", width: "40px",
		        	filterable: {
		        		extra: false,
		        		operators: {
		        			string: {
		        				eq: "Равно"
		        			}
		        		}
		        	}
		        },
                {
                	field: "Name",
                	title: "Тип штатива",
                	width: "90px",
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
                	field: "ColorAsString",
                	title: "Цвет",
                	width: "60px",
                	template: "<div style='background-color: #= ColorAsString #'>&nbsp;</div>",
                	editor: colorEditor,
                	sortable: false,
					filterable: false
                },
				{
					field: "ContainerTypes",
					title: "Тип контейнера",
					width: "200px",
					template: "#= ContainerTypes != null ? ContainerTypes.join(', ') : '' #",
					editor: containerEditor,
					sortable: false,
					filterable: false
				},
                { field: "CountCellsHorizontal", title: "Кол-во ячеек по горизонтали", width: "60px", filterable: false },
                { field: "CountCellsVertical", title: "Кол-во ячеек по вертикали", width: "60px", filterable: false },
                { field: "TimeLimit", title: "Срок", width: "60px", filterable: false },
		        {
		        	field: "LaboratoryName", title: "Лаборатория", width: "80px", editor: laboratoryDropDownEditor,
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
		        	field: "CountNewAndOtherHolders", title: "Кол-во", width: "60px",
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
                	field: "ChangedOn",
                	title: "Дата изменения",
                	width: "120px",
                	//template: "#= kendo.parseDate(ChangedOn, 'dd-MM-yyyy HH:mm:ss') #",
                	format: "{0:dd-MM-yyyy HH:mm:ss}", filterable: false
                },
                {
                	field: "CreatedOn",
                	title: "Дата создания",
                	width: "120px",
                	format: "{0:dd-MM-yyyy HH:mm:ss}", filterable: false
                },
                { command: ["edit"], title: "&nbsp;", width: "180px" }
        	],
        	editable: "inline",
        	toolbar: ["create"],
        	edit: function (e) {
        		if (e.model.isNew()) {
        			$(".k-grid-edit-row").on("click", ".k-hierarchy-cell", function (e) {
        				e.stopPropagation();
        			});
        		}
        	}
        };

        function addNewHoldersCancel() {
        	vm.addNewHoldersWindow.close();
        }

        function addNewHoldersApply() {
            console.log("addNewHoldersApply " + idHolderTypeToAddNewHolders);
            var cntNewHolders = ($("#numericTextBox").val());
            $http({
                url: '/HolderTypes/AddHoldersToType',
                method: "POST",
                params: { holderTypeId: idHolderTypeToAddNewHolders, count: cntNewHolders }
            }).then(function () {
            	updateGrid();
            });
            vm.addNewHoldersWindow.close();
        }

        function addNewHolders(holderTypeId) {
        	console.log("addNewHolders: " + holderTypeId);
        	$http({
        		url: '/HolderTypes/GetCountNewHoldersForHolderType',
        		method: "POST",
        		params: { holderTypeId: holderTypeId }
        	}).then(function (response) {
        		var count = response.data.Count;
        		//console.log("addNewHolders maxCntNewHolders - count: " + maxCntNewHolders - count);
        		if ((maxCntNewHolders - count) > 0) {
        			idHolderTypeToAddNewHolders = holderTypeId;
        			//vm.numericTextBox.set({ max: maxCntNewHolders - count });
        			vm.numericTextBox.options.max = maxCntNewHolders - count;

        			$("#numericTextBox").data("kendoNumericTextBox").value(1);
        			vm.addNewHoldersWindow.center().open();
        		} else {
        			toastr.info('В этом типе уже содержится 300 штативов, вы не можете добавить больше.');
        		}
        	});
        }

        function allHoldersToWork(holderTypeId) {
            console.log("allHoldersToWork: " + holderTypeId);
            idHolderTypeToWork = holderTypeId;
            $http({
                url: '/HolderTypes/GetCountNewHoldersForHolderType',
                method: "POST",
                params: { holderTypeId: holderTypeId }
            }).then(function (response) {
                var count = response.data.Count;
                if (count > 0) {
                    vm.confirmToWorkText = 'Перевести в статус "Пустой" следующие штативы: ' + count + '?';
                    vm.confirmToWorkWindow.center().open();
                } else {
                    toastr.info('Для этого типа нет новых штативов', '');
                }
            });
        }

        function confirmToWorkCancel() {
            vm.confirmToWorkWindow.close();
        }

        function confirmToWorkApply() {
            console.log("confirmToWorkApply " + idHolderTypeToWork);
            $http({
                url: '/HolderTypes/AllToWork',
                method: "POST",
                params: { holderTypeId: idHolderTypeToWork }
            }).then(function () {
            	updateGrid();
            });
            vm.confirmToWorkWindow.close();
        }

        function confirmDeleteCancel() {
            vm.confirmDeleteWindow.close();
        }

        function confirmDeleteApply() {
            console.log("confirmDeleteApply " + idHolderToDelete);
            $http({
                url: '/HolderTypes/DeleteHolderConfirmed',
                method: "POST",
                params: { holderId: idHolderToDelete }
            }).then(function () {
            	updateGrid();
            });

            vm.confirmDeleteWindow.close();
        }

        function deleteHolderButtonClick(id) {
            idHolderToDelete = id;
            console.log("deleteHolderButtonClick: " + id);
            vm.confirmDeleteText = 'Удалить штатив ' + id + '?';
            vm.confirmDeleteWindow.center().open();
        }

        function holderToWorkButtonClick(id) {
            console.log("holderToWorkButtonClick: " + id);
            $http({
                url: '/HolderTypes/OneHolderToWork',
                method: "POST",
                params: { holderId: id }
            }).then(function () {
            	updateGrid();
                //if (expandedRowUid) {
                //    grid.expandRow($('tr[data-uid=' + expandedRowUid + ']'));
                //}
            });
        }

        function printHolders(currentColor, ids, holderTypeId, winPrint) {
        	console.log("printHolders https");
            if (winPrint != null) {
                winPrint.document.write('<html><head>');
                winPrint.document.write("<script src='https://code.jquery.com/jquery-1.10.2.min.js'><\/script>");
                winPrint.document.write("<script src='https://cdn.jsdelivr.net/jsbarcode/3.5.8/JsBarcode.all.min.js'><\/script>");
                winPrint.document.write('</head>');
                winPrint.document.write('<body>');
                winPrint.document.write('<style>');
                winPrint.document.write('.holderid { font: "Arial Narrow", Arial, sans-serif; font-size: 33px; }');
                winPrint.document.write('</style>');

                var rowsCount = ids.length / 3;
                rowsCount = rowsCount | 0;
                if (ids.length % 3 > 0) rowsCount += 1;
                var totalCount = ids.length;
                var currentIndex = 0;
                var columnsCount = 3;

                winPrint.document.write('<table border="1" cellspacing="0" cellpadding="0" align="center" width="180mm">');

                for (var iRow = 0; iRow < rowsCount; iRow++) {
                    winPrint.document.write('<tr>');
                    for (var iColumn = 0; iColumn < columnsCount && currentIndex < totalCount; iColumn++) {
                        var strNumber = String(ids[currentIndex]);
                        while (strNumber.length < 5) strNumber = '0' + strNumber;

                        winPrint.document.write('<td height="80px">');

                        var strBarCode = holderTypeId + '-' + ids[currentIndex];
                        //console.log('strBarCode ' + strBarCode);
                        var idBarcode = 'barcode' + iRow + iColumn;

                        // inner table
                        //winPrint.document.write('<table border="1" cellspacing="0" cellpadding="0" align= "center" width="58mm">');
                        winPrint.document.write('<table align="center" width="235px">');
                        winPrint.document.write('<tr><td align="center" height="25px"><div class="holderid">' + strNumber + '</div></td></tr>');
                        winPrint.document.write('<tr><td align="center" height="20px"><div style="background-color:' + currentColor + ';width:40mm;height:25px;"></div></td></tr>');
                        winPrint.document.write('<tr><td align="center" height="22px"><div><script>$(document).ready(function() {JsBarcode("#' + idBarcode + '", "' + strBarCode + '", { height: 25, displayValue: false });});<\/script><svg id="' + idBarcode + '"></svg></div></td></tr>');
                        winPrint.document.write('</table>');
                        // end of inner table

                        winPrint.document.write('<td>');

                        currentIndex++;
                    }
                    winPrint.document.write('</tr>');
                }
                winPrint.document.write('<script>$(window).load(function() {window.print();});<\/script>');
                winPrint.document.write('</body></html>');
                winPrint.document.close();
                winPrint.focus();
            }
        }

        function printHolderButtonClick(id, holderTypeId, currentColor) {
            console.log("printHolderButtonClick: id = " + id + ", holderTypeId = " + holderTypeId + ", currentColor = " + currentColor);
            var ids = new Array();
            ids[0] = id;
            var winPrint = window.open();
            printHolders(currentColor, ids, holderTypeId, winPrint);
        }

        function printAllNewHoldersButtonClick(holderTypeId, currentColor) {
        	//console.log("printAllNewHoldersButtonClick: holderTypeId = " + holderTypeId + ", currentColor = " + currentColor);
        	var grid = $("#MainGrid").data("kendoGrid");
        	var data = grid.dataSource.data();
			// get count of new holders from the table
	        var countNewAndOtherHolders;
        	$.each(data, function (i, row) {
        		if (row.Id === holderTypeId) {
        			countNewAndOtherHolders = row.CountNewAndOtherHolders;
		        }
        	});
        	if (countNewAndOtherHolders == undefined) {
        		toastr.error('Не удалось получить кол-во штативов для данного типа', 'Ошибка');
		        return;
	        }
        	var slashPos = countNewAndOtherHolders.indexOf("/");
        	if (slashPos < 0) {
        		toastr.error('Не удалось получить кол-во штативов для данного типа: ' + countNewAndOtherHolders, 'Ошибка');
        		return;
        	}
        	var cntNewHolders = countNewAndOtherHolders.substring(0, slashPos);
        	//console.log("cntNewHolders = " + cntNewHolders);
        	if (isNaN(cntNewHolders)) {
        		toastr.error('Не удалось получить кол-во штативов для данного типа: ' + countNewAndOtherHolders, 'Ошибка');
        		return;
        	}

        	if (cntNewHolders < 1) {
        		toastr.info('Для этого типа нет новых штативов', '');
		        return;
	        }

            var winPrint = window.open();
            $http({
                url: '/HolderTypes/GetNewHoldersForHolderType',
                method: "POST",
                params: { holderTypeId: holderTypeId }
            }).then(function (response) {
                var ids = response.data.Array;
                if (ids.length > 0) {
                    printHolders(currentColor, ids, holderTypeId, winPrint);
                } else {
                    alert('Не удалось получить id для новых штативов');
                }
            });
        }

        function showErrorMessage(e) {
	        console.log('holderTypes showErrorMessage');
	        console.log(e);
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

    function colorEditor(container, options) {
        var input = $("<input/>");
        input.attr("name", options.field);
        input.appendTo(container);
        // initialize a Kendo UI AutoComplete
        input.kendoColorPicker({
            //palette: 'basic',
            value: "#FFFFFF",
            buttons: false
        });
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
                }
            });
    }
})();