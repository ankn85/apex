"use strict";

function DataTablesUtils() {
}

DataTablesUtils.prototype.renderStatusCell = function (status) {
    if (status) {
        return '<i class="fa fa-check text-green"></i>';
    }

    return '<i class="fa fa-times text-red"></i>';
};

DataTablesUtils.prototype.truncateText = function (text, maxLength) {
    if (!maxLength || text.length <= maxLength) {
        return text;
    }

    return [
        '<span title="',
        text,
        '">',
        text.substr(0, maxLength - 3),
        "...</span>"
    ].join("");
};

var dtUtils = new DataTablesUtils();