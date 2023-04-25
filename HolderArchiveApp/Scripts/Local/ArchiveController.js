(function () {
	'use strict';

	angular.module("HolderArchiveApp")
        .controller("ArchiveController", ArchiveController);

	ArchiveController.$inject = ['$scope', '$http', '$interval', 'toastr'];

	function ArchiveController($scope, $http, $interval, toastr) {
        var vm = this;
        $scope.vm = vm;
        vm.barCode = '';

		// to switch between Archive and LightStand
        $scope.currentMode = { lightStand: false };

        vm.onchangeBarcode = onchangeBarcode;

    	// for LightStand ////////////////////////////
        vm.columns = [];
        vm.rows = [];
        vm.states = { active: 3, processed: 2, empty: 1 };
        vm.lightStandModel = [];

        vm.blink = blink;
        vm.blinkFlag = false;
        vm.confirmFinishApply = confirmFinishApply;
        vm.confirmFinishCancel = confirmFinishCancel;
        vm.finishArchiving = finishArchiving;
        vm.getCell = getCell;
        vm.getStyle = getStyle;
        vm.onchangeBarcode = onchangeBarcode;
        vm.setState = setState;

        var letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH', 'AI', 'AJ', 'AK', 'AL', 'AM', 'AN', 'AO', 'AP', 'AQ', 'AR', 'AS', 'AT', 'AU', 'AV', 'AW', 'AX', 'AY', 'AZ',
                'BA', 'BB', 'BC', 'BD', 'BE', 'BF', 'BG', 'BH', 'BI', 'BJ', 'BK', 'BL', 'BM', 'BN', 'BO', 'BP', 'BQ', 'BR', 'BS', 'BT', 'BU', 'BV', 'BW', 'BX', 'BY', 'BZ',
                'CA', 'CB', 'CC', 'CD', 'CE', 'CF', 'CG', 'CH', 'CI', 'CJ', 'CK', 'CL', 'CM', 'CN', 'CO', 'CP', 'CQ', 'CR', 'CS', 'CT', 'CU', 'CV']; // 100 items
        var lastLightedCell = { row: -1, column: -1 };
		/////////////////////////////////////////////

        $scope.load = function () {
        	
            $http({
                url: '/Archive/GetHoldersForArchive',
                method: "POST"
            }).then(function (response) {
                //console.log(response.data);
                vm.listHolderTypesToArchive = response.data;

                var holderTypesRowLength = 8;
                var holderTypesRowsCount = response.data.length / holderTypesRowLength;
                holderTypesRowsCount = holderTypesRowsCount | 0;
                if (response.data.length % holderTypesRowLength > 0) holderTypesRowsCount += 1;
                var shift = 0;
                var holderTypesRows = [];

                //console.log('holderTypesRowsCount ' + holderTypesRowsCount);
                for (var iRow = 0; iRow < holderTypesRowsCount; iRow++) {

                    var holderTypes = [];
                    for (var iItem = 0; (iItem + shift) < response.data.length && iItem < holderTypesRowLength; iItem++) {
                        var nextHolder = response.data[iItem + shift].NextHolderToArchive;
                        var info;
                        if (nextHolder < 0) {
                            info = response.data[iItem + shift].Info;
                            nextHolder = "";
                        }
                        else {
                            var strNumber = String(nextHolder);
                            while (strNumber.length < 5) strNumber = '0' + strNumber;
                            nextHolder = strNumber;
                            info = "";
                        }
                        holderTypes[iItem] = {
                            Name: response.data[iItem + shift].Name,
                            Color: response.data[iItem + shift].Color,
                            HolderId: nextHolder,
                            Info: info
                        };
                    }

                    holderTypesRows[iRow] = holderTypes.slice();
                    shift += holderTypesRowLength;
                }

                $scope.HolderTypesRows = holderTypesRows;
            });
        }

        init();

        function blink() {
        	vm.blinkFlag = !vm.blinkFlag;
        }

        function confirmFinishCancel() {
        	console.log("confirmFinishCancel " + vm.holderId);
        	vm.barCode = '';
        	vm.confirmFinishWindow.close();
        }

        function confirmFinishApply() {
        	console.log("confirmFinishApply " + vm.holderId);
        	$http({
        		url: '/Archive/ArchiveHolder',
        		method: "POST",
        		params: { holderId: vm.holderId }
        	}).then(function () {
        		toastr.success('Штатив ' + vm.holderId + ' переведен в архив', '');
		        $scope.currentMode.lightStand = false;
	        });

        	vm.barCode = '';
        	vm.confirmFinishWindow.close();
        }

        function finishArchiving() {
        	vm.confirmFinishWindow.center().open();
        }

        function getStyle(cellState) {
        	var defaultStyle = { 'background-color': 'lightgray' };
        	var cellStyle;

        	switch (cellState) {
        		case vm.states.active:
        			cellStyle = vm.blinkFlag ? { 'background-color': 'yellow' } : { 'background-color': 'white' };
        			break;
        		case vm.states.processed:
        			cellStyle = { 'background-color': 'green' };
        			break;
        		case vm.states.empty:
        			cellStyle = defaultStyle; break;
        		default:
        			cellStyle = defaultStyle;
        	}

        	//if (cellIsControl)
        	//    cellStyle['border'] = '1px solid red';

        	return cellStyle;
        }

        function getCell(cells, x, y) {
        	var resultCell = null;
        	angular.forEach(cells, function (cell) {
        		if (cell.x === x && cell.y === y) {
        			resultCell = cell;
        			return;
        		}
        	});
        	return resultCell;
        }

        function init() {
        	vm.setState();

        	$interval(blink, 500);
        }

        function lightNextCell() {
        	var row = lastLightedCell.row;
        	var column = lastLightedCell.column;
        	if (lastLightedCell.row < (vm.rows.length - 1)) {
        		row++;
        	}
        	else if (lastLightedCell.column < (vm.columns.length - 1)) {
        		column++;
        		row = 0;
        	}
        	else {
        		return false;
        	}
        	if (row < 0) row = 0;
        	if (column < 0) column = 0;

        	vm.lightStandModel[row][column].state = vm.states.active;
        	lastLightedCell.row = row;
        	lastLightedCell.column = column;
        	return true;
        }

		function onchangeBarcode(val) {
			//console.log("Barcode value = " + val);

			if ($scope.currentMode.lightStand === false) {
				var dashPos = val.indexOf("-");
				if (dashPos < 0) {
					toastr.error('Неизвестный штрих-код', 'Ошибка');
					return;
				}

				var holderTypeId = val.substring(0, dashPos);
				var holderId = val.substring(dashPos + 1, val.length);
				//console.log("Barcode holderTypeId = " + holderTypeId);
				//console.log("Barcode holderId = " + holderId);

				holderId = parseInt(holderId);
				holderTypeId = parseInt(holderTypeId);

				if (isNaN(holderId) || isNaN(holderTypeId)) {
					toastr.error('Неизвестный штрих-код', 'Ошибка');
					return;
				}

				var holderTypeExists = false;
				var nextHolderId;
				for (var i = 0; i < vm.listHolderTypesToArchive.length; i++) {
					if (vm.listHolderTypesToArchive[i].Id === holderTypeId) {
						holderTypeExists = true;
						nextHolderId = vm.listHolderTypesToArchive[i].NextHolderToArchive;
					}
				}

				if (!holderTypeExists) {
					toastr.error('Неизвестный штрих-код', 'Ошибка');
					return;
				}
				if (nextHolderId === undefined || nextHolderId < 0) {
					toastr.error('Для этого типа штатива нет штативов для архивации', 'Ошибка');
					return;
				}
				if (nextHolderId !== holderId) {
					toastr.error('Возьмите другой штатив', 'Ошибка');
					return;
				}

				vm.holderTypeId = holderTypeId;
				vm.holderId = holderId;

				// paint LightStand
				$http({
					url: '/Archive/GetHolderTypeInfo',
					method: "POST",
					params: { holderTypeId: vm.holderTypeId }
				}).then(function (response) {
					console.log(response.data);
					vm.holderTypeName = response.data.Name;
					vm.holderTypeColor = response.data.Color;

					for (var iCol = 0; iCol < response.data.CountCellsVertical; iCol++) {
						vm.columns[iCol] = letters[iCol];
					}
					for (var iRow = 0; iRow < response.data.CountCellsHorizontal; iRow++) {
						vm.rows[iRow] = iRow + 1;
					}

					for (var iRow = 0; iRow < response.data.CountCellsHorizontal; iRow++) {
						vm.lightStandModel[iRow] = [];
						for (var iCol = 0; iCol < response.data.CountCellsVertical; iCol++) {
							vm.lightStandModel[iRow][iCol] =
							{
								column: iCol,
								row: iRow,
								state: vm.states.empty,
								name: letters[iCol] + String(iRow + 1)
							};
						}
					}
					//console.log("vm.lightStandModel");
					//console.log(vm.lightStandModel);
				});

				console.log('vm.barCode1  ' + vm.barCode);
				vm.barCode = '';
				console.log('vm.barCode2  ' + vm.barCode);
				$scope.currentMode.lightStand = true;
				console.log('$scope.currentMode.lightStand  ' + $scope.currentMode.lightStand);

			}
			else {  // lightStand mode

				if (lastLightedCell.row >= 0 && lastLightedCell.column >= 0)
					vm.lightStandModel[lastLightedCell.row][lastLightedCell.column].state = vm.states.processed;

				$http({
					url: '/Archive/GetValidityOfSample',
					method: "POST",
					params: { barCode: val, holderTypeId: vm.holderTypeId }
				}).then(function (response) {
					var valid = response.data.Valid;
					if (!valid) {
						toastr.error(response.data.Info, 'Возьмите другую пробу');
						return;
					}

					var res = lightNextCell();
					if (!res) {
						toastr.error('Штатив уже заполнен', '');
						return;
					}

					$http({
						url: '/Archive/SaveSampleCoordinates',
						method: "POST",
						params: { holderId: vm.holderId, row: lastLightedCell.row + 1, column: letters[lastLightedCell.column], barCode: val }
					});
				});
			}
		}

        function setState(state) {
        	vm.state = state;
        }
    }
})();