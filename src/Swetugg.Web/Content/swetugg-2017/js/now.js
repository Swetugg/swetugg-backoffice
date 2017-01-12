// Define the `nowApp` module
var nowApp = angular.module('nowApp', []);

// Define the `NowController` controller on the `nowApp` module
nowApp.controller('NowController', function NowController($scope) {
    $scope.data = "This is the data.";
});
