// // ================================================================
// // GLOBAL CONFIG
// // ================================================================
// const API_BASE = "https://localhost:7230/api/employee";
// // ================================================================
// // GLOBAL ALERT FUNCTIONS (YOUR ORIGINAL CODE - UNMODIFIED)
// // ================================================================
// function showAlert(message, type = 'info') {
//     $('.alert').remove();
//     const icon = type === 'success' ? 'check-circle' :
//                  type === 'error' ? 'exclamation-circle' : 'info-circle';

//     const alert = $(`
//         <div class="alert alert-${type} slide-in-left bounce-in">
//             <i class="fas fa-${icon}"></i> ${message}
//         </div>
//     `);

//     $('.container').prepend(alert);
//     setTimeout(() => alert.fadeOut(300, function () { $(this).remove(); }), 5000);
// }

// function showError(xhr) {
//     let message = 'An error occurred';
//     if (xhr.responseJSON?.message) message = xhr.responseJSON.message;
//     else if (xhr.status) message = `Server error: ${xhr.status}`;
//     showAlert(message, 'error');
// }

// function debounce(func, wait) {
//     let timeout;
//     return function executedFunction(...args) {
//         clearTimeout(timeout);
//         timeout = setTimeout(() => func(...args), wait);
//     };
// }

// // ================================================================
// // EMPLOYEE CRUD IMPLEMENTATION
// // ================================================================

// // -------------------------
// // 1. LOAD ALL EMPLOYEES
// // -------------------------
// function loadEmployees() {
//     $.ajax({
//         url: API_BASE,
//         method: "GET",
//         success: function (data) {
//             let rows = "";

//             data.forEach(e => {
//                 rows += `
//                     <tr>
//                         <td>${e.employeeID}</td>
//                         <td>${e.fullName}</td>
//                         <td>${e.email}</td>
//                         <td>${e.phone}</td>
//                         <td>${e.gender}</td>
//                         <td>${e.designation}</td>
//                         <td>${e.experienceInYears}</td>
//                         <td class="${e.isActive ? 'active' : 'inactive'}">
//                             ${e.isActive ? "Active" : "Inactive"}
//                         </td>
//                         <td>
//                             <button class="btn-edit" onclick="openEditForm(${e.employeeID}, '${e.fullName}', '${e.phone}', '${e.gender}', '${e.designation}', ${e.experienceInYears})">Edit</button>
//                             <button class="btn-toggle" onclick="toggleEmployee(${e.employeeID}, ${e.isActive})">
//                                 ${e.isActive ? "Deactivate" : "Activate"}
//                             </button>
//                         </td>
//                     </tr>
//                 `;
//             });

//             $("#employeeTable tbody").html(rows);
//             showAlert("Employees loaded successfully", "success");
//         },
//         error: showError
//     });
// }

// // -------------------------
// // 2. ADD EMPLOYEE
// // -------------------------
// $("#addEmployeeForm").submit(function (e) {
//     e.preventDefault();

//     const data = {
//         fullName: $("#add_fullName").val(),
//         email: $("#add_email").val(),
//         phone: $("#add_phone").val(),
//         gender: $("#add_gender").val(),
//         designation: $("#add_designation").val(),
//         experienceInYears: $("#add_experience").val()
//     };

//     $.ajax({
//         url: API_BASE,
//         method: "POST",
//         contentType: "application/json",
//         data: JSON.stringify(data),
//         success: function (res) {
//             showAlert(res.message, "success");
//             loadEmployees();
//         },
//         error: showError
//     });
// });

// // -------------------------
// // 3. OPEN EDIT FORM
// // -------------------------
// function openEditForm(id, name, phone, gender, designation, exp) {
//     $("#upd_employeeID").val(id);
//     $("#upd_fullName").val(name);
//     $("#upd_phone").val(phone);
//     $("#upd_gender").val(gender);
//     $("#upd_designation").val(designation);
//     $("#upd_experience").val(exp);

//     $("#listSection").hide();
//     $("#updateSection").show();
// }

// // -------------------------
// // 4. UPDATE EMPLOYEE
// // -------------------------
// $("#updateEmployeeForm").submit(function (e) {
//     e.preventDefault();

//     const id = $("#upd_employeeID").val();

//     const data = {
//         fullName: $("#upd_fullName").val(),
//         phone: $("#upd_phone").val(),
//         gender: $("#upd_gender").val(),
//         designation: $("#upd_designation").val(),
//         experienceInYears: $("#upd_experience").val()
//     };

//     $.ajax({
//         url: `${API_BASE}/${id}`,
//         method: "POST",
//         contentType: "application/json",
//         data: JSON.stringify(data),
//         success: function (res) {
//             showAlert(res.message, "success");
//             loadEmployees();
//             $("#updateSection").hide();
//             $("#listSection").show();
//         },
//         error: showError
//     });
// });

// // -------------------------
// // 5. ACTIVATE OR DEACTIVATE EMPLOYEE
// // -------------------------
// function toggleEmployee(id, isActive) {
//     const data = { isActive: !isActive };

//     $.ajax({
//         url: `${API_BASE}/${id}/status`,
//         method: "POST",
//         contentType: "application/json",
//         data: JSON.stringify(data),
//         success: function (res) {
//             showAlert(res.message, "success");
//             loadEmployees();
//         },
//         error: showError
//     });
// }

// // ================================
// // INITIAL LOAD
// // ================================
// $(document).ready(() => {
//     loadEmployees();
//     $("#listSection").show();
// });
