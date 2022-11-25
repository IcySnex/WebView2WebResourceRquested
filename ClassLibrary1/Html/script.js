function Add(x, y) {
    return x + y;
}

$(function () {
    $("#mybutton").click(function () {
        $.ajax({
            url: "https://httpbin.org/html",
            //data: {
            //    zipcode: 97201
            //},
            success: function (result) {
                $("#response").html("success:" + result);
            },
            error: function (result) {
                $("#response").html("error:" + JSON.stringify(result));
            }
        });

        //$.get("https://httpbin.org/html")
        //    .done(function (data) {
        //        $("#response").html(data);
        //    });
    });

});