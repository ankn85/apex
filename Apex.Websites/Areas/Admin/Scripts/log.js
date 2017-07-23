(function ($) {
    "use strict";

    function detailFunction(data) {
        /*return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
            '<tr>' +
            '<td>Full name:</td>' +
            '<td>' + d.logger + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td>Extension number:</td>' +
            '<td>' + d.exception + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td>Extra info:</td>' +
            '<td>And any further details here (images etc)...</td>' +
            '</tr>' +
            '</table>';*/
        return '<p><span class="text-bold">' + resources.callsite + ':</span> ' + data.callsite + '</p>' +
            '<p class="border-top"><span class="text-bold">' + resources.exception + ':</span> ' + data.exception + '</p>';
    }

    function initialize() {
        var $level = $("#level");

        var drWrapper = new DatePickerWrapper({ startDate: "-3m", endDate: "0d" });
        drWrapper.render("#fromDate", "#toDate");

        var options = {
            ajax: {
                url: "/admin/log/search",
                data: function (data) {
                    data.FromDate = drWrapper.getFromDate();
                    data.ToDate = drWrapper.getToDate();
                    data.Level = $level.val();
                }
            },
            columns: [
                { title: resources.level, data: "level", orderable: true, searchable: false },
                { title: resources.message, data: "message", orderable: false, searchable: false },
                { title: resources.logger, data: "logger", orderable: false, searchable: false },
                { title: resources.logged, data: "logged", orderable: true, searchable: false }
            ],
            order: [[4, "desc"]]
        };

        var dtWrapper = new DataTablesWrapper(options);
        dtWrapper.render("logTable", detailFunction, false, false, true);

        $("#btnSearch").on("click", function () {
            dtWrapper.refresh();
        });

        $("#btnDelete").on("click", function () {
            var checkedItems = dtWrapper.getCheckedItems();

            if (checkedItems.length !== 0) {
                var jqxhr = $.post(
                    "/admin/log/delete",
                    { ids: checkedItems },
                    function (data, status, xhr) {
                        if (xhr.status === 200) {
                            dtWrapper.refresh();
                        }
                    },
                    "json");

                jqxhr.always(function () { });
            }
        });
    }

    initialize();
})(jQuery);
