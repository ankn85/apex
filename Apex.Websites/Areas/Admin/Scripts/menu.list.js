(function ($) {
    "use strict";
    var $deleteButton = null;
    var $checkboxAll = null;
    var $checkboxItems = null;

    function ajaxDeleteHandler(checkedValues) {
        $.post(
            "/admin/menu/delete",
            { ids: checkedValues },
            function (data, status, xhr) {
                if (xhr.status === 200) {
                    location.href = location.href;
                }
            },
            "json");
    }

    function deleteHandler() {
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
    }

    function checkboxAllChangedHandler() {
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
    }

    function checkboxChangedHandler() {
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
    }

    function initialize() {
        $deleteButton = $("#btnDelete")
            .off()
            .on("click", deleteHandler);

        $checkboxAll = $("#checkbox-all")
            .off()
            .on("change", checkboxAllChangedHandler);

        $checkboxItems = $('.checkbox-control input[type="checkbox"]')
            .off()
            .on("change", checkboxChangedHandler);
    }

    initialize();
})(jQuery);
