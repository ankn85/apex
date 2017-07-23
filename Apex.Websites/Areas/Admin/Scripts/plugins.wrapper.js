function DataTablesWrapper(options) {
    this.options = options;

    this.getDefaultOptions = function () {
        return {
            processing: true,
            serverSide: true,
            deferRender: true,
            searching: false,
            //searchDelay: 1000,
            info: true,
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
            ordering: true/*,
            scrollY: "50vh",
            scrollCollapse: true*/
        };
    };

    this.render = function (id, detailFunction, updateFunction, delFunction) {
        this.options.ajax.method = this.options.ajax.method || "POST";

        if (detailFunction) {
            this.options.columns.push({
                title: "",
                data: null,
                orderable: false,
                searchable: false,
                targets: -1,
                render: function (data, type, full, meta) {
                    return '<a href="javascript:;" class="fa fa-commenting-o"></a>';
                },
                className: "detail-control"
            });
        }

        if (updateFunction) {
            var isUrl = typeof updateFunction === "string";

            this.options.columns.push({
                title: "",
                data: null,
                orderable: false,
                searchable: false,
                targets: -2,
                render: function (data, type, full, meta) {
                    var href = isUrl ? updateFunction + "?id=" + data.id : "javascript:;";
                    return '<a href="' + href + '" class="fa fa-pencil-square-o"></a>';
                },
                className: "update-control"
            });
        }

        if (delFunction) {
            this.options.columns.push({
                title: "",
                data: null,
                orderable: false,
                searchable: false,
                targets: -3,
                render: function (data, type, full, meta) {
                    return '<a href="javascript:;" class="fa fa-trash-o"></a>';
                },
                className: "delete-control"
            });
        }

        this.options.columns.unshift({
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

        var mergedOptions = this.getDefaultOptions();
        $.extend(mergedOptions, this.options);

        var table = $(id);
        var dataTable = table.DataTable(mergedOptions);

        var self = this;

        dataTable.on("draw", function () {
            if (detailFunction) {
                table.find("td.detail-control").on("click", function () {
                    var tr = $(this).closest("tr");
                    var row = dataTable.row(tr);

                    if (row.child.isShown()) {
                        row.child.hide();
                        tr.removeClass("shown");
                    }
                    else {
                        row.child(detailFunction).show();
                        tr.addClass("shown");
                    }
                });
            }

            var checkboxAll = table.find("#checkbox-all").prop("checked", false);
            var checkboxItems = table.find('td.checkbox-control input[type="checkbox"]');

            checkboxAll.on("change", function () {
                if (this.checked) {
                    $.each(checkboxItems, function (index, item) {
                        item.checked = true;
                    });
                }
                else {
                    $.each(checkboxItems, function (index, item) {
                        item.checked = false;
                    });
                }
            });

            checkboxItems.on("change", function () {
                if (this.checked) {
                    if (checkboxItems.length === table.find('td.checkbox-control input[type="checkbox"]:checked').length) {
                        checkboxAll.prop("checked", true);
                    }
                }
                else if (checkboxAll.is(":checked")) {
                    checkboxAll.prop("checked", false);
                }
            });

            self.$checkboxItems = checkboxItems;
        });

        this.options = mergedOptions;
        this.$dataTable = dataTable;
    };

    this.refresh = function () {
        this.$dataTable.ajax.reload();
    };

    this.getCheckedItems = function () {
        var checkedItems = [];

        if (this.$checkboxItems) {
            $.each(this.$checkboxItems, function (index, item) {
                if (item.checked) {
                    checkedItems.push(item.value);
                }
            });
        }

        return checkedItems;
    };
}

function DatePickerWrapper(options) {
    this.options = options;

    this.getDefaultOptions = function () {
        return {
            format: "mm/dd/yyyy",
            autoclose: true
            //todayHighlight: true
        };
    };

    this.render = function (fromDateId, toDateId) {
        var mergedOptions = this.getDefaultOptions();
        $.extend(mergedOptions, options);

        var fromDate = $(fromDateId).datepicker(mergedOptions);

        if (toDateId) {
            var toDate = $(toDateId).datepicker(mergedOptions);

            fromDate.on("changeDate", function (e) {
                if (e.date.valueOf() > toDate.datepicker("getDate").valueOf()) {
                    toDate.datepicker("setDate", e.date);
                }
            });

            toDate.on("changeDate", function (e) {
                if (e.date.valueOf() < fromDate.datepicker("getDate").valueOf()) {
                    fromDate.datepicker("setDate", e.date);
                }
            });

            this.$toDate = toDate;
        }

        this.options = mergedOptions;
        this.$fromDate = fromDate;
    };

    this.getFromDate = function () {
        return this.$fromDate.datepicker("getDate");
    };

    this.getToDate = function () {
        return this.$toDate.datepicker("getDate");
    };

    this.getDate = function () {
        return this.getFromDate();
    };
}