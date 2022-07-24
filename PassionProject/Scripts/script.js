window.onload = function () {
    $('#services-list').change(function () {
        var service = $(this).val();
        var employees = $('#employee-list');
        employees.empty();
        // clear existing options
        $.ajax({
            url: 'https://passionproject20220717203816.azurewebsites.net/api/EmployeeData/GetEmployeesByServiceId/'+ service, // do not hard code url's
            type: "GET",
            dataType: "json",
            success: function (data) {
                if (data) {
                    console.log("Successfull!", data);
                    employees.append($('<option></option>').val("X").html("Please Select"));
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

    $("#imgInput").change(function () {
        readURL(this);
    });
    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#profile-pic').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    
}