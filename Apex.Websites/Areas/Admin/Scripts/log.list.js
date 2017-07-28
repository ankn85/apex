(function ($) {
    "use strict";

    function detailFunction(data) {
        return [
            '<p><span class="text-bold">',
            resources.callsite,
            ":</span> ",
            data.callsite,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.exception,
            ":</span> ",
            data.exception,
            "</p>"
        ].join("");
    }

    function initialize() {
        var $level = $("#level");

        var drOptions = { startDate: "-3m", endDate: "0d" };
        var drWrapper = new DatePickerWrapper(drOptions, "#fromDate", "#toDate");
        drWrapper.render();

        $level.select2({
            minimumResultsForSearch: Infinity
        });

        var dtOptions = {
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
                { title: resources.logged, data: "logged", orderable: true, searchable: false, className: "pretty-date" }
            ],
            order: [[4, "desc"]]
        };

        var crudOptions = {
            read: detailFunction,
            delete: "/admin/log/delete"
        };

        var jdtWrapper = new DataTablesWrapper(dtOptions, "#jDataTable", crudOptions);
        jdtWrapper.render();

        $("#btnSearch").on("click", function () {
            jdtWrapper.refresh();
        });
    }

    initialize();
})(jQuery);
