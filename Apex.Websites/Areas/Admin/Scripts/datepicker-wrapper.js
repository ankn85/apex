function DatePickerWrapper(options, fromDateId, toDateId) {
    var self = this;
    var defaultOpts = {
        format: "mm/dd/yyyy",
        autoclose: true
        //todayHighlight: true
    };

    self.render = function () {
        var mergedOpts = defaultOpts;
        $.extend(mergedOpts, options);

        self.$fromDate = $(fromDateId).datepicker(mergedOpts);

        if (toDateId) {
            self.$toDate = $(toDateId).datepicker(mergedOpts);

            self.$fromDate.on("changeDate", function (e) {
                if (e.date.valueOf() > self.getToDate().valueOf()) {
                    self.$toDate.datepicker("setDate", e.date);
                }
            });

            self.$toDate.on("changeDate", function (e) {
                if (e.date.valueOf() < self.getFromDate().valueOf()) {
                    self.$fromDate.datepicker("setDate", e.date);
                }
            });
        }
    };

    self.getFromDate = function () {
        return self.$fromDate.datepicker("getDate");
    };

    self.getToDate = function () {
        return self.$toDate.datepicker("getDate");
    };

    self.getDate = function () {
        return self.getFromDate();
    };
}
