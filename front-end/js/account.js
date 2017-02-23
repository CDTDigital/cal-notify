$(document).ready(function () {

    var baseApiAddress = "http://localhost:3002";
    var baseAddress = "http://localhost:3000";
    var homeRedirect = "/home2.html";

    function getUrlParameter(name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    };


    var userId = localStorage.getItem('user_id')  || getUrlParameter('user');
    var token = localStorage.getItem('auth_token') || getUrlParameter('token');

    if(userId != '' && userId != null) {
        localStorage.setItem('user_id', userId);
    } else {
        window.location.href = baseAddress + homeRedirect;
    }

    if(token != '' && token != null) {
        localStorage.setItem('auth_token', token);
    } else {
        window.location.href = baseAddress + homeRedirect;
    }


    var userDetails = {};

    var req = $.get(baseApiAddress + "/v1/users/" + userId,{auth_token: token});
    req.done(function(data){
        userDetails = data.result;
        update();
    })


    function update(){
        console.log(userDetails);
        $('.js-toggle_email').collapse({
            toggle: userDetails.enabled_email
        });

        $('#email-input').val(userDetails.email);

        $('.js-toggle_sms').collapse({
            toggle: userDetails.enabled_sms
        });

        console.log(userDetails.address);
        $('#autocomplete').val(userDetails.address.formatted_address);
    }


    $('.js-save').on('click', function(ev){

        var update = $.post(baseApiAddress/ 'v1/users/' + userId + "?auth_token=" + token, userDetails);


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

        var addressDetails = {};

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

        userDetails.address = addressDetails;
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
    console.log(window.geolocate);
});
