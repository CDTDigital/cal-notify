
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
	} else {
		// Init map
		coverageMap = new L.map($('.coverage-map')[0]).setView([38.663, -98.454], 4);

		// Set base tiles
		L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
				attribution: settings.mapAttribution
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
				, rectangle: true
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
			
			drawnItems.addLayer(e.layer);
			coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
		});

		coverageMap.on(L.Draw.Event.EDITED, function(e) {
			bindDrawnItemsToInputs();
		});

		coverageMap.on(L.Draw.Event.DELETED, function(e) {
			var type = e.layerType;
			if (type === 'marker') {
				inputPin.val('');
	        } else {
	        	inputArea.val('');
	        }
			drawnItems.removeLayer(e.layer);
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
	bindDrawnItemsToInputs();

	$(window).one('alert_modal_shown', function (e) {
       	$(".coverage-map").css("opacity","1");
       	$(".alert-modal").trigger('resize');
       	coverageMap.invalidateSize();
       	if (drawnItems && drawnItems.getLayers().length > 0) {
       		coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
       	} else {
       		coverageMap.setView([38.572958, -121.490101], 9);
       	}
   	});
}

function publishHandlers() {
	$(".publish-btn").on('click', function() {
    	var currBtn = $(this);
    	currBtn.button('loading');
    	setTimeout(function(){ 
    		currBtn.button('reset'); 
    		//currBtn.removeClass("publish-btn").addClass("republish-btn");
    		//currBtn.text("Republish"); 
    	}, 3000);
    });
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

$(document).ready(function () {
	// Reference to AngularJS scope
	scope = angular.element($("body")).scope();

	// Save token to use on API calls
	scope.$apply(function(){ 
		scope.setApiToken(getUrlParameter('token'));
		// Retrieve alerts from API
		scope.getAlerts();
	});

	// Alert modal handlers

	$(".alert-modal").on('shown.bs.modal', function () {
		// Notify that modal is loaded so the map is resized
		var evt = $.Event('alert_modal_shown');
		$(window).trigger(evt);
	});

	$(".alert-modal").on('show.bs.modal', function () {
		$(".coverage-map").css("opacity","0");
		$(".modal-error-msg").text("");
	});

	$(".save-alert-btn").on('click', function () {
		$(".alert-modal").modal('hide');
        
        publishHandlers();
	});

	$(".new-alert-btn").on('click', function () {
		setCoverageMap({}, {});
		
	    scope.$apply(function(){ 
	    	scope.resetAlert();
	    	scope.newAlert = true; 
	    });
		$(".alert-modal").modal('show');
	});

	publishHandlers();

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

    // Use Leaflets resize event to set new map height and zoom level
	alertsMap.on('resize', function(e) {
	  	if (e.newSize.x <= mapBreakWidth) {
			$("#alerts_map").css('height', smallMapHeight);
		} else if (e.newSize.x > mapBreakWidth) {
			$("#alerts_map").css('height', largeMapHeight);
		}
	});

	// Event triggered after a tab is displayed
	$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
		// Toggle active tab
		$(".preview-toggle a").removeClass("active");
		$(e.target).addClass("active");

		// If map tab is selected & map size isn't set, init & center map to geoJSON layer
  		if($(e.target).attr("href") == "#alerts_map_tab" && mapFirstLoad) {
  			alertsMap.invalidateSize();
	    	alertsMap.fitBounds(geoJSONLayer.getBounds());
	    	mapFirstLoad = false;
		}
	});

});
