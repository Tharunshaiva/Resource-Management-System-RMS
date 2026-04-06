/* ============================================================
   API ENDPOINTS
============================================================ */
const API_BASE = "https://localhost:7230/api/Employee";
const SKILLS_API = "https://localhost:7230/api/Skill";
 
const API = {
    list: `${API_BASE}/GetEmployees`,
    add: `${API_BASE}/AddEmployee`,
    update: `${API_BASE}/UpdateEmployee`,
    status: `${API_BASE}/SetEmployeeStatus`
};
 
/* ============================================================
   GLOBAL STATE
============================================================ */
let allEmployees = [];
let currentPage = 1;
let sortColumn = "employeeID";
let sortAscending = true;
 
/* ============================================================
   ALERT MESSAGE
============================================================ */
function alertMessage(msg, type = "info") {
    let color = type === "success" ? "green" :
                type === "error" ? "red" : "blue";
 
    $("#alerts").html(
        `<div class="alert" style="border:1px solid ${color};color:${color};padding:10px">${msg}</div>`
    );
 
    setTimeout(() => $("#alerts").html(""), 4000);
}
 
/* ============================================================
   SHOW SECTION
============================================================ */
function showSection(id) {
    $("section").hide();
    $("#" + id).show();
 
    if (id === "listSection") {
        loadEmployees();
    } else if (id === "addSection") {
        $("#add_skills").val("");
        updateSkillsDisplay();
    }
}
 
/* ============================================================
   VALIDATION HELPERS
============================================================ */
const isValidEmail = email => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
const isValidPhone = phone => phone === "" || /^[0-9]{7,10}$/.test(phone);
const isValidName = name => /^[A-Za-z\s]+$/.test(name);
 
/* ============================================================
   LOAD EMPLOYEES (PAGINATED)
============================================================ */
function loadEmployees() {
    let pageSize = $("#pageSize").val();
 
    $.ajax({
        url: `${API.list}?pageNumber=${currentPage}&pageSize=${pageSize}`,
        method: "POST",
        success: function (response) {
            allEmployees = response.items;
            populateFilterDropdowns();
            renderTable();
            renderPagination(response.pageNumber, response.totalPages);
        },
        error: function () {
            alertMessage("Failed to load employees.", "error");
        }
    });
}
 
/* ============================================================
   RENDER TABLE
============================================================ */
function renderTable() {
    const searchTerm = $("#searchBox").val().toLowerCase();
    const statusFilter = $("#statusFilter").val();
    const genderFilter = $("#filter_gender").val();
    const designationFilter = $("#filter_designation").val();
    const experienceFilter = $("#filter_experienceInYears").val();
 
    let filtered = allEmployees.filter(e => {
        let match = e.fullName.toLowerCase().includes(searchTerm);
 
        if (statusFilter === "Active") match &= e.isActive;
        if (statusFilter === "Inactive") match &= !e.isActive;
        if (genderFilter) match &= e.gender === genderFilter;
        if (designationFilter) match &= e.designation === designationFilter;
        if (experienceFilter) match &= e.experienceInYears == experienceFilter;
 
        return match;
    });
 
    // Sorting
    filtered.sort((a, b) => {
        let x = a[sortColumn], y = b[sortColumn];
 
        if (typeof x === "number")
            return sortAscending ? x - y : y - x;
 
        return sortAscending
            ? String(x).localeCompare(String(y))
            : String(y).localeCompare(String(x));
    });
 
    let tbody = $("#employeeTable tbody");
    tbody.empty();
 
    filtered.forEach(emp => {
        tbody.append(`
            <tr>
                <td>${emp.employeeID}</td>
                <td>${emp.fullName}</td>
                <td>${emp.email}</td>
                <td>${emp.phone}</td>
                <td>${emp.gender}</td>
                <td>${emp.designation}</td>
                <td>${emp.experienceInYears}</td>
                <td>${emp.isActive ? "Active" : "Inactive"}</td>
 
                <td>
                    <button class="btn primary editBtn"
                        data-id="${emp.employeeID}"
                        data-name="${emp.fullName}"
                        data-email="${emp.email}"
                        data-phone="${emp.phone}"
                        data-gender="${emp.gender}"
                        data-designation="${emp.designation}"
                        data-exp="${emp.experienceInYears}">
                        Edit
                    </button>
 
                    <button class="btn warn statusBtn"
                        data-id="${emp.employeeID}"
                        data-status="${emp.isActive}">
                        ${emp.isActive ? "Deactivate" : "Activate"}
                    </button>
                </td>
            </tr>
        `);
    });
}
 
