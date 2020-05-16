
var dataTable; 

$(document).ready(function () {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url":"/Admin/Company/GetAll"
        }, 
        "columns": [
            {
                "data": "name", "width":"12%"
            }, 
            {
                "data": "streetAddress", "width": "12%"
            },
            {
                "data": "city", "width": "12%"
            }, 
            {
                "data": "state", "width": "12%"
            },
            {
                "data": "postalCode", "width": "12%"
            },
            {
                "data": "phoneNumber", "width": "12%"
            },  
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked />`
                    }
                    else {
                        return `<input type="checkbox" disabled/>`
                    }
                },
                "width": "10%"
            },
            {
                "data": "id", 
                "render": function (data) {
                    return `
                          <div class="text-center">
    <a  href ="/Admin/Company/Upsert/${data}"class="btn btn-success text-white" style="cursor:pointer;">
        <i class="fas fa-edit"></i>
    </a>
    <a  onClick="Delete('/Admin/Company/Delete/${data}')" class="btn btn-danger text-white" style="cursor:pointer;">
        <i class="fas fa-trash-alt"></i>
    </a>

</div>
                            `
                }, "width":"16%"
            }
        ]
    });
});


function Delete(url) {
    swal({
        title: "Do you really want to delete this Cover Type",
        text: "You will not be able to undo this action",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else
                        toastr.error(data.message);
                }
            });
        }
    });
}
