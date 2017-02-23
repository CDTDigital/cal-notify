$(document).ready(function () {

    var baseApiAddress = "http://localhost:3002";
    var baseAddress = "http://localhost:3000";
    var addressDetails = {};


    $(".js-form-wrapper-signup .js-next").click(function (e) {
        var button = $(this);
        var form = $('.js-form-wrapper-signup .modal-body');

        var currentSection = button.closest(".panel");
        var currentSectionIndex = currentSection.index();

        currentSection.removeClass("is-active").next().addClass("is-active");

        e.preventDefault();
        console.log(button);
        if (button.hasClass('js-send')) {

            addressDetails["email"] = $("#email-input").val();
            addressDetails["phone"] = $("#phone-input").val();
            addressDetails["password"] = $("#password-input").val();

            addressDetails = {
                "name": "matthew",
                "email": "matt.d.clemens@gmail.com",
                "phone": "9167056448",
                "lat": 38,
                "lng": 38,
                "number": "5344",
                "street": "new britton",
                "state": "Ca",
                "zip": "95843",
                "city": "Antelope",
                "password": 'test123'
            }

            $.ajax({
                url: baseApiAddress + '/v1/users/create',
                type: "POST",
                data: JSON.stringify(addressDetails),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                    form.find('.alert').remove();
                    var msg = "Your account has been created. Please check your inbox or phone for a verification code to complete the process.";
                    var success = '<div class="alert alert-success"><strong>Success! </strong>' + msg + '</div>'
                    form.prepend(success);
                    button.attr("disabled", true);
                },
                error: function (xhr, status, error) {
                    form.find('.alert').remove(); // clear old alerts
                    if (xhr.responseJSON !== undefined) {
                        console.log(xhr.responseJSON);
                        var details = xhr.responseJSON.meta.details != null ? xhr.responseJSON.meta.details : [xhr.responseJSON.meta.message];
                        details.map(function (msg) {
                            var alert = '<div class="alert alert-danger">' + msg + '</div>'
                            form.prepend(alert);
                        })
                    } else {
                        var msg = "unknown server error";
                        var alert = '<div class="alert alert-danger">' + msg + '</div>'
                        form.prepend(alert);
                    }


                }
            });
            ;
        }


        if (currentSectionIndex === 3) {
            $(document).find(".form-wrapper .section").first().addClass("is-active");
            $(document).find(".steps li").first().addClass("is-active");
        }
    });

    var placeSearch, autocomplete;
    var componentForm = {
        street_number: 'short_name',
        route: 'long_name',
        locality: 'long_name',
        administrative_area_level_1: 'short_name',
        country: 'long_name',
        postal_code: 'short_name'
    };

    function initAutocomplete() {
        // Create the autocomplete object, restricting the search to geographical
        // location types.
        autocomplete = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */(document.getElementById('autocomplete')),
            {types: ['geocode']});
        console.log(autocomplete);
        // When the user selects an address from the dropdown, populate the address
        // fields in the form.
        autocomplete.addListener('place_changed', fillInAddress);
    }

    function fillInAddress() {
        // Get the place details from the autocomplete object.
        var place = autocomplete.getPlace();

        var googlAddressNames = {};
        // Get each component of the address from the place details
        // and fill the corresponding field on the form.
        for (var i = 0; i < place.address_components.length; i++) {
            var addressType = place.address_components[i].types[0];
            if (componentForm[addressType]) {
                var val = place.address_components[i][componentForm[addressType]];
                console.log(addressType, val);
                googlAddressNames[addressType] = val;
                // document.getElementById(addressType).value = val;
            }
        }


        addressDetails['lat'] = place.geometry.location.lat();
        addressDetails['lng'] = place.geometry.location.lng();
        addressDetails['formatted_address'] = place.formatted_address;
        addressDetails['number'] = googlAddressNames["street_number"];
        addressDetails["street"] = googlAddressNames["route"];
        addressDetails["state"] = googlAddressNames["administrative_area_level_1"]
        addressDetails["zip"] = googlAddressNames["postal_code"];
        addressDetails["city"] = googlAddressNames["locality"];
        console.log(addressDetails);
    }

    window.initAutocomplete = initAutocomplete;
    initAutocomplete();
    // Bias the autocomplete object to the user's geographical location,
    // as supplied by the browser's 'navigator.geolocation' object.
    function geolocate() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var geolocation = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                var circle = new google.maps.Circle({
                    center: geolocation,
                    radius: position.coords.accuracy
                });
                autocomplete.setBounds(circle.getBounds());
            });
        }
    }

    window.geolocate = geolocate;

    $(".form-wrapper").submit(function (e) {
        e.preventDefault();
    });

});
