//angular.module('alertsApp', [])
app.controller('alertsCtrl', function($scope, $timeout, $http) {
    
    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'Non-Emergency'];

    $scope.alerts = [
    	{id: 1, severity:'Emergency', title:'MAJOR FLOODING HWY 99', description:'ROAD CLOSED NEAR MARYSVILLE', created:new Date(), category:"FLOOD", sent:null, published:false},
    	{id: 2, severity:'Emergency', title:'MAJOR FLOODING HWY 70', description:'ROAD CLOSED ALL POINTS SOUTH OF OROVILLE DAM', created:new Date(), category:"FLOOD", sent:new Date(), published:true},
    	{id: 3, severity:'Non-Emergency', title:'SEVERE WEATHER', description:'WINTER WEATHER ADVISORY FOR TRUCKEE CA', created:new Date(), category:"WEATHER", sent:null, published:false}
    ];

    $scope.newAlert = function() {
        return "";
    };
    /*$timeout(function () {
        $scope.alert = "";
    }, 2000);*/

	/*$http.get("customers.php").then(function(response) {
        $scope.myData = response.data.records;
    });*/

});