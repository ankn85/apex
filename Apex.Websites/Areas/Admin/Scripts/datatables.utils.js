"use strict";

function DataTablesUtils() {
}

DataTablesUtils.prototype.renderStatusCell = function (status) {
    if (status) {
        return '<i class="fa fa-check text-green"></i>';
    }

    return '<i class="fa fa-times text-red"></i>';
};

var dtUtils = new DataTablesUtils();