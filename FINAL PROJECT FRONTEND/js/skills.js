const API = "https://localhost:7230/api/Skill";
const EMP_API = "https://localhost:7230/api/Employee/GetAllEmployees";
const REPORT_API = "https://localhost:7230/api/Report/skill-availability";
 
function alertMessage(msg, type = "info") {
    let color = type === "success" ? "green" :
                type === "error" ? "red" : "blue";
 
    $("#alerts").html(`<div class="alert" style="border:1px solid ${color}; color:${color}">${msg}</div>`);
    setTimeout(() => $("#alerts").html(""), 4000);
}
 
function showSection(id) {
    $("section").hide();
    $("#" + id).show();
 
    if (id !== "listSection") loadDropdowns();
    if (id === "listSection") loadSkills();
}
 
/* -------------------- LOAD DROPDOWNS -------------------- */
function loadDropdowns() {
    // Load skills
    $.ajax({
        url: API + "/list",
        method: "POST",
        success: function (skills) {
            let options = '<option value="">-- Select Skill --</option>' +
                skills.map(s =>
                    `<option value="${s.skillName}" data-id="${s.skillID}">${s.skillName}</option>`
                ).join("");
 
            $("#deleteSkillDropdown").html(options);
            $("#assignSkillDropdown").html(options);
            $("#removeSkillDropdown").html(options);
        }
    });
 
    // Load employees
    $.ajax({
        url: EMP_API,
        method: "POST",
        success: function (employees) {
            let opts = '<option value="">-- Select Employee --</option>' +
                employees.map(e => `<option value="${e.fullName}">${e.fullName}</option>`).join("");
 
            $("#assignEmployeeDropdown").html(opts);
            $("#removeEmployeeDropdown").html(opts);
            $("#employeeSkillsDropdown").html(opts);
        }
    });
}
 
/* -------------------- LIST SKILLS -------------------- */
function loadSkills() {
    $.ajax({
        url: API + "/list",
        method: "POST",
        success: function (res) {
            $("#skillsTable tbody").html(
                res.map((s, i) => `
                    <tr>
                        <td>${i + 1}</td>
                        <td>${s.skillName}</td>
                        <td>${s.description || "-"}</td>
                        <td>
                            <button class="btn primary editBtn"
                                data-id="${s.skillID}"
                                data-name="${s.skillName}"
                                data-desc="${s.description || ''}">
                                Edit
                            </button>
                            <button class="btn danger deleteBtn"
                                data-name="${s.skillName}">
                                Delete
                            </button>
                        </td>
                    </tr>
                `).join("")
            );
        }
    });
}
 
/* -------------------- CREATE SKILL -------------------- */
$("#createSkillForm").on("submit", function (e) {
    e.preventDefault();
 
    $.ajax({
        url: API + "/create",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            skillName: $("#createSkillName").val(),
            description: $("#createSkillDesc").val()
        }),
        success: function () {
            alertMessage("Skill created", "success");
            $("#createSkillForm")[0].reset();
            showSection("listSection");
        },
        error: function () {
            alertMessage("Failed to create skill", "error");
        }
    });
});
 
/* -------------------- UPDATE -------------------- */
$("#updateSkillForm").on("submit", function (e) {
    e.preventDefault();
 
    let id = $("#updateSkillForm").data("skillId");
 
    $.ajax({
        url: API + `/update?id=${id}`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            skillName: $("#updateSkillName").val(),
            description: $("#updateSkillDesc").val()
        }),
        success: function () {
            alertMessage("Skill updated", "success");
            showSection("listSection");
        },
        error: function () {
            alertMessage("Failed to update skill", "error");
        }
    });
});
 
/* -------------------- DELETE (NEW LOGIC WITH AVAILABILITY CHECK) -------------------- */
async function checkIfSkillAssigned(skillName) {
    return new Promise((resolve) => {
        $.ajax({
            url: REPORT_API,
            method: "POST",
            success: function (report) {
                let entry = report.find(x => x.skillName.trim().toLowerCase() === skillName.trim().toLowerCase());
 
                // if availableCount > 0 → skill has assigned employees
                if (entry && entry.availableCount > 0) resolve(true);
                else resolve(false);
            },
            error: function () {
                resolve(false);
            }
        });
    });
}
 
$(document).on("click", ".deleteBtn", async function () {
    let skillName = $(this).data("name");
 
    if (!confirm(`Delete skill "${skillName}"?`)) return;
 
    const isAssigned = await checkIfSkillAssigned(skillName);
 
    if (isAssigned) {
        alertMessage("Employee is assigned to this skill — cannot delete.", "error");
        return;
    }
 
    // DELETE since skill is NOT assigned
    $.ajax({
        url: API + `/delete?skillName=${encodeURIComponent(skillName)}`,
        method: "POST",
        success: function (response) {
            if (response?.message?.toLowerCase().includes("deleted")) {
                alertMessage("Deleted successfully", "success");
            } else {
                alertMessage(response.message || "Could not delete skill", "error");
            }
 
            loadSkills();
            loadDropdowns();
        },
        error: function () {
            alertMessage("Failed to delete skill", "error");
        }
    });
});
 
/* -------------------- ASSIGN SKILL -------------------- */
$("#assignSkillForm").on("submit", function (e) {
    e.preventDefault();
 
    $.ajax({
        url: API + "/assign",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            employeeName: $("#assignEmployeeDropdown").val(),
            skillName: $("#assignSkillDropdown").val()
        }),
        success: function () {
            alertMessage("Assigned", "success");
            showSection("listSection");
        }
    });
});
 
/* -------------------- REMOVE SKILL -------------------- */
$("#removeSkillForm").on("submit", function (e) {
    e.preventDefault();
 
    $.ajax({
        url: API + "/remove",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            employeeName: $("#removeEmployeeDropdown").val(),
            skillName: $("#removeSkillDropdown").val()
        }),
        success: function () {
            alertMessage("Removed", "success");
            showSection("listSection");
        }
    });
});
 
/* -------------------- EMPLOYEE SKILLS -------------------- */
$("#employeeSkillsForm").on("submit", function (e) {
    e.preventDefault();
 
    let emp = $("#employeeSkillsDropdown").val();
 
    $.ajax({
        url: API + "/employee/" + emp,
        method: "POST",
        success: function (data) {
            $("#employeeSkillsList").html(
                data.map(s => `<p><b>${s.skillName}</b> - ${s.description || ""}</p>`).join("")
            );
        }
    });
});
 
/* -------------------- EDIT BUTTON -------------------- */
$(document).on("click", ".editBtn", function () {
    let skillId = $(this).data("id");
    let skillName = $(this).data("name");
    let skillDesc = $(this).data("desc");
 
    $("#updateCurrentSkillName").val(skillName);
    $("#updateSkillName").val("");
    $("#updateSkillDesc").val(skillDesc);
 
    $("#updateSkillForm").data("skillId", skillId);
 
    showSection("updateSection");
});
 
/* -------------------- INITIAL -------------------- */
$(document).ready(() => showSection("listSection"));