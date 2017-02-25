
var coverageMap, drawnItems, scope;

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

function setCoverageMap(data, radius) {
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
				if (type === 'marker') {
					inputPin.val(JSON.stringify(e.layer.toGeoJSON().geometry));
					scope.alertCoords = JSON.stringify(e.layer.toGeoJSON().geometry);
		        } else {
		        	inputArea.val(JSON.stringify(e.layer.toGeoJSON().geometry));
		        	scope.alertAreaCoords = JSON.stringify(e.layer.toGeoJSON().geometry);
		        }
	        });
			
			drawnItems.addLayer(e.layer);
			coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
		});

		coverageMap.on(L.Draw.Event.EDITED, function(e) {
			var type = e.layerType;

			scope.$apply(function() { 
				var geometry = JSON.stringify(drawnItems.toGeoJSON().features[0].geometry);
				if (type === 'marker') {
					inputPin.val(geometry);
					scope.alertCoords = geometry;
		        } else {
		        	inputArea.val(geometry);
		        	scope.alertAreaCoords = geometry;
		        }
	        });
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
	if (typeof data.coordinates !== 'undefined') {
		L.marker([data.coordinates[1], data.coordinates[0]]).addTo(drawnItems);
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

	//setCoverageMap({ "type": "Point", "coordinates": [-121.50772690773012,38.644356698715285]}, 8000);
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

$(document).ready(function () {
	$(".alert-modal").on('shown.bs.modal', function () {
		// Notify that modal is loaded so the map is resized
		var evt = $.Event('alert_modal_shown');
		$(window).trigger(evt);
	});

	$(".alert-modal").on('show.bs.modal', function () {
		$(".coverage-map").css("opacity","0");
	});

	$(".save-alert-btn").on('click', function () {
		$(".alert-modal").modal('hide');
        
        publishHandlers();
	});

	$(".new-alert-btn").on('click', function () {
		setCoverageMap({});
		
	    scope.$apply(function(){ 
	    	scope.resetAlert();
	    	scope.newAlert = true; 
	    });
		$(".alert-modal").modal('show');
	});

	publishHandlers();

	scope = angular.element($("body")).scope();

	// Save token to use on API calls
	scope.$apply(function(){ 
		scope.setApiToken(getUrlParameter('token'));
	});
});
