function getDistricts() {
    var selectedProvinceId = $("#province").val();
    $.ajax({
        url: `/Identity/Districts`, // Replace with your controller action URL
        type: 'GET',
        data: { provinceId: selectedProvinceId },
        success: function (data) {
            // Update the district dropdown with received data
            $("#district").empty().prop("disabled", false);
            for (var i = 0; i < data.length; i++) {
                $("#district").append($('<option></option>').val(data[i].districtId).text(data[i].districtName));
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error:", textStatus, errorThrown);
        }
    });
}