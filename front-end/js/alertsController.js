var app = angular.module('alertsApp', []);

// CUSTOM FILTER FOR ALERTS
app.filter("criteriaMatch", function() {

    function categoryCriteria(alert, filters) {
        if(typeof filters.category === 'undefined' || filters.category == "Any")
            return true;
        else
            return alert.category === filters.category;
    }

    function severityCriteria(alert, filters) {
        var match = false;

        if(!filters.severityEmergency && !filters.severityNonEmergency) {
            return true;
        } else {
            if(filters.severityEmergency && alert.severity == "Emergency")
                match = true;
            if(filters.severityNonEmergency && alert.severity == "NonEmergency")
                match = true;
        }
        
        return match;
    }

    function sourceCriteria(alert, filters) {
        if(typeof filters.source === 'undefined' || filters.source == "Any")
            return true;
        else
            return alert.source === filters.source;
    }

    function datesCriteria(alert, filters) {
        if(typeof filters.startDate !== 'undefined' && typeof filters.endDate !== 'undefined') {
            var startDate = new Date(filters.startDate);
            var endDate = new Date(filters.endDate);
            return (alert.created >= startDate && alert.created <= endDate);
        }
        return true;
    }

    return function(alert, filters) {
        return alert.filter(function(element) { 
            return categoryCriteria(element,filters) && severityCriteria(element,filters) && sourceCriteria(element,filters) && datesCriteria(element, filters);
        });
    };
});

app.filter('detectURLs', function($sce) {
    return function(text) {
        var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
        return $sce.trustAsHtml(text.replace(exp,"<a target='_blank' href='$1'>$1</a>")); 
    };
});

