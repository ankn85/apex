(function ($) {
    "use strict";

    function initialize() {
        var dtOptions = {
            ajax: {
                url: "/admin/role/search"
            },
            columns: [
                { title: resources.roleName, data: "name", orderable: true, searchable: false },
                { title: resources.normalizedName, data: "normalizedName", orderable: false, searchable: false }
            ],
            order: [[1, "asc"]],
            paging: false
        };

        var crudOptions = {
            create: "/admin/role/create",
            update: "/admin/role/update",
            delete: "/admin/role/delete"
        };

        var jdtWrapper = new DataTablesWrapper(dtOptions, "#jDataTable", crudOptions);
        jdtWrapper.render();
    }

    initialize();
})(jQuery);
