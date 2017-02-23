
var coverageMap, drawnItems;

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
				, marker: (typeof data.coordinates === 'undefined')
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
				//drawnItems.clearLayers();
			}

			var type = e.layerType;
			if (type === 'marker') {
				inputPin.val(JSON.stringify(e.layer.toGeoJSON().geometry));
				//var coords = e.layer.toGeoJSON().geometry.coordinates;
	            //addCircleToDrawnItems(coords[1], coords[0], radius);
	        } else {
	        	inputArea.val(JSON.stringify(e.layer.toGeoJSON().geometry));
	        }
			
			drawnItems.addLayer(e.layer);
			coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
		});

		coverageMap.on(L.Draw.Event.EDITED, function(e) {
			input.val(JSON.stringify(drawnItems.toGeoJSON().features[0].geometry));
		});

		coverageMap.on(L.Draw.Event.DELETED, function(e) {
			drawnItems.clearLayers();
			input.val('');
		});
	}

	// Add circle with center on sent coords
	if (typeof data.coordinates !== 'undefined') {
		L.marker([data.coordinates[1], data.coordinates[0]]).addTo(drawnItems);
		//addCircleToDrawnItems(data.coordinates[1], data.coordinates[0], radius);
	}

	$(window).one('alert_modal_shown', function (e) {
       	$(".coverage-map").css("opacity","1");
       	$(".alert-modal").trigger('resize');
       	coverageMap.invalidateSize();
       	if (drawnItems && drawnItems.getLayers().length > 0) {
       		coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
       	} else {
       		map.setView([38.663, -98.454], 5);
       	}
   	});
}

function publishHandlers() {
	$(".publish-btn").on('click', function() {
    	var currBtn = $(this);
    	currBtn.button('loading');
    	setTimeout(function(){ 
    		currBtn.button('reset'); 
    		currBtn.removeClass("publish-btn").addClass("republish-btn");
    		currBtn.text("Republish"); 
    	}, 5000);
    });

    $(".republish-btn").on('click', function() {
    	setCoverageMap({ "type": "Point", "coordinates": [-121.50772690773012,38.644356698715285]}, 8000);

    	var currBtn = $(this);
    	currBtn.button('loading');

    	$(".alert-modal .modal-title").text("Update Alert");
    	$(".alert-modal .save-alert-btn").text("Save Changes");
    	$(".alert-modal .affected-area-text").text("Use the drawing tools to define the area that should receive the alert:");
		$(".alert-modal").modal('show');	
    });
}

$(document).ready(function () {
	$(".alert-modal").on('shown.bs.modal', function () {
		// Notify that modal is loaded so the map is resized
		var evt = $.Event('alert_modal_shown');
		$(window).trigger(evt);
	});

	$(".alert-modal").on('show.bs.modal', function () {
		$(".coverage-map").css("opacity","0");
	});

	$(".alert-modal").on('hidden.bs.modal', function () {
		$(".republish-btn").button('reset'); 
	});

	$(".save-alert-btn").on('click', function () {
		if($(this).text() == "Save Changes") {
			console.log("Enter save changes");
			// Update republish btn
			$(".republish-btn").button('reset'); 
			$(".republish-btn").removeClass("republish-btn").addClass("publish-btn");
    		$(".republish-btn").text("Publish"); 
		} else {
			// Create alert
			var html = '<tr>' +
                    '<td><span class="glyphicon glyphicon-warning-sign text-danger"></span></td>' +
                    '<td><span class="text-uppercase"><a href="">Major Flooding HWY 99</a></span></td>' +
                    '<td class="text-uppercase">Road closed near Pocket</td>' +
                    '<td><time datetime="2017-02-14">Feb 22, 2017</time></td>' +
                    '<td class="text-uppercase">Flood</td>' +
                    '<td><time datetime="2017-02-14">Feb 22, 2017</time></td>' +
                    '<td><button class="btn btn-success publish-btn" data-loading-text="Notifying users...">Publish</button></td>' +
                '</tr>';
        	$(".alert-table tbody").prepend(html);
		}

		$(".alert-modal").modal('hide');
        
        publishHandlers();
	});

	$(".new-alert-btn").on('click', function () {
		setCoverageMap({});
		$(".alert-modal .modal-title").text("New Alert");
		$(".alert-modal .save-alert-btn").text("Create Alert");
		$(".alert-modal .affected-area-text").text("Drop a pin on the alert's location & use the drawing tools to define the affected area:");
		$(".alert-modal").modal('show');
	});

	publishHandlers();
});
