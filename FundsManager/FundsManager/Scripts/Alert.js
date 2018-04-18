$(function () {
    $.extend({
        ShowMsg: function (txt) {
            $('.err').text(txt);
            $('.err').slideToggle();
            setTimeout(function () { $('.err').slideToggle(); }, 3000);
        },
        testshow: function () {
            alert('success');
        }
    });
});