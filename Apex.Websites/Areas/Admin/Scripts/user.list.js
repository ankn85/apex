(function ($, utils) {
    "use strict";

    function renderLocked(data, type, full, meta) {
        if (data) {
            return '<span class="label label-danger">Locked</span>';
        }

        return '<span class="label label-success">Active</span>';
    }

    function detailFunction(data) {
        return [
            '<p><span class="text-bold">',
            resources.gender,
            ":</span> ",
            data.gender,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.birthday,
            ":</span> ",
            data.birthday,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.address,
            ":</span> ",
            data.address,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.emailConfirmed,
            ":</span> ",
            data.emailConfirmed,
            "</p>",
            '<p class="border-top"><span class="text-bold">',
            resources.accessFailedCount,
            ":</span> ",
            data.accessFailedCount,
            "</p>"
        ].join("");
    }

    function initialize() {
        var $email = $("#email");
        var $roles = $("#roles").select2();

        var dtOptions = {
            ajax: {
                url: "/admin/user/search",
                data: function (data) {
                    data.Email = $email.val();
                    data.RoleIds = $roles.val();
                }
            },
            columns: [
                { title: resources.email, data: "email", orderable: true, searchable: false },
                { title: resources.fullName, data: "fullName", orderable: false, searchable: false },
                { title: resources.roles, data: "roles", orderable: false, searchable: false },
                { title: resources.phoneNumber, data: "phoneNumber", orderable: false, searchable: false, defaultContent: "" },
                { title: resources.status, data: "locked", orderable: false, searchable: false, render: renderLocked, className: "text-center" }
            ],
            order: [[1, "asc"]]
        };

        var crudOptions = {
            create: "/admin/user/create",
            read: detailFunction,
            update: "/admin/user/update",
            delete: "/admin/user/delete"
        };

        var jdtWrapper = new DataTablesWrapper(dtOptions, "#jDataTable", crudOptions);
        jdtWrapper.render();

        $("#btnSearch").on("click", function () {
            jdtWrapper.refresh();
        });
    }

    initialize();
})(jQuery, dtUtils);
