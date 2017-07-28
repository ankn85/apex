"use strict";

function DataTablesWrapper(options, tableId, crudOptions) {
    var self = this;
    var defaultOpts = {
        processing: true,
        serverSide: true,
        deferRender: true,
        searching: false,
        //searchDelay: 1000,
        rowId: "id",
        autoWidth: false,
        paging: true,
        pagingType: "full_numbers",
        language: {
            paginate: {
                first: "«",
                previous: "‹",
                next: "›",
                last: "»"
            },
            processing: '<div class="facebook-loading"></div>'
        },
        lengthMenu: [10, 25, 50, 100],
        pageLength: 25,
        ordering: true,
        dom: '<"row"<"col-sm-6"l><"#jDataTable_filter.col-sm-6"f>><"row"<"col-sm-12"tr>><"row"<"col-sm-5"i><"col-sm-7"p>>'
        /*,scrollY: "50vh",
        scrollCollapse: true*/
    };
    var showCreate = false;
    var showRead = false;
    var showUpdate = false;
    var showDelete = false;
    var $dataTable = null;
    var $deleteButton = null;
    var $checkboxAll = null;
    var $checkboxItems = null;

    var setCRUDPermission = function () {
        if (crudOptions) {
            showCreate = crudOptions.create && crudOptions.create.length !== 0;
            showRead = crudOptions.read && typeof crudOptions.read === "function";
            showUpdate = crudOptions.update && crudOptions.update.length !== 0;
            showDelete = crudOptions.delete && crudOptions.delete.length !== 0;
        }
    };

    var mergeDefaultOptions = function () {
        $.extend(defaultOpts, options);

        defaultOpts.ajax.method = defaultOpts.ajax.method || "POST";

        if (showRead) {
            defaultOpts.columns.push({
                title: "",
                data: null,
                orderable: false,
                searchable: false,
                targets: -1,
                render: function (data, type, full, meta) {
                    return '<i class="fa fa-plus" title="Detail"></i>';
                },
                className: "read-control"
            });
        }

        if (showUpdate) {
            var updateUrl = crudOptions.update + "?id=";

            defaultOpts.columns.push({
                title: "",
                data: null,
                orderable: false,
                searchable: false,
                targets: -2,
                render: function (data, type, full, meta) {
                    return '<a href="' + updateUrl + data.id + '" class="fa fa-pencil" title="Update"></a>';
                },
                className: "update-control"
            });
        }

        if (showDelete) {
            defaultOpts.columns.unshift({
                title: '<input type="checkbox" id="checkbox-all" />',
                data: null,
                orderable: false,
                searchable: false,
                targets: 0,
                render: function (data, type, full, meta) {
                    return '<input type="checkbox" value="' + data.id + '">';
                },
                className: "checkbox-control"
            });
        }

        return defaultOpts;
    };

    var renderButtons = function () {
        var buttonsHtml = "";

        if (showCreate) {
            buttonsHtml += '<a href="' + crudOptions.create + '" class="btn btn-sm btn-flat btn-success"><i class="fa fa-plus"></i> Create New</a>&nbsp';
        }

        if (showDelete) {
            buttonsHtml += '<button type="button" class="btn btn-sm btn-flat btn-danger" id="btnDelete"><i class="fa fa-trash-o"></i> Delete (Selected)</button>';
        }

        if (buttonsHtml.length !== 0) {
            buttonsHtml = '<div class="dataTables_filter">' + buttonsHtml + "</div>";

            return $("#jDataTable_filter").append(buttonsHtml);
        }
    };

    var readHandler = function () {
        var $this = $(this);
        var tr = $this.closest("tr");
        var icon = $this.children().first();
        var row = $dataTable.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass("shown");
            icon.removeClass("fa fa-minus").addClass("fa fa-plus");
        }
        else {
            row.child(crudOptions.read(row.data())).show();
            tr.addClass("shown");
            icon.removeClass("fa fa-plus").addClass("fa fa-minus");
        }
    };

    var ajaxDeleteHandler = function (checkedValues) {
        $.post(
            crudOptions.delete,
            { ids: checkedValues },
            function (data, status, xhr) {
                if (xhr.status === 200) {
                    self.refresh();
                }
            },
            "json");
    };

    var deleteHandler = function () {
        if (!$checkboxItems) {
            return;
        }

        var checkedValues = $checkboxItems.map(function () {
            if (this.checked) {
                return this.value;
            }
        }).get();

        var checkedCount = checkedValues.length;

        if (checkedCount === 0) {
            return;
        }

        bootbox.confirm({
            title: "Delete Confirm?",
            message: "Are you sure you want to delete " + checkedCount + " selected item(s)?",
            buttons: {
                cancel: {
                    className: "btn-flat btn-default pull-left",
                    label: '<i class="fa fa-times"></i> Close'
                },
                confirm: {
                    className: "btn-flat btn-danger",
                    label: '<i class="fa fa-trash-o"></i> Delete'
                }
            },
            callback: function (result) {
                if (result) {
                    ajaxDeleteHandler(checkedValues);
                }
            }
        });
    };

    var checkboxAllChangedHandler = function () {
        if (this.checked) {
            $.each($checkboxItems, function (index, item) {
                item.checked = true;
            });

            $deleteButton.removeClass("disabled").prop("disabled", false);
        }
        else {
            $.each($checkboxItems, function (index, item) {
                item.checked = false;
            });

            $deleteButton.addClass("disabled").prop("disabled", true);
        }
    };

    var checkboxChangedHandler = function () {
        var checkboxCount = $checkboxItems.length;

        if (this.checked) {
            $deleteButton.removeClass("disabled").prop("disabled", false);

            for (var i = 0; i < checkboxCount; i++) {
                if (!$checkboxItems[i].checked) {
                    $checkboxAll.prop("indeterminate", true);
                    return;
                }
            }

            $checkboxAll
                .prop("indeterminate", false)
                .prop("checked", true);
        }
        else {
            for (var j = 0; j < checkboxCount; j++) {
                if ($checkboxItems[j].checked) {
                    $checkboxAll.prop("indeterminate", true);
                    return;
                }
            }

            $deleteButton.addClass("disabled").prop("disabled", true);

            $checkboxAll
                .prop("indeterminate", false)
                .prop("checked", false);
        }
    };

    self.render = function () {
        setCRUDPermission();
        var table = $(tableId);

        mergeDefaultOptions();
        $dataTable = table.DataTable(defaultOpts);

        var filterContainer = renderButtons();
        var tbody = table.find("tbody");

        if (showRead) {
            tbody.on("click", ".read-control", readHandler);
        }

        if (showDelete && filterContainer) {
            $deleteButton = filterContainer
                .find("#btnDelete")
                .off()
                .on("click", deleteHandler);

            $checkboxAll = table
                .find("#checkbox-all")
                .off()
                .on("change", checkboxAllChangedHandler);

            tbody.on("change", '.checkbox-control input[type="checkbox"]', checkboxChangedHandler);

            $dataTable.on("draw", function (e, settings) {
                $checkboxItems = tbody.find('.checkbox-control input[type="checkbox"]');

                $deleteButton.addClass("disabled").prop("disabled", true);
                $checkboxAll
                    .prop("indeterminate", false)
                    .prop("checked", false)
                    .prop("disabled", $dataTable.data().length === 0);
            });
        }
    };

    self.refresh = function () {
        $dataTable.ajax.reload();
    };
}
