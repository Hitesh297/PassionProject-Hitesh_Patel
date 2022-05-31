window.onload = function () {
    $('#services-list').change(function () {
        var service = $(this).val();
        var employees = $('#employee-list');
        // clear existing options
        $.ajax({
            url: 'https://localhost:44364/api/EmployeeData/GetEmployeesByServiceId/'+ service, // do not hard code url's
            type: "GET",
            dataType: "json",
            success: function (data) {
                if (data) {
                    console.log("Successfull!",data);
                    $.each(data, function (index, item) {
                        employees.append($('<option></option>').val(item.EmployeeId).html(item.Fname));
                    });
                } else {
                    // oops
                }
            },
            error: function (err) {
                console.log("Error occured!");
                alert(err);
            }
        });
    });
}