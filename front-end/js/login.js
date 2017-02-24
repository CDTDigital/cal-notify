$(document).ready(function () {


    $(".js-form-wrapper-login").submit(function (e) {
        e.preventDefault();
    });
    $(".js-form-wrapper-login .js-login").click(function (e) {
        e.preventDefault();

        var form = $('.js-form-wrapper-login .js-form-body');

        var data=  {
            contact_info: form.find('[name="contact_info"]').val(),
            password: form.find('[name="password"]').val()
        };
        console.log(data);

        $.ajax({
            url: baseApiAddress + '/v1/users/login',
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data);
                window.location.href = data.result

            },
            error: function (xhr, status, error) {
                form.find('.alert').remove(); // clear old alerts
                if (xhr.responseJSON !== undefined) {
                    console.log(xhr.responseJSON.meta);
                    var alert = '<div class="alert alert-danger">' + xhr.responseJSON.meta.message + '</div>'
                    form.prepend(alert);
                } else {
                    var msg = "unknown server error";
                    var alert = '<div class="alert alert-danger">' + msg + '</div>'
                    form.prepend(alert);
                }


            }
        });
    });

});
