$(document).ready(function () {

    var addressDetails = {};

    $('[data-toggle="tooltip"]').tooltip();
    function send(e) {
        var button =  $(".js-form-wrapper-signup .js-next");
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

            addressDetails["name"] = $("#name-input").val();
            console.log(addressDetails["formatted_address"] == undefined);



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

        }


        if (currentSectionIndex === 3) {
            $(document).find(".form-wrapper .section").first().addClass("is-active");
            $(document).find(".steps li").first().addClass("is-active");
        }
    }

    $(".js-form-wrapper-signup .js-next").click(send);
    $('.js-form-wrapper-signup').keypress(function (e) {
        if (e.which == 13) {
            send();
            return false;
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

    function getSuggestion(){
        if((addressDetails["formatted_address"] == undefined) && ($('#autocomplete').val() !== '')){
            var service = new google.maps.places.AutocompleteService();
            service.getPlacePredictions({ input: $('#autocomplete').val() }, setSuggestion);

        }
    }

    $('#autocomplete').on('blur', getSuggestion);

    function setSuggestion(predictions){
        console.log(predictions);
        if(predictions.length > 0) {
            var predication = predictions[0];
            var geocoder = new google.maps.Geocoder;
            geocoder.geocode({'placeId': predication.place_id}, function (results, status) {
                if(results.length == 0){
                    return;
                }
                var place = results[0];
                var googlAddressNames = {};
                // Get each component of the address from the place details
                // and fill the corresponding field on the form.
                for (var i = 0; i < place.address_components.length; i++) {
                    var addressType = place.address_components[i].types[0];
                    if (componentForm[addressType]) {
                        var val = place.address_components[i][componentForm[addressType]];
                        googlAddressNames[addressType] = val;
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
            })
        }

    }

    function initAutocomplete() {
        // Create the autocomplete object, restricting the search to geographical
        // location types.
        autocomplete = new google.maps.places.Autocomplete(
            /** @type {!HTMLInputElement} */(document.getElementById('autocomplete')),
            {types: ['geocode']});

        // When the user selects an address from the dropdown, populate the address
        // fields in the form.
        autocomplete.addListener('place_changed', fillInAddress);
    }

    function fillInAddress(passedPlace) {
        // Get the place details from the autocomplete object.
        var place = passedPlace || autocomplete.getPlace();
        console.log(place);
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

    var located = false;
    // Bias the autocomplete object to the user's geographical location,
    // as supplied by the browser's 'navigator.geolocation' object.
    function geolocate() {
        if (navigator.geolocation &&!located) {
            located = true;
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

    window.formatPhone = function formatPhone(obj) {
        var numbers = obj.value.replace(/\D/g, ''),
            char = {0:'(',3:') ',6:' - '};
        obj.value = '';
        for (var i = 0; i < numbers.length; i++) {
            obj.value += (char[i]||'') + numbers[i];
        }
    }
});
