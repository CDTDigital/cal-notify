//angular.module('alertsApp', [])
app.controller('alertsCtrl', function($scope, $filter, $timeout, $http) {
        
    //---------------------------------------------------------------------------------------------------
    //------------------------------------- C O N S T A N T S -------------------------------------------
    //---------------------------------------------------------------------------------------------------

    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'Non-Emergency'];

    // API Token used as authorization on each API call
    $scope.apiToken = "";
    $scope.setApiToken = function(token) {
        $scope.apiToken = token;
        $http.defaults.headers.common['Authorization'] = 'Bearer ' + $scope.apiToken;
    };

    var baseApiAddress = "http://api-cal-notify.symsoftsolutions.com";

    //---------------------------------------------------------------------------------------------------
    //--------------------------------------- A L E R T S -----------------------------------------------
    //---------------------------------------------------------------------------------------------------

    $scope.alerts = [];

    // ------------ A L E R T  M O D E L ------------

    var Alert = function(obj, apiObj) {
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
            self.status = "New"; // New, Published, Archived
            self.created = new Date();
            self.published = null;
            self.author_id = null;
            self.published_by = null;
        } else {
            self = obj;
            // Convert string date to date objects
            self.created = (self.created && self.created != "" ? new Date(self.created) : self.created);
            self.published = (self.published && self.published != "" ? new Date(self.published) : self.published);

            // Check if object is created from API object
            if(apiObj) {
                // Convert API JSON into valid GeoJSON
                // { "x": 0, "y": 0, "srid": 0 } --> { type: "Point", coordinates: [x, y] }
                if(self.location && typeof self.location == 'object') {
                    self.location = { type: "Point", coordinates: [self.location.x, self.location.y] };
                }
                // [{ "x": 0, "y": 0 }, ...] --> { type: "Polygon", coordinates: [[x, y], ...] }
                if(self.affected_area && typeof self.affected_area == 'object') {
                    var apiCoords = self.affected_area[0], coords = [];
                    for(var i = 0, size = apiCoords.length; i < size; i++) {
                        coords.push([apiCoords[i].x, apiCoords[i].y]);
                    }
                    self.affected_area = { type: "Polygon", coordinates: coords };
                }
            } else {
                // Create JSON object in case it was converted to string
                self.location = (typeof self.location == 'object' ? self.location : JSON.parse(self.location));
                self.affected_area = (typeof self.affected_area == 'object' ? self.affected_area : JSON.parse(self.affected_area));
            }
        }

        self.convertDatesToString = function () {
            self.created = (self.created ? $filter('date')(self.created, "yyyy-MM-dd'T'HH:mm:ss") : self.created);
            self.published = (self.published ? $filter('date')(self.published, "yyyy-MM-dd'T'HH:mm:ss") : self.published);
        }

        self.prepareDataForAPI = function () {
            return { 
                title: self.title, 
                details: self.details,
                location: (typeof self.location == 'object' ? self.location : JSON.parse(self.location)),
                affected_area: (typeof self.affected_area == 'object' ? self.affected_area : JSON.parse(self.affected_area)),
                category: self.category,
                source: self.source,
                severity: self.severity,
                status: self.status,
                published: (!self.published ? "0001-01-01T00:00:00" : self.published)
            };
        }

        return self;
    };

    $scope.currAlert = new Alert(); // Object used on Create/Edit alert modals

    $scope.newAlert = true; // Used to change UI depending if we're creating or updating an alert
    $scope.alertId = 0;     // Used to keep track of the Id of the alert that's currently being edited

    // Test alerts
    /*var alert1 = { id: 1, title:'MAJOR FLOODING HWY 99', details:'ROAD CLOSED NEAR MARYSVILLE', location: {}, affected_area: {}, category:"Flood", 
                source: "Any", severity:'Emergency', status: "New", created:new Date(), published:"", author_id:null, published_by:null };
    var alert2 = { id: 2, title:'MAJOR FLOODING HWY 70', details:'ROAD CLOSED ALL POINTS SOUTH OF OROVILLE DAM', location: {}, affected_area: {}, category:"Flood", 
                source: "Any", severity:'Emergency', status: "Published", created:new Date(), published:new Date(), author_id:null, published_by:null };
    var alert3 = { id: 3, title:'SEVERE WEATHER', details:'WINTER WEATHER ADVISORY FOR TRUCKEE CA', location: {}, affected_area: {}, category:"Weather", 
                source: "Any", severity:'Non-Emergency', status: "New", created:new Date(), published:"", author_id:null, published_by:null };
    $scope.alerts = [ new Alert(alert1), new Alert(alert2), new Alert(alert3) ];*/

    // Retrieve Alerts

    $scope.getAlerts = function() {
        $http({
            method: 'GET',
            url: baseApiAddress + '/v1/notification',
            headers: { 'Content-Type': 'application/json' }
        }).then(function successCallback(response) {
            // This callback will be called asynchronously when the response is available
            var alerts = response.data.result;
            for(var i = 0, size = alerts.length; i < size; i++) {
                $scope.alerts.push(new Alert(alerts[i], true));
            }
        }, function errorCallback(response) {
            // Called asynchronously if an error occurs or server returns response with an error status.
            console.log(response);
        });
    };

    // Alerts CRUD operations

    $scope.createAlert = function() {
        $(".save-alert-btn").button('loading');
        updateLocationFields();

        $http({
            method: 'PUT',
            url: baseApiAddress + '/v1/notification',
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify($scope.currAlert.prepareDataForAPI())
        }).then(function successCallback(response) {
            // Create alert based on returned object
            $scope.alerts.splice(0, 0, new Alert(response.data.result, true));
            closeModal();
        }, function errorCallback(response) {
            showErrorInModal(response.data.meta.message);
        });
    };

    $scope.updateAlert = function() {
        $(".save-alert-btn").button('loading');
    	updateLocationFields();
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: $scope.alertId }, true)[0];
        alert = $scope.currAlert;

        $http({
            method: 'PATCH',
            url: baseApiAddress + '/v1/notification/' + alert.id,
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify($scope.currAlert.prepareDataForAPI())
        }).then(function successCallback(response) {
            // Update alert based on returned object
            alert = new Alert(response.data.result, true);
            closeModal();
        }, function errorCallback(response) {
            showErrorInModal(response.data.meta.message);
        });
    };

    $scope.editAlert = function(id) {
    	$scope.newAlert = false;
    	$scope.alertId = id;

    	// Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
        $scope.currAlert = new Alert(alert);

        var location = typeof $scope.currAlert.location == 'object' ? $scope.currAlert.location : JSON.parse($scope.currAlert.location);
        var area = typeof $scope.currAlert.affected_area == 'object' ? $scope.currAlert.affected_area : JSON.parse($scope.currAlert.affected_area);
        setCoverageMap(location, area);
		$(".alert-modal").modal('show');
    }

    $scope.publishAlert = function(id) {
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
    	alert.status = "Published";
        alert.published = new Date();

        $http({
            method: 'PATCH',
            url: baseApiAddress + '/v1/notification/' + alert.id,
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify(alert.prepareDataForAPI())
        }).then(function successCallback(response) {
            // Update alert based on returned object
            alert = new Alert(response.data.result, true);
        }, function errorCallback(response) {
            console.log(response.data.meta.message);
        });
    };

    $scope.resetAlert = function() {
        $scope.currAlert = new Alert();
    };

    function updateLocationFields() {
        $scope.currAlert.location = $(".coverage-map-coords").val();
        $scope.currAlert.affected_area = $(".coverage-map-area-coords").val();
    }

    function closeModal() { 
        $(".alert-modal").modal('hide');
        $(".save-alert-btn").button('reset');
    }

    function showErrorInModal(msg) {
        $(".save-alert-btn").button('reset');
        $(".modal-error-msg").text(msg);
    }

});