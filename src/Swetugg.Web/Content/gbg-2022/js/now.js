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

    refreshFeed = function () {
        $http.get('slots-feed').then(function (response) {
            $scope.slots = response.data;
            for (var n = 0; n < $scope.slots.length; n++) {
                $scope.slots[n].Start = Date.parse($scope.slots[n].Start);
                $scope.slots[n].End = Date.parse($scope.slots[n].End);
            }
            console.log(response.data);
            filterSlots();
        })
    };

    filterSlots = function() {
        var now = new Date();
        //now.setMonth(1);
        //now.setDate(3); // For testing purposes
        //now.setHours(11, 30);
        // now.setHours(now.getHours() - 7, now.getMinutes() - 10);
        //console.log(now);

        var nextSlotIndex = 0;
        var currentSlotIndex = null;

        for (var n = 0; n < $scope.slots.length; n++) {
            if ($scope.slots[n].Start <= now && $scope.slots[n].End >= now) {
                currentSlotIndex = n;
            } else if ($scope.slots[n].Start > now) {
                break;
            }

            nextSlotIndex++;
        }

        if (currentSlotIndex != null) {
            $scope.currentSlot = $scope.slots[currentSlotIndex];
        } else {
            $scope.currentSlot = null;
        }

        if (nextSlotIndex < $scope.slots.length) {
            $scope.nextSlot = $scope.slots[nextSlotIndex];
        } else {
            $scope.nextSlot = null;
        }
    }

    refreshFeed();

    $interval(refreshFeed, 30 * 60000);
    $interval(filterSlots, 60000);

});
