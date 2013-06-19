$(document).ready(function() {
    var d = new Date();

    var currDate = d.getDate();
    var currMonth = d.getMonth();
    var currYear = d.getFullYear();

    var dateStr = (currMonth - 1) + "-" + currDate + "-" + currYear;
    var dateEnd = currMonth + "-" + currDate + "-" + currYear;

    $('#startDate').datepicker(({ dateFormat: "mm-dd-yy", autoclose: true, defaultDate: dateStr })).on('show', function() {
        var dp = $(this);
        if (dp.val() == '') {
            dp.val(dateStr).datepicker('update');
        }
    });
    $("#endDate").datepicker(({ dateFormat: "mm-dd-yy", autoclose: true, defaultDate: dateEnd })).on('show', function() {
        var dp = $(this);
        if (dp.val() == '') {
            dp.val(dateEnd).datepicker('update');
        }
    });
});