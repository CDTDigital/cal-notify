
var coverageMap, drawnItems, scope, alertsMap, geoJSONLayer;

function addCircleToDrawnItems(lat, lng, radius) {
	var coverageCircle = L.circle([lat, lng], radius);

	// Since circle is not part of the GeoJSON standard, create a layer with the circle data
	var geoJSON = coverageCircle.toGeoJSON();
    geoJSON.properties.radius = radius;

	L.geoJson(geoJSON, {
        pointToLayer: function(feature, latlng) {
            return new L.Circle(latlng, feature.properties.radius);
        }
    }).eachLayer(function(layer) {
        layer.addTo(drawnItems);
    });
}

function bindDrawnItemsToInputs() {
	var features = drawnItems.toGeoJSON().features;
	for(var i = 0; i < features.length; i++) {
		var geometry = JSON.stringify(features[i].geometry).replace('[[[','[[').replace(']]]',']]');
		// Since edits can be done randomly, always check for the type of geometry changed and update accordingly
		if (features[i].geometry.type === "Point") {
			$('.coverage-map-coords').val(geometry);
        } else {
        	$('.coverage-map-area-coords').val(geometry);
        }
	}
	updateAlertLocationInputs();
}

function bindDeletedItemsToInputs() {
	// Clear inputs where the geometry has been deleted
	var features = drawnItems.toGeoJSON().features;
	var locationFound = false, affectedAreaFound = false;
	for(var i = 0; i < features.length; i++) {
		var geometry = JSON.stringify(features[i].geometry).replace('[[[','[[').replace(']]]',']]');
		// Since delete can be done randomly, always check for the type of geometry and update accordingly
		if (features[i].geometry.type === "Point") {
			locationFound = true;
        } else {
        	affectedAreaFound = true;
        }
	}
	if(!locationFound)
		$('.coverage-map-coords').val('');
	if(!affectedAreaFound)
		$('.coverage-map-area-coords').val('');
	updateAlertLocationInputs();
}

function updateAlertLocationInputs() {
	scope.$apply(function() { 
        scope.currAlert.location = $(".coverage-map-coords").val();
		scope.currAlert.affected_area = $(".coverage-map-area-coords").val();
    });
}

// ----------- Map builder for alert CRUD operations -----------

