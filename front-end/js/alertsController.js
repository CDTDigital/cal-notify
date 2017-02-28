var app = angular.module('alertsApp', []);

app.controller('alertsCtrl', function($scope, $filter, $timeout, $http) {
        
    //---------------------------------------------------------------------------------------------------
    //-------------------------------------- I N I T  V A R S -------------------------------------------
    //---------------------------------------------------------------------------------------------------

    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'GIS', '...'];
    $scope.severities = ['Emergency', 'NonEmergency'];
    $scope.alertsLoading = true;

    // API Token used as authorization on each API call
    $scope.apiToken = "";
    $scope.setApiToken = function(token) {
        $scope.apiToken = token;
        $http.defaults.headers.common['Authorization'] = 'Bearer ' + $scope.apiToken;
    };

    var baseApiAddress = "http://api-cal-notify.symsoftsolutions.com";//window.baseApiAddress;

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
            self.location = "";
            self.affected_area = "";
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
            var data = { 
                title: self.title,
                details: self.details,
                location: (typeof self.location == 'object' ? self.location : JSON.parse(self.location)),
                affected_area: (typeof self.affected_area == 'object' ? self.affected_area : JSON.parse(self.affected_area)),
                category: self.category,
                source: self.source,
                severity: self.severity,
                status: self.status
            };
            if(self.published)
                data.published = self.published;
            return data;
        }

        return self;
    };

    $scope.currAlert = new Alert(); // Object used on Create/Edit alert modals

    $scope.newAlert = true; // Used to change UI depending if we're creating or updating an alert
    $scope.alertId = 0;     // Used to keep track of the Id of the alert that's currently being edited

    // Retrieve Alerts

    $scope.getAlerts = function() {
        $http({
            method: 'GET',
            url: baseApiAddress + '/v1/notification',
            headers: { 'Content-Type': 'application/json' }
        }).then(function successCallback(response) {
            $scope.alertsLoading = false;
            // This callback will be called asynchronously when the response is available
            var alerts = response.data.result;
            for(var i = 0, size = alerts.length; i < size; i++) {
                $scope.alerts.push(new Alert(alerts[i], true));
            }
            updateMap();
        }, function errorCallback(response) {
            // Called asynchronously if an error occurs or server returns response with an error status.
            $scope.alertsLoading = false;
        });
    };

    // Alerts CRUD operations

    $scope.createAlert = function() {
        $(".save-alert-btn").button('loading');

        $http({
            method: 'PUT',
            url: baseApiAddress + '/v1/notification',
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify($scope.currAlert.prepareDataForAPI())
        }).then(function successCallback(response) {
            // Create alert based on returned object
            $scope.alerts.splice(0, 0, new Alert(response.data.result, true));
            updateMap();
            closeModal();
        }, function errorCallback(response) {
            showErrorInModal(response.data.meta.message);
        });
    };

    $scope.updateAlert = function() {
        $(".save-alert-btn").button('loading');

        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: $scope.alertId }, true)[0];
        var alertIndex = $scope.alerts.indexOf(alert);
        alert = $scope.currAlert;

        $http({
            method: 'PATCH',
            url: baseApiAddress + '/v1/notification/' + alert.id,
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify($scope.currAlert.prepareDataForAPI())
        }).then(function successCallback(response) {
            // Update alert based on returned object
            alert = new Alert(response.data.result, true);
            $scope.alerts[alertIndex] = alert;
            updateMap();
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
        $("#publish_btn_" + id).button('loading');
        // Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
        var alertIndex = $scope.alerts.indexOf(alert);

        // Broadcast the notification to the affected users
        $http({
            method: 'PUT',
            url: baseApiAddress + '/v1/notification/' + alert.id,
            headers: { 'Content-Type': 'application/json' }
        }).then(function successCallback(response) {
            setTimeout(function() { 
                // Update alert status
                alert.status = "Published";
                alert.published = new Date();
                $scope.alerts[alertIndex] = alert;
                $scope.$apply();
                $("#publish_btn_" + id).button('reset'); 
            }, 2000);
        }, function errorCallback(response) {
            console.log(response.data.meta.message);
            $("#publish_btn_" + id).button('reset');
        });
    };

    $scope.resetAlert = function() {
        $scope.currAlert = new Alert();
    };

    //---------------------------------------------------------------------------------------------------
    //------------------------------ P R I V A T E  M E T H O D S ---------------------------------------
    //---------------------------------------------------------------------------------------------------

    // Map customization

    // Bind infowindows to features
    function onEachFeature(feature, layer) {
        if (feature.properties) {
            layer.bindPopup("<b>" + feature.properties.category + "</b><br/>" +feature.properties.title);
        }
    }

    // Style map features, add category icons
    function getCategoryIcons(feature, latlng) {
        // Marker colors: 'red', 'darkred', 'orange', 'green', 'darkgreen', 'blue', 'purple', 'darkpuple', 'cadetblue'
        // Categories ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
        var categoryIcon = "";
        switch(feature.properties.category) {
            case "Fire":
                categoryIcon = "fire"; break;
            case "Flood":
                categoryIcon = "tint"; break;
            case "Weather":
                categoryIcon = "cloud"; break;
            case "Tsunami":
                categoryIcon = "life-ring"; break;
            case "Earthquake": 
                categoryIcon = "globe"; break;
            default:
                categoryIcon = "medkit";
        }
        var categoryMarker = L.AwesomeMarkers.icon({
            icon: categoryIcon,
            markerColor: (feature.properties.severity == "Emergency" ? 'red' : 'blue')
        });

        return L.marker(latlng, {icon: categoryMarker});
    }

    function updateMap() {
        // Clear geoJSON layer
        if(geoJSONLayer)
            alertsMap.removeLayer(geoJSONLayer);

        // Collect geometries from alerts and create geoJSON
        var geoJSON = { type: "FeatureCollection", features: [] }
        $.each($scope.alerts, function( index, alert ) {
            var newFeature = { type: "Feature", geometry: alert.location, properties: { title: alert.title, category: alert.category, severity: alert.severity } }; 
            geoJSON.features.push(newFeature);
        });

        if(geoJSON.features.length > 0) {
            // Add geoJSON layer to the map
            geoJSONLayer = L.geoJSON(geoJSON, {
                //style: style,
                onEachFeature: onEachFeature,
                pointToLayer: getCategoryIcons
            }).addTo(alertsMap);
            alertsMap.fitBounds(geoJSONLayer.getBounds());
        }
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