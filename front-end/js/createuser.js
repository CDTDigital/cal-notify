$(document).ready(function () {

    var addressDetails = {};

    $(".form-wrapper-signup .js-next").click(function (e) {
        var button = $(this);
        var currentSection = button.closest(".panel");
        var currentSectionIndex = currentSection.index();

        currentSection.removeClass("is-active").next().addClass("is-active");

        e.preventDefault();
        console.log(button);
        if (button.hasClass('js-send')) {

            addressDetails["email"] = $("#email-input").val();
            addressDetails["phone"] = $("#phone-input").val();
            $.ajax({
                url: 'http://localhost:3002/v1/users/create',
                type: "POST",
                data: JSON.stringify(addressDetails),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function () {
                   console.log(returnedData);
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
            { types: ['geocode'] });
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
        console.log(autocomplete);
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