function setCoverageMap(location, area, radius) {
	if(typeof radius === 'undefined') radius = 5000;

	var settings = {
		mapAttribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
		drawToolPosition: 'topright',
		maxZoom: 14,
		strokeColor: '#3388ff',
		fillColor: '#3388ff'
	}

	var inputPin = $('.coverage-map-coords'), inputArea = $('.coverage-map-area-coords'), drawOptions, drawControl;

	if (typeof coverageMap !== 'undefined') {
		drawnItems.clearLayers();
		$('.coverage-map-coords').val('');
		$('.coverage-map-area-coords').val('');
	} else {
		// Init map
		coverageMap = new L.map($('.coverage-map')[0]).setView([38.663, -98.454], 4);

		// Set base tiles
		/*L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
				attribution: settings.mapAttribution
		}).addTo(coverageMap);*/
		L.tileLayer('http://{s}.tiles.wmflabs.org/bw-mapnik/{z}/{x}/{y}.png', {
			maxZoom: 18,
			attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
		}).addTo(coverageMap);

		// Add search box
		coverageMap.addControl(new L.Control.Search({
			url: 'http://nominatim.openstreetmap.org/search?format=json&q={s}',
			jsonpParam: 'json_callback',
			propertyName: 'display_name',
			propertyLoc: ['lat', 'lon'],
			circleLocation: false,
			markerLocation: false,
			autoType: false,
			autoCollapse: true,
			autoCollapseTime: 100,
			textPlaceholder: 'Search...',
			animateLocation: false,
			minLength: 2,
			zoom: 10,
			position: 'topright'
		}));

		// Set styling of drawn items
		drawnItems = new L.geoJson([], {
			style: function(feature) {
				return {
					color: settings.strokeColor,
					fillColor: settings.fillColor
				}
			}
		});
		drawnItems.addTo(coverageMap);

		drawOptions = {
			position: settings.drawToolPosition
			, draw: {
				circle: false
				, marker: true
				, polygon: true
				, polyline: false
				, rectangle: false
			}
			, edit: {
				featureGroup: drawnItems
			}
		};

		// Define drawing tooltips
	    L.drawLocal.draw.handlers.marker.tooltip.start = 'Click map to set alert location';
	    L.drawLocal.draw.handlers.polygon.tooltip.start = 'Click to start drawing affected area';
	    L.drawLocal.draw.handlers.polygon.tooltip.cont = 'Click to continue drawing affected area';
	    L.drawLocal.draw.handlers.polygon.tooltip.end = 'Click first point to close the affected area';
	    L.drawLocal.draw.handlers.rectangle.tooltip.start = 'Click and drag to draw affected area';
	    L.drawLocal.draw.handlers.circle.tooltip.start = 'Click and drag to draw affected area';

		drawControl = new L.Control.Draw(drawOptions);
		coverageMap.addControl(drawControl);

		// Draw toolbar handlers
		coverageMap.on(L.Draw.Event.CREATED, function(e) {
			var type = e.layerType;
			if (drawnItems && drawnItems.getLayers().length !== 0) {
				drawnItems.eachLayer(function (layer) {
					// Remove all markers while drawing markers
					if(type === 'marker' && layer instanceof L.Marker) {
						drawnItems.removeLayer(layer);
					// Remove all polygons & rectangles while drawing polygons or rectangles
					} else if(type !== 'marker' && !(layer instanceof L.Marker)) {
						drawnItems.removeLayer(layer);
					}
				});
			}

			var type = e.layerType;
			// Alert angular about changes on input field (Fix when value is changed through jQuery)
			scope.$apply(function() { 
				var geometry = JSON.stringify(e.layer.toGeoJSON().geometry).replace('[[[','[[').replace(']]]',']]');
				if (type === 'marker') {
					inputPin.val(geometry);
		        } else {
		        	inputArea.val(geometry);
		        }
	        });
	        updateAlertLocationInputs();
			
			drawnItems.addLayer(e.layer);
			coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
		});

		coverageMap.on(L.Draw.Event.EDITED, function(e) {
			bindDrawnItemsToInputs();
		});

		coverageMap.on(L.Draw.Event.DELETED, function(e) {
			drawnItems.removeLayer(e.layer);
			bindDeletedItemsToInputs();
		});
	}

	// Add marker with sent coords
	if (typeof location.coordinates !== 'undefined') {
		L.marker([location.coordinates[1], location.coordinates[0]]).addTo(drawnItems);
	}
	if(typeof area.coordinates !== 'undefined') {
		// Reverse coordinates to match Leaflet standard
		$.map(area.coordinates, function reverse(item) { 
			return $.isArray(item) && $.isArray(item[0]) ? $.map(item, reverse) : item.reverse();
        });
		L.polygon(area.coordinates).addTo(drawnItems);
	}

	$(window).one('alert_modal_shown', function (e) {
       	$(".coverage-map").css("opacity","1");
       	$(".alert-modal").trigger('resize');
       	coverageMap.invalidateSize();
       	if (drawnItems && drawnItems.getLayers().length > 0) {
       		coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
       	} else {
       		coverageMap.setView([38.572958, -121.490101], 9);
       	}
       	bindDrawnItemsToInputs();
   	});
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

$(document).ready(function () {

	$('[data-toggle="tooltip"]').tooltip();

	// Reference to AngularJS scope
	scope = angular.element($("body")).scope();

	var alertId = getUrlParameter('alertId');
	// Save token to use on API calls
	scope.$apply(function(){ 
		scope.setApiToken(getUrlParameter('token'));

		// If alertId is present we're in the alert details page
		if(alertId != "") {
			scope.getAlert(alertId);
		} else {
			// Retrieve alerts from API
			scope.getAlerts();
		}
	});

	// Alert modal handlers

	$(".alert-modal").on('hidden.bs.modal', function () {
		// Clear missing area flag so next time the modal is opened the warning msg is gone
		scope.$apply(function(){ 
	    	scope.missingAffectedArea = false;
	    	scope.publishAlertId = null;
	    });
	});

	$(".alert-modal").on('shown.bs.modal', function () {
		// Notify that modal is loaded so the map is resized
		var evt = $.Event('alert_modal_shown');
		$(window).trigger(evt);
	});

	$(".alert-modal").on('show.bs.modal', function () {
		$(".coverage-map").css("opacity","0");
		$(".modal-error-msg").text("");
	});

	$(".confirmation-modal").on('show.bs.modal', function () {
		$(".modal-error-msg").text("");
	});

	$(".save-alert-btn").on('click', function () {
		$(".alert-modal").modal('hide');
	});

	$(".new-alert-btn").on('click', function () {
		setCoverageMap({}, {});
		
	    scope.$apply(function(){ 
	    	scope.resetAlert();
	    	scope.newAlert = true; 
	    });
		$(".alert-modal").modal('show');
	});

	// Init datetime pickers

	// If alertId is present we're in the alert details page, in which case we don't need the datepicker
	if(alertId == "") {
		$('#filter_start_date').datetimepicker({ format: 'MMM D, YYYY, h:mm a' });
		$('#filter_end_date').datetimepicker({ format: 'MMM D, YYYY, h:mm a' });

		// Listener to change scope filters
		$("#filter_start_date").on("dp.change", function() {
	        scope.filters.startDate = $("#filter_start_date").val();
	        scope.$apply();
	    });
	    $("#filter_end_date").on("dp.change", function() {
	        scope.filters.endDate = $("#filter_end_date").val();
	        scope.$apply();
	    });
	}

	// -------- Init Alerts Map (tab) --------

	var largeMapHeight = 650;
  	var smallMapHeight = 300;
  	var mapBreakWidth = 720;
  	var highZoom = 8;
  	var lowZoom = 7;
  	var initZoom;
  	var mapFirstLoad = true;
	
	// Set initial map height, based on the calculated width of the map container
	if ($("#alerts_map").width() > mapBreakWidth) {
		initZoom = highZoom;
		$("#alerts_map").height(largeMapHeight);
	} else {
		initZoom = lowZoom;
		$("#alerts_map").height(smallMapHeight);
	};
	  
    alertsMap = new L.Map('alerts_map', {
     	center: [38.571849, -121.497790],
    	zoom: initZoom
    });

	L.tileLayer('http://{s}.tiles.wmflabs.org/bw-mapnik/{z}/{x}/{y}.png', {
		maxZoom: 18,
		attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
	}).addTo(alertsMap);

	// Add search box
	alertsMap.addControl(new L.Control.Search({
		url: 'http://nominatim.openstreetmap.org/search?format=json&q={s}',
		jsonpParam: 'json_callback',
		propertyName: 'display_name',
		propertyLoc: ['lat', 'lon'],
		circleLocation: false,
		markerLocation: false,
		autoType: false,
		autoCollapse: true,
		autoCollapseTime: 100,
		textPlaceholder: 'Search...',
		animateLocation: false,
		minLength: 2,
		zoom: 10,
		position: 'topright'
	}));

	// If alertId is present we're in the alert details page, in which case we don't add a legend
	if(alertId == "") {
		// Add legend to map
		var legend = L.control({position: 'bottomright'});
		legend.onAdd = function (map) {

		    var div = L.DomUtil.create('div', 'info legend'),
		        severities = [{ color: "#CF3C28", name: "Emergency" }, { color: "#37A7DA", name: "Non-Emergency" }],
		        categories = [{ icon: "fire", name: "Fire" }, { icon: "tint", name: "Flood" }, { icon: "cloud", name: "Weather" }, 
		        			  { icon: "flag", name: "Tsunami" }, { icon: "globe", name: "Earthquake" }, { icon: "star", name: "Any" }];

		    // Generate a label for each Severity & Category type
		    for (var i = 0; i < severities.length; i++) {
		        div.innerHTML += '<div><i style="background:' + severities[i].color + '"></i> ' + severities[i].name + '</div>';
		    }
		    div.innerHTML += '<hr/>';
		    for (var j = 0; j < categories.length; j++) {
		        div.innerHTML += '<div><i class="fa fa-' + categories[j].icon + '" aria-hidden="true"></i> ' + categories[j].name + '</div>';
		    }

		    return div;
		};

		legend.addTo(alertsMap);
	}

    // Use resize event to set new map height and zoom level
	window.onresize = function() {
	    var mapWidth = $("#alerts_map").width();
	    if (mapWidth <= mapBreakWidth) {
			$("#alerts_map").css('height', smallMapHeight);
		} else if (mapWidth > mapBreakWidth) {
			$("#alerts_map").css('height', largeMapHeight);
		}
	};

	// Event triggered after a tab is displayed
	$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
		// Toggle active tab
		$(".preview-toggle a").removeClass("active");
		$(e.target).addClass("active");

		// If map tab is selected & map size isn't set, init & center map to geoJSON layer
  		if($(e.target).attr("href") == "#alerts_map_tab" && mapFirstLoad) {
  			$(window).trigger('resize');
  			alertsMap.invalidateSize();
  			if(geoJSONLayer)
	    		alertsMap.fitBounds(geoJSONLayer.getBounds());
	    	mapFirstLoad = false;
		}
	});

});
