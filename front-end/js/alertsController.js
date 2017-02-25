//angular.module('alertsApp', [])
app.controller('alertsCtrl', function($scope, $filter, $timeout, $http) {
        
    //---------------------------------------------------------------------------------------------------
    //------------------------------------- C O N S T A N T S -------------------------------------------
    //---------------------------------------------------------------------------------------------------

    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'Non-Emergency'];

    //---------------------------------------------------------------------------------------------------
    //--------------------------------------- A L E R T S -----------------------------------------------
    //---------------------------------------------------------------------------------------------------

    // ------------ A L E R T  M O D E L ------------

    var Alert = function(obj) {
        var self = this;

        if(!obj) {
            self.id = null;
            self.title = "";
            self.details = "";
            self.location = "{}";
            self.affected_area = "{}";
            self.category = "";
            self.source = "";
            self.severity = "";
            self.status = ""; // New, Published, Archived
            self.created = new Date();
            self.published = null;
            self.author_id = null;
            self.published_by = null;
        } else {
            self = obj;
            // Convert string date to date objects
            self.created = (self.created && self.created != "" ? new Date(self.created) : self.created);
            self.published = (self.published && self.published != "" ? new Date(self.published) : self.published);
            // Convert string to JSON object if necessary
            //self.location = (typeof self.location == 'object' ? self.location : JSON.parse(self.location));
            //self.affected_area = (typeof self.affected_area == 'object' ? self.affected_area : JSON.parse(self.affected_area));
        }

        return self;
    };

    $scope.currAlert = new Alert(); // Object used on Create/Edit alert modals

    $scope.newAlert = true; // Used to change UI depending if we're creating or updating an alert
    $scope.alertId = 0;     // Used to keep track of the Id of the alert that's currently being edited

    // Test alerts
    var alert1 = { id: 1, title:'MAJOR FLOODING HWY 99', details:'ROAD CLOSED NEAR MARYSVILLE', location: {}, affected_area: {}, category:"Flood", 
                source: "Any", severity:'Emergency', status: "New", created:new Date(), published:"", author_id:null, published_by:null };
    var alert2 = { id: 2, title:'MAJOR FLOODING HWY 70', details:'ROAD CLOSED ALL POINTS SOUTH OF OROVILLE DAM', location: {}, affected_area: {}, category:"Flood", 
                source: "Any", severity:'Emergency', status: "Published", created:new Date(), published:new Date(), author_id:null, published_by:null };
    var alert3 = { id: 3, title:'SEVERE WEATHER', details:'WINTER WEATHER ADVISORY FOR TRUCKEE CA', location: {}, affected_area: {}, category:"Weather", 
                source: "Any", severity:'Non-Emergency', status: "New", created:new Date(), published:"", author_id:null, published_by:null };
    $scope.alerts = [ new Alert(alert1), new Alert(alert2), new Alert(alert3) ];

    // Alerts CRUD operations

    $scope.createAlert = function() {
        updateLocationFields();
        $scope.alerts.splice(0, 0, $scope.currAlert);
    };

    $scope.updateAlert = function() {
    	updateLocationFields();
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: $scope.alertId }, true)[0];
        alert = $scope.currAlert;
    };

    $scope.editAlert = function(id) {
    	$scope.newAlert = false;
    	$scope.alertId = id;

    	// Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
        $scope.currAlert = new Alert(alert);

        setCoverageMap(typeof $scope.currAlert.location == 'object' ? $scope.currAlert.location : JSON.parse($scope.currAlert.location));
		$(".alert-modal").modal('show');
    }

    $scope.publishAlert = function(id) {
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
    	alert.status = "Published";
        alert.published = new Date();
    };

    $scope.resetAlert = function() {
        $scope.currAlert = new Alert();
    };

    function updateLocationFields() {
        $scope.currAlert.location = $(".coverage-map-coords").val();
        $scope.currAlert.affected_area = $(".coverage-map-area-coords").val();
    }

    //---------------------------------------------------------------------------------------------------
    //------------------------------------ A P I  C A L L S ---------------------------------------------
    //---------------------------------------------------------------------------------------------------

    // API Token used as authorization on each API call
    $scope.apiToken = "";
    $scope.setApiToken = function(token) {
        $scope.apiToken = token;
        $http.defaults.headers.common['Authorization'] = 'Bearer ' + $scope.apiToken;
    };

    $scope.login = function() {

        var data = {
            title: "Example Notification",
            details: "Example notification details",
            location: {
                "type": "Point",
                "coordinates": [
                    -121.51977539062499,
                    38.61901643727865
                ]
            },
            affected_area: {
                "type": "Polygon",
                "coordinates": [
                    [
                    -121.3604736328125,
                    39.11727568585598
                    ],
                    [
                    -122.13226318359375,
                    38.53957267203905
                    ],
                    [
                    -120.9320068359375,
                    38.14535757293734
                    ],
                    [
                    -121.3604736328125,
                    39.11727568585598
                    ]
                ]
            },
            category: "Fire",
            source: "TEST",
            severity: "Emergency",
            status: "New",
            published: "0001-01-01T00:00:00"
        };


		$http({
			method: 'POST',
		 	url: baseApiAddress + '/v1/admin/login',
		 	headers: {
		   		'Content-Type': 'application/json'
		 	},
		 	data: {
			 	email: "testAdmin1@test.com",
			 	password: "123testadmin"
			}
		}).then(function successCallback(response) {
			console.log(response);
		    // this callback will be called asynchronously
		    // when the response is available
		}, function errorCallback(response) {
			console.log(response);
		    // called asynchronously if an error occurs
		    // or server returns response with an error status.
	 	});

    }


});