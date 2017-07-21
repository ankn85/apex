(function ($) {
    "use strict";
    function renderCheckboxCell(data, type, full, meta) {
        return '<input type="checkbox" value="' + data.id + '">';
    }

    function renderDetailCell(data, type, full, meta) {
        return '<a href="javascript:;" class="fa fa-commenting-o"></a>';
    }

    function initialize() {
        var $level = $("#level");

        var drWrapper = new DatePickerWrapper();
        drWrapper.initDateRange("#fromDate", "#toDate", { startDate: "-3m", endDate: "0d" });

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
                { data: null, orderable: false, searchable: false, targets: 0, render: renderCheckboxCell },
                { data: "level", orderable: true, searchable: false },
                { data: "message", orderable: false, searchable: false },
                { data: "logger", orderable: false, searchable: false },
                { data: "logged", orderable: true, searchable: false },
                { data: null, orderable: false, searchable: false, targets: -1, class: "detail-control", render: renderDetailCell }
            ],
            order: [[4, "desc"]]
        };

        var logWrapper = new DataTablesWrapper();
        logWrapper.initialize("#tableLog", options, true);

        $("#btnSearch").on("click", function () {
            logWrapper.refresh();
        });

        $("#btnDelete").on("click", function () {
            var checkedItems = logWrapper.getCheckedItems();

            if (checkedItems.length !== 0) {
                var jqxhr = $.post(
                    "/admin/log/delete",
                    { ids: checkedItems },
                    function (data, status, xhr) {
                        if (xhr.status === 200) {
                            logWrapper.refresh();
                        }
                    },
                    "json");

                jqxhr.always(function () { });
            }
        });




        // Add event listener for opening and closing details
        $('#tableLog tbody').on('click', 'td.details-control', function () {
            var tr = $(this).closest('tr');
            var row = logWrapper.$dataTable.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child(format(row.data())).show();
                tr.addClass('shown');
            }
        });
    }


    function format(d) {
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
        return '<p><span class="text-bold">Logger:</span> ' + d.logger + '</p>' +
            '<p>Exception: ' + d.exception + '</p>';
    }

    initialize();
})(jQuery);
