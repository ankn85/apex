(function ($, utils) {
    "use strict";

    function renderEmailConfirmed(data, type, full, meta) {
        return utils.renderStatusCell(data);
    }

    function renderLocked(data, type, full, meta) {
        return utils.renderStatusCell(data);
    }

    function detailFunction(data) {
        return [
            '<p><span class="text-bold">',
            resources.accessFailedCount,
            ":</span> ",
            data.accessFailedCount,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.twoFactorEnabled,
            ":</span> ",
            data.twoFactorEnabled,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.phoneNumberConfirmed,
            ":</span> ",
            data.phoneNumberConfirmed,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.phoneNumber,
            ":</span> ",
            data.phoneNumber,
            "</p>"
        ].join("");
    }

    function initialize() {
        var $email = $("#email");
        var $roles = $("#roles").select2();

        var dtOptions = {
            ajax: {
                url: "/admin/manageaccount/search",
                data: function (data) {
                    data.Email = $email.val();
                    data.RoleIds = $roles.val();
                }
            },
            columns: [
                { title: resources.email, data: "email", orderable: true, searchable: false },
                { title: resources.name, data: "name", orderable: false, searchable: false },
                { title: resources.roles, data: "roles", orderable: false, searchable: false },
                { title: resources.emailConfirmed, data: "emailConfirmed", orderable: false, searchable: false, render: renderEmailConfirmed, className: "text-center" },
                { title: resources.locked, data: "locked", orderable: false, searchable: false, render: renderLocked, className: "text-center" }
            ],
            order: [[1, "asc"]]
        };

        var crudOptions = {
            create: "/admin/manageaccount/create",
            read: detailFunction,
            update: "/admin/manageaccount/update",
            delete: "/admin/manageaccount/delete"
        };

        var jdtWrapper = new DataTablesWrapper(dtOptions, "#jDataTable", crudOptions);
        jdtWrapper.render();

        $("#btnSearch").on("click", function () {
            jdtWrapper.refresh();
        });
    }

    initialize();
})(jQuery, dtUtils);
