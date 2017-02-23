//angular.module('alertsApp', [])
app.controller('alertsCtrl', function($scope, $timeout, $http) {
    
    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'Non-Emergency'];

    $scope.alerts = [
    	{severity:'Emergency', title:'MAJOR FLOODING HWY 99', description:'ROAD CLOSED NEAR MARYSVILLE', created:"Feb 14, 2017", category:"FLOOD", published:false},
    	{severity:'Emergency', title:'MAJOR FLOODING HWY 70', description:'ROAD CLOSED ALL POINTS SOUTH OF OROVILLE DAM', created:"Feb 14, 2017", category:"FLOOD", published:true},
    	{severity:'Non-Emergency', title:'SEVERE WEATHER', description:'WINTER WEATHER ADVISORY FOR TRUCKEE CA', created:"Feb 15, 2017", category:"WEATHER", published:false}
    ];

    /*$scope.newAlert = function() {
        return "";
    };*/
    /*$timeout(function () {
        $scope.alert = "";
    }, 2000);*/

	/*$http.get("customers.php").then(function(response) {
        $scope.myData = response.data.records;
    });*/

});