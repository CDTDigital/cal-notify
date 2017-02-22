var coverageMap, coverageCircle;

function setCoverageMap(data, radius) {
	if(typeof radius === 'undefined') radius = 5000;

	var settings = {
		mapAttribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
		drawToolPosition: 'topright',
		maxZoom: 14,
		strokeColor: '#3388ff',
		fillColor: '#3388ff'
	}

	var input = $('.coverage-map-coords'), drawnItems, drawOptions, drawControl;

	if (typeof coverageMap !== 'undefined') {
		coverageMap.remove();
	}

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

	if (typeof data.coordinates !== 'undefined') {
		coverageCircle = L.circle([data.coordinates[1],data.coordinates[0]], radius);
		coverageCircle.addTo(coverageMap);
		//drawnItems.addData(data);
	}

	drawOptions = {
		position: settings.drawToolPosition
		, draw: {
			circle: true
			, marker: false
			, polygon: true
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

	coverageMap.on(L.Draw.Event.CREATED, function(e) {
		if (drawnItems && drawnItems.getLayers().length !== 0) {
			drawnItems.clearLayers();
		}
		if(coverageCircle) {
			coverageMap.removeLayer(coverageCircle);
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

	$(window).one('alert_modal_shown', function (e) {
       	$(".coverage-map").css("opacity","1");
       	$(".alert-modal").trigger('resize');
       	coverageMap.invalidateSize();
       	if (drawnItems && drawnItems.getLayers().length > 0) {
       		coverageMap.fitBounds([drawnItems.getBounds()], { maxZoom: settings.maxZoom });
       	} else {
       		coverageMap.fitBounds(coverageCircle.getBounds(), { maxZoom: settings.maxZoom });
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
});
