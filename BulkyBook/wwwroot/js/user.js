var dataTable;

$(document).ready(function () {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            {
                "data": "name", "width": "12%"
            },
            {
                "data": "email", "width": "12%"
            },
            {
                "data": "streetAddress", "width": "12%"
            },
            {
                "data": "phoneNumber", "width": "12%"
            },
            {
                "data": "role", "width": "12%"
            },
            {
                "data": "company.name", "width": "12%"
            },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {

                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return `
                        <div class="text-center">

    <a  onclick="LockUnlock('${data.id}')" class="btn btn-success text-white" style="cursor:pointer;">
        <i class="fas fa-lock-open"></i> Unlock
    </a>

</div>
                            `;
                    } else {
                        return `
                        <div class="text-center">

    <a  onclick="LockUnlock('${data.id}')" class="btn btn-danger text-white" style="cursor:pointer;">
        <i class="fas fa-lock"></i> Lock
    </a>

</div>
                            `;
                    }
                }
                    
                , "width": "28%"
            }
        ]
    });
});

function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: "/Admin/User/LockUnlock",
        data: JSON.stringify(id),
        contentType : "application/json",
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