// Define the `nowApp` module
var nowApp = angular.module('nowApp', ['ng-trunk8']).config(function (trunk8ConfigProvider) {
    trunk8ConfigProvider.setConfig({
        lines: 2
    })
});

// Define the `NowController` controller on the `nowApp` module
nowApp.controller('NowController', function NowController($scope, $http, $interval) {
    var self = this;
    $scope.slots = [];
    $scope.currentSlot = null;
    $scope.nextSlot = null;

    $http.get('slots-feed').then(function (response) {
        $scope.slots = response.data;
        for (var n = 0; n < $scope.slots.length; n++) {
            $scope.slots[n].Start = Date.parse($scope.slots[n].Start);
            $scope.slots[n].End = Date.parse($scope.slots[n].End);
        }
        console.log(response.data);
        filterSlots();
    });

    filterSlots = function() {
        var now = new Date();
        now.setDate(30); // For testing purposes
        console.log(now);

        var nextSlotIndex = 0;
        var currentSlotIndex = null;

        for (var n = 0; n < $scope.slots.length; n++) {
            console.log(n);
            if ($scope.slots[n].Start <= now && $scope.slots[n].End >= now) {
                currentSlotIndex = n;
            } else if ($scope.slots[n].Start > now) {
                break;
            }

            nextSlotIndex++;
        }

        if (currentSlotIndex != null) {
            $scope.currentSlot = $scope.slots[currentSlotIndex];
        }

        if (nextSlotIndex < $scope.slots.length) {
            $scope.nextSlot = $scope.slots[nextSlotIndex];
        }
    }

    $interval(filterSlots, 60000);

})
    .directive('slot', function() {
        return {
            template: 'Slot: {{ slot.Start }}'
        };
});
