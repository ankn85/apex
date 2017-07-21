function DataTablesWrapper() {
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

    this.initialize = function (id, options, supportCheckbox) {
        options.ajax.method = options.ajax.method || "POST";

        var mergedOptions = this.getDefaultOptions();
        $.extend(mergedOptions, options);

        var table = $(id);
        var dataTable = table.DataTable(mergedOptions);

        if (supportCheckbox) {
            var self = this;

            dataTable.on("draw", function () {
                var checkboxAll = table.find("#checkbox-all").prop("checked", false);
                var checkboxItems = table.find('tbody input[type="checkbox"]');

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
                        if (checkboxItems.length === table.find('tbody input[type="checkbox"]:checked').length) {
                            checkboxAll.prop("checked", true);
                        }
                    }
                    else if (checkboxAll.is(":checked")) {
                        checkboxAll.prop("checked", false);
                    }
                });

                self.$checkboxItems = checkboxItems;
            });
        }

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

function DatePickerWrapper() {
    this.getDefaultOptions = function () {
        return {
            format: "mm/dd/yyyy",
            autoclose: true
            //todayHighlight: true
        };
    };

    this.initDatePicker = function (id, options) {
        var mergedOptions = this.getDefaultOptions();
        $.extend(mergedOptions, options);

        var datePicker = $(id).datepicker(mergedOptions);

        this.$datePicker = datePicker;
    };

    this.initDateRange = function (fromDateId, toDateId, options) {
        var mergedOptions = this.getDefaultOptions();
        $.extend(mergedOptions, options);

        var fromDate = $(fromDateId).datepicker(mergedOptions);
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

        this.$fromDate = fromDate;
        this.$toDate = toDate;
    };

    this.getDate = function () {
        return this.$datePicker.datepicker("getDate");
    };

    this.getFromDate = function () {
        return this.$fromDate.datepicker("getDate");
    };

    this.getToDate = function () {
        return this.$toDate.datepicker("getDate");
    };
}