/* ============================================================
   PAGINATION
============================================================ */
function renderPagination(pageNumber, totalPages) {
    let pagination = $("#paginationControls");
    pagination.empty();
 
    if (pageNumber > 1)
        pagination.append(`<button onclick="goToPage(${pageNumber - 1})">Prev</button>`);
 
    for (let i = 1; i <= totalPages; i++) {
        pagination.append(`
            <button class="${i === pageNumber ? "active-page" : ""}"
                onclick="goToPage(${i})">${i}
            </button>
        `);
    }
 
    if (pageNumber < totalPages)
        pagination.append(`<button onclick="goToPage(${pageNumber + 1})">Next</button>`);
}
 
function goToPage(page) {
    currentPage = page;
    loadEmployees();
}
 
/* ============================================================
   POPULATE FILTERS
============================================================ */
function populateFilterDropdowns() {
    const designations = [...new Set(allEmployees.map(e => e.designation))].filter(Boolean);
    const experiences = [...new Set(allEmployees.map(e => e.experienceInYears))]
        .filter(e => e !== null && e !== undefined)
        .sort((a, b) => a - b);
 
    $("#filter_designation").html(
        `<option value="">All Designations</option>` +
        designations.map(d => `<option value="${d}">${d}</option>`).join("")
    );
 
    $("#filter_experienceInYears").html(
        `<option value="">All Experience Levels</option>` +
        experiences.map(exp => `<option value="${exp}">${exp} years</option>`).join("")
    );
}
 
/* ============================================================
   CLEAR FILTERS
============================================================ */
function clearAllFilters() {
    $("#searchBox").val("");
    $("#statusFilter").val("");
    $("#filter_gender").val("");
    $("#filter_designation").val("");
    $("#filter_experienceInYears").val("");
 
    renderTable();
    alertMessage("All filters cleared", "info");
}
 
/* ============================================================
   ADD EMPLOYEE
============================================================ */
$("#addEmployeeForm").on("submit", function (e) {
    e.preventDefault();
 
    let fullName = $("#add_fullName").val().trim();
    let email = $("#add_email").val().trim();
    let phone = $("#add_phone").val().trim();
    let gender = $("#add_gender").val();
    let designation = $("#add_designation").val().trim();
    let experience = $("#add_experience").val().trim();
 
    const mapGender = g =>
        g === "M" ? "Male" :
        g === "F" ? "Female" :
        g === "O" ? "Other" : g;
 
    if (!fullName || !isValidName(fullName)) return alertMessage("Invalid name", "error");
    if (!email || !isValidEmail(email)) return alertMessage("Invalid email", "error");
    if (!isValidPhone(phone)) return alertMessage("Phone must be 7–10 digits", "error");
    if (!gender) return alertMessage("Gender is required", "error");
 
    const newEmployee = {
        fullName,
        email,
        phone,
        gender: mapGender(gender),
        designation,
        experienceInYears: Number(experience || 0)
    };
 
    // 1. Create employee
    $.ajax({
        url: API.add,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(newEmployee),
        success: function () {
            let selectedSkills = $("#add_skills").val()
                ? $("#add_skills").val().split(",").filter(s => s.trim())
                : [];
 
            if (selectedSkills.length === 0) {
                alertMessage("Employee added!", "success");
                $("#addEmployeeForm")[0].reset();
                updateSkillsDisplay();
                showSection("listSection");
                return;
            }
 
            // 2. Assign skills
            let skillRequests = selectedSkills.map(skill =>
                $.ajax({
                    url: `${SKILLS_API}/assign`,
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        employeeName: fullName,
                        skillName: skill
                    })
                })
            );
 
            $.when.apply($, skillRequests)
                .done(() => {
                    alertMessage("Employee added with skills!", "success");
                    $("#addEmployeeForm")[0].reset();
                    updateSkillsDisplay();
                    showSection("listSection");
                })
                .fail(() => {
                    alertMessage("Employee added, but skill assignment failed", "error");
                });
        },
        error: function (xhr) {
            alertMessage("Failed to add employee: " + xhr.responseText, "error");
        }
    });
});
 
