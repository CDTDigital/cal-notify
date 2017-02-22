// Uses CommonJS, AMD or browser globals to create a jQuery plugin.
(function(factory) {
	if (typeof define === 'function' && define.amd) {
		// AMD. Register as an anonymous module.
		define(['jquery'], factory);
	} else if (typeof module === 'object' && module.exports) {
		// Node/CommonJS
		module.exports = function(root, jQuery) {
			if (jQuery === undefined) {
				// require('jQuery') returns a factory that requires window to
				// build a jQuery instance, we normalize how we use modules
				// that require this pattern but the window provided is a noop
				// if it's defined (how jquery works)
				if (typeof window !== 'undefined') {
					jQuery = require('jquery');
				} else {
					jQuery = require('jquery')(root);
				}
			}
			factory(jQuery);
			return jQuery;
		};
	} else {
		// Browser globals
		factory(jQuery);
	}
}(function($) {
	// Add css for bootstrap-datetimepicker
	$('head').append($(["<style type='text/css'>", ".bootstrap-datetimepicker-widget {display: block;}", "</style>"].join('\n')));

	$.fn.extend({
		formBuilder: function(dataObj, options) {
			var defaults = {
					class: 'formBuilder',
					reqSymb: '*',
					reqClass: 'required',
					formStyle: 'position: relative;',
					formLegend: '',
					mapType: 'leaflet',
					mapAttribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
					timeout: 500,
					drawToolPosition: 'topright',
					maxZoom: 14,
					strokeColor: '#3388ff',
					fillColor: '#3388ff'
				},
				settings = $.extend({}, defaults, options),
				container = new $('<form />', {
					class: settings.class,
					style: settings.formStyle
				}),
				form = '';

			form = [
				{
					'type': 'legend',
					'html': settings.formLegend
				}
			]

			function beautifyString(str) {
				var cleanedStr = str.substring(1).replace(/([a-z])([A-Z])/g, '$1 $2');
				return str.charAt(0).toUpperCase() + cleanedStr;
			}

			function inArray(n, h) {
				var c = h.length,
					r = false;

				for (var i = 0; i < c; i++) {
					if (h[i] === n) {
						r = true;
					}
				}

				return r;
			}

			/*
			 * Matches US phone number format
			 *
			 * where the area code may not start with 1 and the prefix may not start with 1
			 * allows '-' or ' ' as a separator and allows parens around area code
			 * some people may want to put a '1' in front of their number
			 *
			 * 1(212)-999-2345 or
			 * 212 999 2344 or
			 * 212-999-0983
			 *
			 * but not
			 * 111-123-5434
			 * and not
			 * 212 123 4567
			 */
			$.validator.addMethod('phoneUS', function(phone_number, element) {
				phone_number = phone_number.replace(/\s+/g, "");
				return this.optional(element) || phone_number.length > 9 && phone_number.match(/^(\+?1-?)?(\([2-9]([02-9]\d|1[02-9])\)|[2-9]([02-9]\d|1[02-9]))-?[2-9]([02-9]\d|1[02-9])-?\d{4}$/);
			}, 'Please specify a valid phone number');

			$.validator.addMethod('zipcodeUS', function(value, element) {
				return this.optional(element) || /^\d{5}(-\d{4})?$/.test(value);
			}, 'The specified US ZIP Code is invalid');

			$.validator.addMethod('numericalDigit', function(value, element) {
				return this.optional(element) || value%1 === 0 && /(^-?\d\d*\.\d*$)|(^-?\d\d*$)|(^-?\.\d\d*$)/.test(value);
			}, 'Please enter a valid numerical value');

			// Setup new types for dForm
			$.dform.options.prefix = null;

			$.dform.addType('bool', function(opt) {
				return $('<input type="checkbox" />').dform('attr', opt);
			})

			$.dform.addType('plaintext', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('number', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('decimal', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('url', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('email', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('phone', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('text', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('zip', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('datetime', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('date', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('time', function(opt) {
				return $('<input type="text" />').dform('attr', opt);
			})

			$.dform.addType('longtext', function(opt) {
				return $('<textarea />').dform('attr', opt);
			})

			$.dform.addType('submit', function(opt) {
				return $('<button type="submit" />').dform('attr', opt);
			})

			$.dform.addType('mappoint', function(opt) {
				return $('<input type="hidden" />').dform('attr', opt);
			})

			$.dform.subscribe('[post]', function(opt, type) {
				if (type === 'bool') {
					this.parent().html(this.parent().html() + this.attr('data-caption'))
				}

				if (type === 'datetime') {
					var validDate = moment(this.val(), 'MM/DD/YYYY HH:mm:ss A', true).isValid();
					this.datetimepicker({
						  format: 'MM/DD/YYYY HH:mm:ss A'
						// , debug: true
						// , widgetPositioning: {vertical: 'bottom'}
						, defaultDate: (validDate
							? new Date(this.val())
							: new Date())
					});
				}

				if (type === 'time') {
					var validDate = moment(this.val(), 'HH:mm:ss', true).isValid();
					this.datetimepicker({
						  format: 'LT'
						// , debug: true
						// , widgetPositioning: {vertical: 'bottom'}
						, defaultDate: (validDate
							? new Date(this.val())
							: new Date())
					});
				}

				if (type === 'date') {
					var validDate = moment(this.val(), 'MM/DD/YYYY', true).isValid();
					this.datetimepicker({
						  format: 'MM/DD/YYYY'
						// , debug: true
						// , widgetPositioning: {vertical: 'bottom'}
						, defaultDate: (validDate
							? new Date(this.val())
							: new Date())
					});
				}

				if (type === 'mappoint') {
					if (settings.mapType.toLowerCase() === 'leaflet') {
						var mapElement = '<div id="form_builder_map" class="form-builder-map"></div>'
						  , container = this.parent().find('div#map-container')
						  , input = this.parent().find('input#form_builder_coords')
						  , id = '#' + container.attr('id')
						  , data = $(container).data('value')
						  , drawnItems
						  , drawOptions
						  , drawControl
						  , map;

						container[0].innerHTML = mapElement;

						if (typeof map !== 'undefined') {
							map.removeLayer(drawnItems);
							map.remove();
						} else {
							map = new L.map(container.find('#form_builder_map')[0]).setView([38.663, -98.454], 4);
						}

						L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
		  					attribution: settings.mapAttribution
						}).addTo(map);

						drawnItems = new L.geoJson([], {
							style: function(feature) {
								return {
									color: settings.strokeColor,
									fillColor: settings.fillColor
								}
							}
						});
						drawnItems.addTo(map);

						if (typeof data.coordinates !== 'undefined') {
							drawnItems.addData(data);
						}

						drawOptions = {
							position: settings.drawToolPosition
							, draw: {
								circle: false
								, marker: false
								, polygon: false
								, polyline: false
								, rectangle: false
							}
							, edit: {
								featureGroup: drawnItems
							}
						};

						if (data.type.toLowerCase() === 'point') {
							drawOptions.draw.marker = true;
						} else if (data.type.toLowerCase() === 'linestring' || data.type.toLowerCase() === 'multilinestring') {
							drawOptions.draw.polyline = true;
						} else if (data.type.toLowerCase() === 'polygon' || data.type.toLowerCase() === 'multipolygon') {
							// drawOptions.draw.circle = true;
							drawOptions.draw.polygon = true;
							drawOptions.draw.rectangle = true;
						} else {
							drawOptions.draw.marker = true;
							drawOptions.draw.polyline = true;
							// drawOptions.draw.circle = true;
							drawOptions.draw.polygon = true;
							drawOptions.draw.rectangle = true;
						}

						drawControl = new L.Control.Draw(drawOptions);
						map.addControl(drawControl);

						map.on(L.Draw.Event.CREATED, function(e) {
							if (drawnItems && drawnItems.getLayers().length !== 0) {
								drawnItems.clearLayers();
							}

							input.val(JSON.stringify(e.layer.toGeoJSON().geometry));
							drawnItems.addLayer(e.layer);
						});

						map.on(L.Draw.Event.EDITED, function(e) {
							input.val(JSON.stringify(drawnItems.toGeoJSON().features[0].geometry));
						});

						map.on(L.Draw.Event.DELETED, function(e) {
							drawnItems.clearLayers();
							input.val('');
						});

						$(window).one('form_builder_shown', function (e) {
	                       $("#" + e.divId).trigger('resize');
	                       map.invalidateSize();
	                       if (drawnItems && drawnItems.getLayers().length > 0) {
	                           map.fitBounds([drawnItems.getBounds()], { maxZoom: settings.maxZoom });
	                       } else {
	                           map.setView([38.663, -98.454], 5);
	                       }
					   	});
					} else {
						// Google Maps
						var mapElement = '<div id="form_builder_map" class="form-builder-map"></div>'
						  , center = new google.maps.LatLng(38.663, -98.454)
						  , container = this.parent().find('div#map-container')
						  , input = this.parent().find('input#form_builder_coords')
						  , id = '#' + container.attr('id')
						  , data = $(container).data('value')
						  , bounds = ''
						  , controlUI = document.createElement('div')
						  , controlText = document.createElement('div')
						  , map
						  , timer
						  , counter = 1;

						function makeMap() {
							if (container.find('#form_builder_map')[0]) {
  								container.find('#form_builder_map').remove();
  							}

  							container[0].innerHTML = mapElement;

  							map = new google.maps.Map(container.find('#form_builder_map')[0]);

  							map.setOptions({
								mapMaker: true,
  								zoom: 3,
  								center: center
  						    });

							map.data.setStyle(function(feature) {
								var canEdit = false;
								if (feature.getProperty('isClicked')) {
									canEdit = true;
								}

								return ({
									strokeColor: settings.strokeColor,
									fillColor: settings.fillColor,
									editable: canEdit,
									draggable: canEdit
								})
							});

							// disable editable if you click on the map
							if (data.type.toLowerCase() !== 'point') {
								map.addListener('click', function() {
									map.data.forEach(function(feature) {
										feature.setProperty('isClicked', false);
										controlUI.style.display = 'none';
									});
								});
							}

							var customControlDiv = document.createElement('div');
						    var customControl = new CustomControl(customControlDiv, map);

						    customControlDiv.index = 1;

							// config which tools to enable
							switch(data.type.toLowerCase()) {
								case 'point':
									map.data.setControls(['Point']);
								 	break;

								case 'linestring':
								case 'multilinestring':
									map.data.setControls(['LineString']);
									break;

								case 'polygon':
								case 'multipolygon':
									map.data.setControls(['Polygon']);
									break;

								default:
									map.data.setControls(['Point', 'LineString', 'Polygon']);
									break;
							}

							// Set tool position
							switch (settings.drawToolPosition) {
								case 'bottomcenter':
									map.data.setControlPosition(google.maps.ControlPosition.BOTTOM_CENTER);
									map.controls[google.maps.ControlPosition.BOTTOM_CENTER].push(customControlDiv);
									break;
								case 'bottomleft':
									map.data.setControlPosition(google.maps.ControlPosition.BOTTOM_LEFT);
									map.controls[google.maps.ControlPosition.BOTTOM_LEFT].push(customControlDiv);
									break;
								case 'bottomright':
									map.data.setControlPosition(google.maps.ControlPosition.BOTTOM_RIGHT);
									map.controls[google.maps.ControlPosition.BOTTOM_RIGHT].push(customControlDiv);
									break;
								case 'leftbottom':
									map.data.setControlPosition(google.maps.ControlPosition.LEFT_BOTTOM);
									map.controls[google.maps.ControlPosition.LEFT_BOTTOM].push(customControlDiv);
									break;
								case 'leftcenter':
									map.data.setControlPosition(google.maps.ControlPosition.LEFT_CENTER);
									map.controls[google.maps.ControlPosition.LEFT_CENTER].push(customControlDiv);
									break;
								case 'lefttop':
									map.data.setControlPosition(google.maps.ControlPosition.LEFT_TOP);
									map.controls[google.maps.ControlPosition.LEFT_TOP].push(customControlDiv);
									break;
								case 'rightbottom':
									map.data.setControlPosition(google.maps.ControlPosition.RIGHT_BOTTOM);
									map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(customControlDiv);
									break;
								case 'rightcenter':
									map.data.setControlPosition(google.maps.ControlPosition.RIGHT_CENTER);
									map.controls[google.maps.ControlPosition.RIGHT_CENTER].push(customControlDiv);
									break;
								case 'righttop':
									map.data.setControlPosition(google.maps.ControlPosition.RIGHT_TOP);
									map.controls[google.maps.ControlPosition.RIGHT_TOP].push(customControlDiv);
									break;
								case 'topcenter':
									map.data.setControlPosition(google.maps.ControlPosition.TOP_CENTER);
									map.controls[google.maps.ControlPosition.TOP_CENTER].push(customControlDiv);
									break;
								case 'topleft':
									map.data.setControlPosition(google.maps.ControlPosition.TOP_LEFT);
									map.controls[google.maps.ControlPosition.TOP_LEFT].push(customControlDiv);
									break;
								case 'topright':
									map.data.setControlPosition(google.maps.ControlPosition.TOP_RIGHT);
									map.controls[google.maps.ControlPosition.TOP_RIGHT].push(customControlDiv);
									break;
								default:
									map.data.setControlPosition(google.maps.ControlPosition.TOP_RIGHT);
									map.controls[google.maps.ControlPosition.TOP_RIGHT].push(customControlDiv);
									break;
							}

							bindDataListeners(map.data);

  							if (typeof data.coordinates !== 'undefined') {
  								var geoJson = {
  									"type": "FeatureCollection",
  									"features": [
  										{
  											"type": "Feature",
											"id": 0,
  											"geometry": data
  										}
  									]
  								};
  								map.data.addGeoJson(geoJson);
  							}
  						}

						function processPoints(geometry, callback, thisArg) {
							if (geometry instanceof google.maps.LatLng) {
								callback.call(thisArg, geometry);
							} else if (geometry instanceof google.maps.Data.Point) {
								callback.call(thisArg, geometry.get());
							} else {
								geometry.getArray().forEach(function(g) {
									processPoints(g, callback, thisArg);
								});
							}
						}

						function CustomControl(controlDiv, map) {
						    // Set CSS for the control border
						    controlUI.style.backgroundColor = 'rgb(255, 255, 255)';
						    controlUI.style.borderStyle = 'solid';
						    controlUI.style.borderWidth = '1px';
						    controlUI.style.borderColor = '#ccc';
						    controlUI.style.height = '26px';
							controlUI.style.width = '26px';
						    controlUI.style.marginTop = '4px';
						    controlUI.style.paddingTop = '1px';
						    controlUI.style.cursor = 'pointer';
						    controlUI.style.textAlign = 'center';
						    controlUI.title = 'Delete Feature';
						    controlDiv.appendChild(controlUI);

						    // Set CSS for the control interior
						    controlText.style.fontFamily = 'Arial,sans-serif';
						    controlText.style.fontSize = '16px';
						    controlText.style.paddingLeft = '6px';
						    controlText.style.paddingRight = '4px';
						    controlText.style.marginTop = '1px';
						    controlText.innerHTML = 'X';
						    controlUI.appendChild(controlText);

							controlUI.style.display = 'none';

						    // Setup the click event listeners
						    google.maps.event.addDomListener(controlUI, 'click', removeFeature);
						}

						function removeFeature() {
							map.data.forEach(function(feature) {
								if (feature.getProperty('isClicked')) {
									map.data.remove(feature);
								}
								controlUI.style.display = 'none';
							});
						}

						function addFeatureEventListeners(e) {
							// add click event to set feature editable for anything but points
							if (e.feature.getGeometry().getType() !== 'Point') {
								map.data.addListener('click', function(event) {
									map.data.forEach(function(feature) {
										feature.setProperty('isClicked', false);
									});

									event.feature.setProperty('isClicked', true);
									controlUI.style.display = 'block';
								});
							} else {
								map.data.forEach(function(feature) {
									feature.setProperty('isClicked', true);
									controlUI.style.display = 'none';
								});
							}
						}

						function refreshGeoJSONfromData(e) {
							// console.log('e.feature.getId()', e.feature.getId());
							if (e.feature.getGeometry() && e.feature.getId() === undefined) {
								var existing = false
								  , refresh = false;

								if (e.feature.getGeometry().getType() === 'Point') {
									map.data.forEach(function(feature) {
										if (feature.getId() === e.feature.getId()) {
											existing = true;
											if (e.feature.getGeometry() !== feature.getGeometry()) {
												feature.setGeometry(e.feature.getGeometry());
											}
										} else {
											map.data.remove(feature);
										}
									});
								} else {
									map.data.forEach(function(feature) {
										if (
											e.feature.getId() !== undefined
											&& feature.getId() === e.feature.getId()
										) {
											existing = true;
											if (e.feature.getGeometry() !== feature.getGeometry()) {
												feature.setGeometry(e.feature.getGeometry());
											}
										} else if (feature.getId() === undefined) {
											map.data.remove(feature);
										}
									});
								}

								if (!existing) {
									var item = new google.maps.Data.Feature({
										geometry: e.feature.getGeometry(),
										id: counter
									});

									counter++;

									map.data.add(item);
									refresh = true;
								}

								map.data.setMap(null);
								map.data.setMap(map);
							}

							if (refresh) {
								bounds = new google.maps.LatLngBounds();

								map.data.forEach(function(feature) {
									processPoints(feature.getGeometry(), bounds.extend, bounds);
								});

								map.fitBounds(bounds);
							}

							addFeatureEventListeners(e);
							updateHiddenInputs(e);
						}

						function updateHiddenInputs(e) {
							map.data.toGeoJson(function(geoJ) {
								if (
										e.feature.getGeometry().getType().toLowerCase() === 'polygon'
									 || e.feature.getGeometry().getType().toLowerCase() === 'multipolygon'
								) {
									// console.log('polygon length: ', geoJ.features.length);

									if (geoJ.features.length > 1) {
										// console.log('entered greater than 1');
										var data = [];
										geoJ.features.forEach(function(item) {
											if (item.geometry.type.toLowerCase() === 'polygon') {
												data.push(item.geometry.coordinates);
											} else if (item.geometry.type.toLowerCase() === 'multipolygon') {
												item.geometry.coordinates.forEach(function(range) {
													data.push(range);
												});
											}
										});

										input.val(JSON.stringify({ "type": "MultiPolygon", "coordinates": data}));
									} else if (geoJ.features.length === 1) {
										// console.log('entered equal to 1');
										input.val(JSON.stringify(geoJ.features[0].geometry));
									} else {
										// console.log('entered less than 1');
										input.val('');
									}
								} else if (
										e.feature.getGeometry().getType().toLowerCase() === 'linestring'
									 || e.feature.getGeometry().getType().toLowerCase() === 'multilinestring'
								) {
									// console.log('linestring length: ', geoJ.features.length);

									if (geoJ.features.length > 1) {
										var data = [];
										geoJ.features.forEach(function(item) {
											if (item.geometry.type.toLowerCase() === 'linestring') {
												var line = [];
												item.geometry.coordinates.forEach(function(coord) {
													line.push(coord);
												});
												data.push(line);
											}
											else if (item.geometry.type.toLowerCase() === 'multilinestring') {
												item.geometry.coordinates.forEach(function(range) {
													var line = [];
													range.forEach(function(coord) {
														line.push(coord);
													});
													data.push(line);
												});
											}
										});

										input.val(JSON.stringify({ "type": "MultiLineString", "coordinates": data}));
									} else if (geoJ.features.length === 1) {
										input.val(JSON.stringify(geoJ.features[0].geometry));
									} else {
										input.val('');
									}
								} else {
									if (geoJ.features[0]) {
										if (geoJ.features[0].geometry) {
											input.val(JSON.stringify(geoJ.features[0].geometry));
										}
									} else {
										input.val('');
									}
								}
							});
						}

						function bindDataListeners(dLayer) {
  							dLayer.addListener('addfeature', refreshGeoJSONfromData);
							dLayer.addListener('removefeature', updateHiddenInputs);
							dLayer.addListener('setgeometry', refreshGeoJSONfromData);
						}

						$(window).on('form_builder_shown', function (e) {
							setTimeout(function(){
								google.maps.event.trigger(map, 'resize');
								var feat = false;
								bounds = new google.maps.LatLngBounds();
								map.data.forEach(function(feature) {
									if (feature) {
										feat = true;
										processPoints(feature.getGeometry(), bounds.extend, bounds);
									}
								});
								if (feat) {
									map.fitBounds(bounds);
								} else {
									map.setCenter(center);
								}
							}, settings.timeout);
					 	});

						makeMap();
					}
				}
			});

			$.each(dataObj, function(key, value) {
				var name,
					val,
					type,
					cap,
					req
				$.each(value, function(k, v) {
					if (k.toLowerCase() === 'columnname') {
						name = v;

						if (!cap) {
							cap = beautifyString(v);
						}
					}

					if (k.toLowerCase() === 'columnvalue') {
						val = v;
					}

					if (k.toLowerCase() === 'displayname') {
						if (v != '') {
							cap = v;
						}
					}

					if (k.toLowerCase() === 'columntype') {
						type = v.toLowerCase();
					}

					if (k.toLowerCase() === 'required') {
						req = v;
						if (req) {
							cap += ' ' + settings.reqSymb;
						}
					}
				});

				var inputData = {
					'type': 'div',
					'class': 'form-group',
					'html': [
						{
							'type': type,
							'class': 'form-control',
							'name': name,
							'id': name,
							'placeholder': beautifyString(type),
							'caption': cap,
							'value': val,
							'validate': {}
						}
					]
				};

				if (type === 'bool') {
					inputData = {
						'type': 'div',
						'class': 'form-check checkbox',
						'html': [
							{
								'type': 'label',
								'class': 'form-check-label',
								'for': name,
								'html': [
									{
										'type': type,
										'class': 'form-check-input',
										'data-caption': cap,
										'name': name,
										'id': name,
										'validate': {}
									}
								]
							}
						]
					};

					if (val !== '') {
						inputData.html[0].html[0].checked = val;
					}
				} else if (type === 'number') {
					inputData.html[0].validate.numericalDigit = true;
				} else if (type === 'decimal') {
					inputData.html[0].validate.number = true;
				} else if (type === 'phone') {
					inputData.html[0].validate.phoneUS = true;
				} else if (type === 'email') {
					inputData.html[0].validate.email = true;
				} else if (type === 'url') {
					inputData.html[0].validate.url = true;
				} else if (type === 'string') {} else if (type === 'zip') {
					inputData.html[0].validate.zipcodeUS = true;
				} else if (type === 'datetime') {
					inputData.style = 'position:relative;';
				} else if (type === 'date') {
					inputData.style = 'position:relative;';
				} else if (type === 'time') {
					inputData.style = 'position:relative;';
				} else if (type === 'longtext') {

				} else if (type === 'plaintext') {

				} else if (type === 'submit') {
					inputData = {
						'type': 'div',
						'class': 'form-group',
						'html': [
							{
								'type': 'submit',
								'class': 'btn btn-primary',
								'html': name
							}
						]

					}
				} else if (type === 'mappoint') {
					inputData = {
						'type': 'div',
						'class': 'form-group row mapgroup',
						'html': [
							{
								'type': 'div',
								'id': 'map-container',
								'class': 'map-container',
								'data-value': val
							},
							{
								'type': 'mappoint',
								'id': 'form_builder_coords',
								'value': ''
							}
						]
					}

					if ($.parseJSON(val).coordinates) {
						inputData.html[1].value = JSON.stringify($.parseJSON(val))
					}
				}

				if (req) {
					if (type === 'bool') {
						inputData.html[0].html[0].validate.required = req;
					} else {
						inputData.class += ' ' + settings.reqClass;
						inputData.html[0].validate.required = req;
					}
				}

					var validTypes = [
					'bool',
					'number',
					'decimal',
					'url',
					'email',
					'phone',
					'text',
					'zip',
					'datetime',
					'date',
					'time',
					'longtext',
					'plaintext',
					'submit',
					'mappoint'
				];

				if (inArray(type, validTypes)) {
					form.push(inputData);
				}
			})

			var result = container.dform({
				'html': {
					'type': 'fieldset',
					'html': form
				}
			})

			// setTimeout(function(){
			// 	$(window).trigger('resize');
			// 	console.log('Triggered window resize');
			// }, settings.timeout);

			return result
		}
	})
}));
