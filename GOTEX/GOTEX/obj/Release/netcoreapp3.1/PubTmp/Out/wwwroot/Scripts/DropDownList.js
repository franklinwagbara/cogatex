$(document).ready(function () {
    $("#txtRole").ready(function () {
        var html = "";

        $("#txtRole").html("");

        $.getJSON("/Admin/GetFire",
            {},
            function (datas) {
                $("#txtRole").append("<option disabled selected>--Select Role--</option>");
                $.each(datas,
                    function (key, val) {
                        html += "<option value=" + val.RoleID + ">" + val.RoleType + "</option>";
                    });
                $("#txtRole").append(html);
            });
    });

})