/* ============================================================
   EDIT EMPLOYEE
============================================================ */
$(document).on("click", ".editBtn", function () {
    $("#upd_employeeID").val($(this).data("id"));
    $("#upd_fullName").val($(this).data("name"));
    $("#upd_email").val($(this).data("email"));
    $("#upd_phone").val($(this).data("phone"));
    $("#upd_gender").val($(this).data("gender"));
    $("#upd_designation").val($(this).data("designation"));
    $("#upd_experience").val($(this).data("exp"));
 
    // LOAD EMPLOYEE SKILLS FOR UPDATE
    $.ajax({
        url: `${SKILLS_API}/getByEmployee/${$(this).data("name")}`,
        method: "POST",
        success: function (skills) {
            let skillNames = skills.map(s => s.skillName);
            $("#upd_skills").val(skillNames.join(","));
            updateUpdateSkillDisplay();
        }
    });
 
    showSection("updateSection");
});
 
/* ============================================================
   UPDATE EMPLOYEE
============================================================ */
$("#updateEmployeeForm").on("submit", function (e) {
    e.preventDefault();
 
    let data = {
        EmployeeID: Number($("#upd_employeeID").val()),
        FullName: $("#upd_fullName").val().trim(),
        Email: $("#upd_email").val().trim(),
        Phone: $("#upd_phone").val().trim(),
        Gender: $("#upd_gender").val(),
        Designation: $("#upd_designation").val().trim(),
        ExperienceInYears: Number($("#upd_experience").val() || 0)
    };
 
    $.ajax({
        url: `${API.update}/${data.EmployeeID}`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function () {
            alertMessage("Employee updated!", "success");
            showSection("listSection");
        },
        error: function (xhr) {
            alertMessage("Update failed: " + xhr.responseText, "error");
        }
    });
});
 
/* ============================================================
   ACTIVATE / DEACTIVATE EMPLOYEE
============================================================ */
$(document).on("click", ".statusBtn", function () {
    let id = $(this).data("id");
    let newStatus = !$(this).data("status");
 
    $.ajax({
        url: `${API.status}/status/${id}`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ IsActive: newStatus }),
        success: function (res) {
            alertMessage(res.message || "Status updated!", "success");
            loadEmployees();
        },
        error: function (xhr) {
            alertMessage("Status update failed: " + xhr.responseText, "error");
        }
    });
});
 
/* ============================================================
   ADD SKILL MODAL (EXISTING FUNCTIONALITY)
============================================================ */
function openAddSkillModal() {
    $("#addSkillModal").show();
    loadSkillsForModal();
}
 
function closeAddSkillModal() {
    $("#addSkillModal").hide();
}
 
function loadSkillsForModal() {
    $.ajax({
        url: SKILLS_API + "/list",
        method: "POST",
        success: function (skills) {
            $("#skillCheckboxList").html(
                skills.map(s => `
                    <label>
                        <input type="checkbox" class="modal-skill-checkbox" value="${s.skillName}">
                        ${s.skillName}
                    </label>
                `).join("")
            );
        }
    });
}
 
function updateSkillsDisplay() {
    let skills = $("#add_skills").val().split(",").filter(s => s.trim());
 
    if (skills.length === 0) {
        $("#skillsList").html('<span style="color:#999;">No skills selected</span>');
        return;
    }
 
    $("#skillsList").html(
        skills.map(skill => `
            <span class="skill-tag">
                ${skill}
                <button class="remove-skill-btn" data-skill="${skill}">×</button>
            </span>
        `).join("")
    );
}
 
