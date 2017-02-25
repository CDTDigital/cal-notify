if (window.location.href.indexOf("localhost") > -1) {
    window.baseApiAddress = "http://localhost:3002";
    window.baseAddress = "http://localhost:3000";

} else {
    window.baseApiAddress = "http://api-cal-notify.symsoftsolutions.com";
    window.baseAddress = "http://cal-notify.symsoftsolutions.com";
}
window.baseApiAddress = "http://api-cal-notify.symsoftsolutions.com";