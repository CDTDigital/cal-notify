$(document).ready(function () {


    var homeRedirect = "/home2.html";

    function getUrlParameter(name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    };


    var userId =  getUrlParameter('user') ||  localStorage.getItem('user_id');
    var token =  getUrlParameter('token') || localStorage.getItem('auth_token');

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
        $('#emailAlerts').toggleClass('in', userDetails.enabled_email == true)

            $('.js-toggle_email [type="checkbox"]').prop('checked', userDetails.enabled_email);

        $('#email-input').val(userDetails.email);
        $('#phone-input').val(userDetails.phone);
        $('.js-verified_email')
            .toggleClass('fa-check-circle text-success', userDetails.validated_email == true)
            .toggleClass('fa-times-circle text-danger', userDetails.validated_email != true)


        $('.js-verified_sms')
            .toggleClass('fa-check-circle text-success', userDetails.validated_sms == true)
            .toggleClass('fa-times-circle text-danger', userDetails.validated_sms != true)


        console.log(userDetails.enabled_sms);
        $('#smsAlerts').toggleClass('in', userDetails.enabled_sms == true);

        $('.js-toggle_sms [type="checkbox"]').prop('checked', userDetails.enabled_sms);


        console.log(userDetails.address);
        $('#autocomplete').val(userDetails.address.formatted_address);
    }


    $('.js-save').on('click', function(ev){
        var container =  $('.container');

        userDetails.enabled_sms =  $('.js-toggle_sms [type="checkbox"]').prop('checked');
        userDetails.enabled_email = $('.js-toggle_email [type="checkbox"]').prop('checked');

        userDetails.email = $('#email-input').val() != userDetails.email ? $('#email-input').val() : userDetails.email;
        userDetails.sms = $('#phone-input').val() != userDetails.phone ? $('#phone-input').val() : userDetails.phone;
        var update = $.ajax({
            url: baseApiAddress +'/v1/users?auth_token=' + token,
            type: 'PUT',
           data: userDetails,
            success: function (data) {
                userDetails = data.result;
                console.log(userDetails);

                console.log(xhr.responseJSON.meta);
                var alert = '<div class="alert alert-success alert-fixed">' + xhr.responseJSON.meta.message + '</div>'

                setTimeout(function(){
                    alert.fadeOut(200);
                },3000)
            },
            error: function (xhr, status, error) {
                container.find(".alert").remove(); // clear old alerts
                var alert;
                if (xhr.responseJSON !== undefined) {
                    console.log(xhr.responseJSON.meta);
                    var alert = '<div class="alert alert-danger alert-fixed">' + xhr.responseJSON.meta.message + '</div>'
                   alert =  container.prepend(alert);
                } else {
                    var msg = "unknown server error";
                    var alert = '<div class="alert alert-danger alert-fixed">' + msg + '</div>'
                    alert = container.prepend(alert);
                }


            }
        });

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