app.controller('alertsCtrl', function($scope, $filter, $sce, $timeout, $http) {
        
    //---------------------------------------------------------------------------------------------------
    //-------------------------------------- I N I T  V A R S -------------------------------------------
    //---------------------------------------------------------------------------------------------------

    $scope.categories = ['Any', 'Fire', 'Flood', 'Weather', 'Tsunami', 'Earthquake'];
    $scope.sources = ['Any', 'NOAA', 'USGS', 'Manual entry'];
    $scope.severities = ['Emergency', 'NonEmergency'];
    $scope.alertsLoading = true;
    $scope.missingAffectedArea = false;
    $scope.publishAlertId = null;
    $scope.filters = { category: "Any", source: "Any", severityEmergency: false, severityNonEmergency: false };

    // API Token used as authorization on each API call
    $scope.apiToken = "";
    $scope.setApiToken = function(token) {
        $scope.apiToken = token;
        $http.defaults.headers.common['Authorization'] = 'Bearer ' + $scope.apiToken;
    };

    var baseApiAddress = window.baseApiAddress;

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
                self.affected_area = (typeof self.affected_area == 'object' ? self.affected_area : (self.affected_area != "" ? JSON.parse(self.affected_area) : self.affected_area));
            }
        }

        self.addBroadcastDetails = function (log) {
            self.sent_email = log.sent_email;
            self.sent_sms = log.sent_sms;
            self.sent_total = self.sent_email + self.sent_sms;
            self.published = new Date(log.published);
            self.sent_locations = log.sent_locations;
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
                $scope.alerts.unshift(new Alert(alerts[i], true));
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
            setTimeout(function() { highlightNewAlert(); }, 500);
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

    $scope.editAlert = function(id, duplicate) {
        if(typeof duplicate == 'undefined') duplicate = false;

    	$scope.newAlert = duplicate;
    	$scope.alertId = id;

    	// Retrieve alert by id 
    	var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
        $scope.currAlert = new Alert(alert);

        var location = typeof $scope.currAlert.location == 'object' ? $scope.currAlert.location : JSON.parse($scope.currAlert.location);
        var area = typeof $scope.currAlert.affected_area == 'object' ? $scope.currAlert.affected_area : ($scope.currAlert.affected_area != "" ? JSON.parse($scope.currAlert.affected_area) : null);
        setCoverageMap(location, area || {});

		$(".alert-modal").modal('show');
    }

    $scope.confirmPublishAlert = function(id) {
        // Retrieve alert by id 
        var alert = $filter("filter")($scope.alerts, { id: id }, true)[0];
        // Check if alert already has an affected area defined, if it doesn't open the edit modal
        if(!alert.affected_area || alert.affected_area == "") {
            $scope.missingAffectedArea = true;
            $scope.editAlert(id);
        } else {
            $scope.publishAlertId = id;
            $(".confirmation-modal").modal('show');
        }
    };

    $scope.publishAlert = function() {
        if($scope.publishAlertId) {
            $("#publish_alert").button('loading');
            
            // Retrieve alert by id 
            var alert = $filter("filter")($scope.alerts, { id: $scope.publishAlertId }, true)[0];
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
                    $(".confirmation-modal").modal('hide');
                    $("#publish_alert").button('reset');
                }, 2000);
            }, function errorCallback(response) {
                $("#publish_alert").button('reset');
                $(".modal-error-msg").text(response.data.meta.message);
            });
        }
    }

    $scope.getAlert = function(id) {
        $http({
            method: 'GET',
            url: baseApiAddress + '/v1/notification/' + id,
            headers: { 'Content-Type': 'application/json' }
        }).then(function successCallback(response) {
            // Update current alert based on returned object
            $scope.currAlert = new Alert(response.data.result, true);
            // Get broadcast details of alert
            $scope.getAlertDetails(id);
        }, function errorCallback(response) {
            $scope.alertsLoading = false;
        });
    };

    $scope.getAlertDetails = function(id) {
        $http({
            method: 'GET',
            url: baseApiAddress + '/v1/notification/' + id + '/log',
            headers: { 'Content-Type': 'application/json' }
        }).then(function successCallback(response) {
            // Update alert details based on returned object
            $scope.currAlert.addBroadcastDetails(response.data.result);
            updateMapForCurrentAlert();
            $scope.alertsLoading = false;
        }, function errorCallback(response) {
            $scope.alertsLoading = false;
        });
    };

    $scope.resetAlert = function() {
        $scope.currAlert = new Alert();
    };

    $scope.updateAlertsMap = function() {
        updateMap();
    };

    // Update map when filters change

    $scope.$watch('filters.category', function(newVal, oldVal){ updateMap(); });
    $scope.$watch('filters.source', function(newVal, oldVal){ updateMap(); });
    $scope.$watch('filters.severityEmergency', function(newVal, oldVal){ updateMap(); });
    $scope.$watch('filters.severityNonEmergency', function(newVal, oldVal){ updateMap(); });

    //---------------------------------------------------------------------------------------------------
    //------------------------------ P R I V A T E  M E T H O D S ---------------------------------------
    //---------------------------------------------------------------------------------------------------

    // Map customization

    // Bind infowindows to features
    function onEachFeature(feature, layer) {
        if (feature.properties) {
            if (feature.properties.category)
                layer.bindPopup("<b>" + feature.properties.category + "</b><br/>" +feature.properties.title);
            else
                layer.bindPopup("<b>" + feature.properties.title + "</b>");
        }
    }

    // Style map features, add category icons
    function getCategoryIcons(feature, latlng) {
        if(feature.properties) {
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
                    categoryIcon = "flag"; break;
                case "Earthquake": 
                    categoryIcon = "globe"; break;
                default:
                    categoryIcon = "star";
            }
            var categoryMarker = L.AwesomeMarkers.icon({
                icon: categoryIcon,
                markerColor: (feature.properties.severity == "Emergency" ? 'red' : 'blue')
            });

            return L.marker(latlng, {icon: categoryMarker});
        } else {
            return L.marker(latlng, {icon: L.AwesomeMarkers.icon({ icon: 'home', markerColor: 'blue' })});
        }
    }

    function updateMap() {
        // Clear geoJSON layer
        if(geoJSONLayer)
            alertsMap.removeLayer(geoJSONLayer);

        // Filter alerts first, then create geoJSON
        var mapAlerts = $filter('criteriaMatch')($scope.alerts,$scope.filters);

        // Collect geometries from alerts and create geoJSON
        var geoJSON = { type: "FeatureCollection", features: [] }
        $.each(mapAlerts, function( index, alert ) {
            // Create JSON object in case it was converted to string
            alert.location = (typeof alert.location == 'object' ? alert.location : JSON.parse(alert.location));
            var newFeature = { type: "Feature", geometry: alert.location, properties: { title: alert.title, category: alert.category, severity: alert.severity } }; 
            geoJSON.features.push(newFeature);
        });
        
        if(geoJSON.features.length > 0) {
            // Add geoJSON layer to the map
            geoJSONLayer = L.geoJSON(geoJSON, {
                onEachFeature: onEachFeature,
                pointToLayer: getCategoryIcons
            }).addTo(alertsMap);
            alertsMap.fitBounds(geoJSONLayer.getBounds());
        }
    }

    function updateMapForCurrentAlert() {
        // Clear geoJSON layer
        if(geoJSONLayer)
            alertsMap.removeLayer(geoJSONLayer);

        // Collect geometries from alerts and create geoJSON
        var geoJSON = { type: "FeatureCollection", features: [] }

        // Add alert location
        var locationFeature = { type: "Feature", geometry: $scope.currAlert.location, properties: { title: $scope.currAlert.title, category: $scope.currAlert.category, severity: $scope.currAlert.severity } }; 
        geoJSON.features.push(locationFeature);
        
        // Add alert affected area
        var area = $scope.currAlert.affected_area;
        area.coordinates = [area.coordinates];
        var areaGeoJSON = { type: "Feature", geometry: area, properties: { title: "Affected area" } }; 
        geoJSON.features.push(areaGeoJSON);

        // Add sent locations to map
        $.each($scope.currAlert.sent_locations, function( index, location ) {
            // Create JSON object in case it was converted to string
            location = (typeof location == 'object' ? location : JSON.parse(location));
            var locationGeometry = { type: "Point", coordinates: [location.lng, location.lat] };
            var newFeature = { type: "Feature", geometry: locationGeometry, properties: { title: "User location" } }; 
            geoJSON.features.push(newFeature);
        });
        
        if(geoJSON.features.length > 0) {
            // Add geoJSON layer to the map
            geoJSONLayer = L.geoJSON(geoJSON, {
                onEachFeature: onEachFeature,
                pointToLayer: getCategoryIcons
            }).addTo(alertsMap);
            alertsMap.fitBounds(geoJSONLayer.getBounds());
        }
    }

    function highlightNewAlert() {
        // Scroll to top and hightlight 1st row for a second
        $("body").animate({"scrollTop": "0px"}, 100);
        $("table tbody tr:first-child").addClass("highlight");
        setTimeout(function() {
            $("table tbody tr:first-child").removeClass("highlight");
        }, 1000);
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