$(document).on("click", ".remove-skill-btn", function () {
    let skill = $(this).data("skill");
    let skills = $("#add_skills").val().split(",").filter(s => s !== skill);
    $("#add_skills").val(skills.join(","));
    updateSkillsDisplay();
});
 
function saveNewSkill() {
    let selected = $(".modal-skill-checkbox:checked")
        .map(function () { return this.value; })
        .get();
 
    let oldSkills = $("#add_skills").val().split(",").filter(s => s.trim());
 
    selected.forEach(s => {
        if (!oldSkills.includes(s)) oldSkills.push(s);
    });
 
    $("#add_skills").val(oldSkills.join(","));
    updateSkillsDisplay();
    closeAddSkillModal();
}
 
$("#addSkillBtn").click(openAddSkillModal);
 
/* ============================================================
   UPDATE SKILLS MODAL (NEW FUNCTIONALITY)
============================================================ */
function openUpdateSkillModal() {
    $("#updateSkillModal").show();
    loadSkillsForUpdateModal();
}
 
function closeUpdateSkillModal() {
    $("#updateSkillModal").hide();
}
 
function loadSkillsForUpdateModal() {
    let currentSkills = $("#upd_skills").val().split(",").filter(s => s.trim());
 
    $.ajax({
        url: SKILLS_API + "/list",
        method: "POST",
        success: function (skills) {
            $("#updateSkillCheckboxList").html(
                skills.map(s => `
                    <label style="display:block;margin:5px 0;">
                        <input type="checkbox"
                            class="update-skill-checkbox"
                            value="${s.skillName}"
                            ${currentSkills.includes(s.skillName) ? "checked" : ""}>
                        ${s.skillName}
                    </label>
                `).join("")
            );
        }
    });
}
 
function updateUpdateSkillDisplay() {
    let skills = $("#upd_skills").val().split(",").filter(s => s.trim());
 
    if (skills.length === 0) {
        $("#upd_skill_display").html('<span style="color:#777;">No skills selected</span>');
        return;
    }
 
    $("#upd_skill_display").html(
        skills.map(s => `
            <span class="skill-tag">${s}</span>
        `).join("")
    );
}
 
function saveUpdatedSkills() {
    let newSkills = $(".update-skill-checkbox:checked")
        .map(function () { return this.value; })
        .get();
 
    let oldSkills = $("#upd_skills").val().split(",").filter(s => s.trim());
    let employeeName = $("#upd_fullName").val().trim();
 
    // Determine which skills to add or remove
    let skillsToAdd = newSkills.filter(s => !oldSkills.includes(s));
    let skillsToRemove = oldSkills.filter(s => !newSkills.includes(s));
 
    let addRequests = skillsToAdd.map(skill =>
        $.ajax({
            url: `${SKILLS_API}/assign`,
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                employeeName,
                skillName: skill
            })
        })
    );
 
    let removeRequests = skillsToRemove.map(skill =>
        $.ajax({
            url: `${SKILLS_API}/remove`,
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                employeeName,
                skillName: skill
            })
        })
    );
 
    $.when(...addRequests, ...removeRequests)
        .done(() => {
            $("#upd_skills").val(newSkills.join(","));
            updateUpdateSkillDisplay();
            closeUpdateSkillModal();
            alertMessage("Skills updated successfully!", "success");
        })
        .fail(() => {
            alertMessage("Some skill updates failed.", "error");
        });
}
 
/* ============================================================
   FILTER & SORT EVENTS
============================================================ */
$("#searchBox").on("keyup", renderTable);
$("#statusFilter, #filter_gender, #filter_designation, #filter_experienceInYears")
    .on("change", renderTable);
 
$("#pageSize").on("change", function () {
    currentPage = 1;
    loadEmployees();
});
 
$(document).on("click", ".sortable", function () {
    let col = $(this).data("column");
 
    if (sortColumn === col) sortAscending = !sortAscending;
    else { sortColumn = col; sortAscending = true; }
 
    renderTable();
});
 
/* ============================================================
   INITIAL LOAD
============================================================ */
$(document).ready(() => showSection("listSection"));
 
 