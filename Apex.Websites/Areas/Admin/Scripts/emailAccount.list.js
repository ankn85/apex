(function ($, utils) {
    "use strict";

    function renderEnableSsl(data, type, full, meta) {
        return utils.renderStatusCell(data);
    }

    function renderUseDefaultCredentials(data, type, full, meta) {
        return utils.renderStatusCell(data);
    }

    function renderDefaultEmailAccount(data, type, full, meta) {
        return utils.renderStatusCell(data);
    }

    function detailFunction(data) {
        return [
            '<p><span class="text-bold">',
            resources.host,
            ":</span> ",
            data.host,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.port,
            ":</span> ",
            data.port,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.userName,
            ":</span> ",
            data.userName,
            "</p>"
        ].join("");
    }
    
    function initialize() {
        var dtOptions = {
            ajax: {
                url: "/admin/emailaccount/search"
            },
            columns: [
                { title: resources.email, data: "email", orderable: true, searchable: false },
                { title: resources.displayName, data: "displayName", orderable: false, searchable: false },
                { title: resources.enableSsl, data: "enableSsl", orderable: false, searchable: false, render: renderEnableSsl, className: "text-center" },
                { title: resources.useDefaultCredentials, data: "useDefaultCredentials", orderable: false, searchable: false, render: renderUseDefaultCredentials, className: "text-center" },
                { title: resources.defaultEmailAccount, data: "isDefaultEmailAccount", orderable: false, searchable: false, render: renderDefaultEmailAccount, className: "text-center" }
            ],
            order: [[1, "asc"]],
            paging: false
        };

        var crudOptions = {
            create: "/admin/emailaccount/create",
            read: detailFunction,
            update: "/admin/emailaccount/update",
            delete: "/admin/emailaccount/delete"
        };

        var jdtWrapper = new DataTablesWrapper(dtOptions, "#jDataTable", crudOptions);
        jdtWrapper.render();
    }

    initialize();
})(jQuery, dtUtils);
