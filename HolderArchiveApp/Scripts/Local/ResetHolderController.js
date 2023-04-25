(function () {
	'use strict';

	kendo.culture("ru-RU");

	angular.module("HolderArchiveApp")
        .controller("ResetHolderController", ResetHolderController);

    ResetHolderController.$inject = ['$scope', '$http', 'toastr'];

    function ResetHolderController($scope, $http, toastr) {
        var vm = this;
        $scope.vm = vm;

	    vm.barCode = '';
        vm.holderResetCancel = holderResetCancel;
        vm.holderResetApply = holderResetApply;
        vm.onchangeBarcode = onchangeBarcode;
        vm.updateData = updateData;

        var idsOfHoldersToReset = [];
        var idsOfHoldersAlreadyReset = [];

	    setInterval(showCurrentDate, 0);
	    vm.dateOfLastUpdate = dateToString(new Date);

	    vm.mainGridOptions = {
            dataSource: {
                type: "aspnetmvc-ajax",
                transport: {
                    read: {
                    	url: "/ResetHolder/Get",
                        dataType: "json",
                        type: "POST",
                        complete: function (e) {
                        	console.log("read complete");
                        	var grid = $("#MainGrid").data("kendoGrid");
                        	// get and save ids of holders
                        	idsOfHoldersToReset = [];
                        	var data = grid.dataSource.data();
                        	$.each(data, function (i, row) {
                        		idsOfHoldersToReset.push(row.Id);
                        	});
                        	console.log("read complete res = " + idsOfHoldersToReset.length);
                        }
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
                        	HolderTypeName: { editable: false },
                            Id: { editable: false, nullable: false },
                            ArchivedOn: { editable: false, type: "date" },
                            TimeLimit: { editable: false, type: "number" }
                        }
                    }
                },
                error: function (e) {
                    showErrorMessage(e);
                }
            },
            sortable: true,
            pageable: true,
            columns: [
				{ field: "HolderTypeName", title: "Тип", width: "120px" },
                { field: "Id", title: "Номер", width: "50px" },
				{
					field: "ArchivedOn",
					title: "Дата архивации",
					width: "120px",
					template: "#= kendo.toString(kendo.parseDate(ArchivedOn, 'dd-MM-yyyy HH:mm:ss'), 'dd-MM-yyyy HH:mm:ss') #"
				},
                { field: "TimeLimit", title: "Срок хранения", width: "120px" }
            ]
        };

        function dateToString(dt) {
        	var day = dt.getDate();
        	var month = dt.getMonth() + 1;
        	var year = dt.getFullYear();
        	var hours = dt.getHours();
        	var minutes = dt.getMinutes();
        	var seconds = dt.getSeconds();
        	if (day <= 9) day = '0' + day;
        	if (month <= 9) month = '0' + month;
        	if (hours <= 9) hours = '0' + hours;
        	if (minutes <= 9) minutes = '0' + minutes;
        	if (seconds <= 9) seconds = '0' + seconds;
        	return day + '.' + month + '.' + year + '  ' + hours + ':' + minutes + ':' + seconds;
        }

        function holderResetCancel() {
	        vm.barCode = '';
        	vm.resetHolderWindow.close();
        }

        function holderResetApply() {
        	//console.log("holderResetApply " + vm.idOfHolderToReset);
        	$http({
        		url: '/ResetHolder/Reset',
        		method: "POST",
        		params: { holderId: vm.idOfHolderToReset }
        	}).then(function () {
        		toastr.success('Штатив ' + vm.idOfHolderToReset + ' успешно сброшен', '');
        		idsOfHoldersAlreadyReset.push(vm.idOfHolderToReset);
        		// paint the row in the table
        		var grid = $("#MainGrid").data("kendoGrid");
        		var data = grid.dataSource.data();
        		$.each(data, function (i, row) {
        			if (row.Id === vm.idOfHolderToReset) {
        				$('tr[data-uid="' + row.uid + '"] ').css("background-color", "#e0e4e5");
        				$('tr[data-uid="' + row.uid + '"] ').css("color", "#939495");
        			}
        		});
        	});
        	vm.resetHolderWindow.close();
        }

        function onchangeBarcode(val) {
        	console.log("Barcode value = " + val);
        	console.log("vm.barCode = " + vm.barCode);

        	var dashPos = val.indexOf("-");
        	if (dashPos < 0) {
        		toastr.error('Неизвестный штрих-код', 'Ошибка');
        		return;
        	}

        	vm.holderTypeId = val.substring(0, dashPos);
        	vm.idOfHolderToReset = val.substring(dashPos + 1, val.length);
        	//console.log("Barcode holderTypeId = " + holderTypeId);
        	//console.log("Barcode holderId = " + idOfHolderToReset);

        	vm.idOfHolderToReset = parseInt(vm.idOfHolderToReset);
        	vm.holderTypeId = parseInt(vm.holderTypeId);

        	if (isNaN(vm.idOfHolderToReset) || isNaN(vm.holderTypeId)) {
        		toastr.error('Неизвестный штрих-код', 'Ошибка');
        		return;
        	}

        	// check if this holder can be reset
        	if (idsOfHoldersToReset.indexOf(vm.idOfHolderToReset) === -1) {
        		toastr.error('Штатив не подлежит сбросу', 'Ошибка');
        		return;
        	}
        	if (idsOfHoldersAlreadyReset.indexOf(vm.idOfHolderToReset) !== -1) {
        		toastr.error('Этот штатив уже сброшен', 'Ошибка');
        		return;
        	}

			// get holder type color for the modal window
        	$http({
        		url: '/ResetHolder/GetColorForHolder',
        		method: "POST",
        		params: { holderId: vm.idOfHolderToReset }
        	}).then(function (response) {
        		vm.colorOfHolderToReset = response.data.Color;
        	});
        	
        	vm.resetHolderWindow.center().open();
        }

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

        function showCurrentDate() {
        	var curDate = dateToString(new Date);
        	document.getElementById('curtime').innerHTML = curDate;
        }

        function updateData() {
	        updateGrid();
	        vm.dateOfLastUpdate = dateToString(new Date);
        }

        function updateGrid() {
        	var grid = $("#MainGrid").data("kendoGrid");
        	grid.dataSource.read();
        	grid.refresh();
        	// get and save ids of holders
        	idsOfHoldersToReset = [];
	        idsOfHoldersAlreadyReset = [];
        	var data = grid.dataSource.data();
        	$.each(data, function (i, row) {
		        idsOfHoldersToReset.push(row.Id);
		    });
        }
   }
})();