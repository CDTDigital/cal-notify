
var coverageMap, drawnItems;

function setCoverageMap(data, radius) {
	if(typeof radius === 'undefined') radius = 5000;

	var settings = {
		mapAttribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
		drawToolPosition: 'topright',
		maxZoom: 14,
		strokeColor: '#3388ff',
		fillColor: '#3388ff'
	}

	var input = $('.coverage-map-coords'), drawOptions, drawControl;

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
				circle: true
				, marker: false
				, polygon: false
				, polyline: false
				, rectangle: true
			}
			, edit: {
				featureGroup: drawnItems
			}
		};

		// Init drawing toolbar
		drawControl = new L.Control.Draw(drawOptions);
		coverageMap.addControl(drawControl);

		// Draw toolbar handlers
		coverageMap.on(L.Draw.Event.CREATED, function(e) {
			if (drawnItems && drawnItems.getLayers().length !== 0) {
				drawnItems.clearLayers();
			}

			input.val(JSON.stringify(e.layer.toGeoJSON().geometry));
			drawnItems.addLayer(e.layer);
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
		var coverageCircle = L.circle([data.coordinates[1],data.coordinates[0]], radius);

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

	$(window).one('alert_modal_shown', function (e) {
       	$(".coverage-map").css("opacity","1");
       	$(".alert-modal").trigger('resize');
       	coverageMap.invalidateSize();
       	console.log(drawnItems);
       	if (drawnItems && drawnItems.getLayers().length > 0) {
       		coverageMap.fitBounds(drawnItems.getBounds(), { maxZoom: settings.maxZoom });
       	} else {
       		map.setView([38.663, -98.454], 5);
       	}
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
		setCoverageMap({ "type": "Point", "coordinates": [-121.50772690773012,38.644356698715285]}, 8000);
	});

	$(".save-alert-btn").on('click', function () {
		$(".alert-modal").modal('hide');
		var html = '<tr>' +
                        '<td><span class="glyphicon glyphicon-warning-sign text-danger"></span></td>' +
                        '<td><span class="text-uppercase"><a href="">Major Flooding HWY 99</a></span></td>' +
                        '<td class="text-uppercase">Road closed near Pocket</td>' +
                        '<td><time datetime="2017-02-14">Feb 22, 2017</time></td>' +
                        '<td class="text-uppercase">Flood</td>' +
                        '<td><button class="btn btn-success btn-test" data-loading-text="Notifying users...">Republish</button></td>' +
                    '</tr>';
        $(".alert-table tbody").append(html);
        $(".btn-test").button('loading');
        setTimeout(function(){ $(".btn-test").button('reset'); }, 5000);
	});
});
