// Define the `nowApp` module
var nowApp = angular.module('nowApp', []);

// Define the `NowController` controller on the `nowApp` module
nowApp.controller('NowController', function NowController($scope, $http) {
    var self = this;
    $scope.slots = [];
    $scope.currentSlot = null;
    $scope.nextSlot = null;

    $http.get('slots-feed').then(function (response) {
        $scope.slots = response.data;
        console.log(response.data);
        filterSlots();
    });

    filterSlots = function() {
        var now = new Date();
        now.setDate(30);
        console.log(now);

        if (now < new Date($scope.slots[0].Start)) {
            $scope.currentSlot = null;
            $scope.nextSlot = slots[0];
            return;
        }
        else if (now > new Date($scope.slots[$scope.slots.length - 1].End)) {
            $scope.currentSlot = null;
            $scope.nextSlot = null;
            return;
        }

        for (var n = 0; n < $scope.slots.length; n++) {
            if (new Date($scope.slots[n].Start) <= now && new Date($scope.slots[n].End) >= now) {
                $scope.currentSlot = $scope.slots[n];
                if (n < $scope.slots.length - 1) {
                    $scope.nextSlot = $scope.slots[n + 1];
                }
                else {
                    $scope.nextSlot = null;
                }
                break;
            }
        }
    }

    setInterval(filterSlots, 60000);

});
