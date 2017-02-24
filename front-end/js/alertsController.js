//angular.module('alertsApp', [])
app.controller('alertsCtrl', function($scope, $filter, $timeout, $http) {
    
    $scope.newAlert = true;
    $scope.alertId = 0;

    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'Non-Emergency'];

    $scope.alerts = [
    	{id: 1, severity:'Emergency', title:'MAJOR FLOODING HWY 99', description:'ROAD CLOSED NEAR MARYSVILLE', created:new Date(), source: "Any", category:"FLOOD", sent:null, published:false, location: {}, area: {}},
    	{id: 2, severity:'Emergency', title:'MAJOR FLOODING HWY 70', description:'ROAD CLOSED ALL POINTS SOUTH OF OROVILLE DAM', created:new Date(), source: "Any", category:"FLOOD", sent:new Date(), published:true, location: {}, area: {}},
    	{id: 3, severity:'Non-Emergency', title:'SEVERE WEATHER', description:'WINTER WEATHER ADVISORY FOR TRUCKEE CA', created:new Date(), source: "Any", category:"WEATHER", sent:null, published:false, location: {}, area: {}}
    ];

    $scope.createAlert = function() {
        var alert = {
        	id: $scope.alerts.length + 1, severity: $scope.alertSeverity, title: $scope.alertTitle, description: $scope.alertDescription, source: $scope.alertSource,
        	created: new Date(), category: $scope.alertCategory, sent:null, published:false, location: $scope.alertCoords, area: $scope.alertAreaCoords }
        $scope.alerts.splice(0, 0, alert);
    };

    $scope.updateAlert = function() {
    	// Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: $scope.alertId }, true)[0];
    	alert.title = $scope.alertTitle;
        alert.severity = $scope.alertSeverity;
        alert.category = $scope.alertCategory;
        alert.source = $scope.alertSource;
        alert.description = $scope.alertDescription;
        alert.location = $scope.alertCoords;
        alert.area = $scope.alertArea;
    };

    $scope.editAlert = function(id) {
    	$scope.newAlert = false;
    	$scope.alertId = id;

    	// Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
    	$scope.alertTitle = alert.title;
        $scope.alertSeverity = alert.severity;
        $scope.alertCategory = alert.category;
        $scope.alertSource = alert.source;
        $scope.alertDescription = alert.description;
        $scope.alertCoords = alert.location;
        $scope.alertArea = alert.area;

        setCoverageMap(typeof alert.location == 'object' ? alert.location : JSON.parse(alert.location));
		$(".alert-modal").modal('show');
    }

    $scope.publishAlert = function(id) {
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
    	alert.published = true;
    };

    $scope.resetAlert = function() {
        $scope.alertTitle = "";
        $scope.alertSeverity = "";
        $scope.alertCategory = "";
        $scope.alertSource = "";
        $scope.alertDescription = "";
        $scope.alertCoords = "";
        $scope.alertArea = "";
    };

    /*$timeout(function () {
        $scope.alert = "";
    }, 2000);*/

	/*$http.get("customers.php").then(function(response) {
        $scope.myData = response.data.records;
    });*/

});