(function ($) {
    "use strict";
    
    function renderActionCell(data, type, full, meta) {
        return '<button type="button" class="btn btn-xs btn-flat btn-info">View</button>' +
            '<a href="/admin/emailaccount/update?id=1" class="btn btn-xs btn-flat btn-primary">Update</a>' +
            '<button type="button" class="btn btn-xs btn-flat btn-danger">Delete</button>';
        //return [
        //    '<a class="fa fa-pencil-square-o" title="Update" href="/admin/emailaccount/update?id=',
        //    data.id,
        //    '"></a>&nbsp;',
        //    '&nbsp;<a class="fa fa-trash-o" title="Delete" href="javascript:;" data-id="',
        //    data.id,
        //    '" data-username="',
        //    data.userName,
        //    '" data-email="',
        //    data.email,
        //    '"></a>'
        //].join("");
    }

    function initialize() {
        var options = {
            ajax: {
                url: "/admin/emailaccount/search"
            },
            columns: [
                { data: "email", orderable: true, searchable: false },
                { data: "displayName", orderable: false, searchable: false },
                { data: "host", orderable: false, searchable: false },
                { data: "port", orderable: false, searchable: false },
                { data: "enableSsl", orderable: false, searchable: false },
                //{ data: "useDefaultCredentials", orderable: false, searchable: false },
                { data: "isDefaultEmailAccount", orderable: false, searchable: false },
                { data: null, orderable: false, searchable: false, targets: 6, render: renderActionCell },
            ],
            order: [[0, "asc"]],
            paging: false
        };

        var logWrapper = new DataTablesWrapper();
        logWrapper.initialize("#tableEmailAccount", options);
    }

    initialize();
})(jQuery);
