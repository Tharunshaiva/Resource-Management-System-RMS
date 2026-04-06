 
const API_BASE = "https://localhost:7230/api/projects";
 
const API = {
  list:       `${API_BASE}/search`,
  add:        `${API_BASE}/create`,
  update:     `${API_BASE}/update`,
  deactivate: `${API_BASE}/deactivate`,
  setStatus:  `${API_BASE}/set-status`
};
 
// Pagination & filter state
let projCurrentPage = 1;
let projPageSize = 5; // per requirement
let projStatusFilter = 'Active';
 
// External working-days endpoint
const WORKING_DAYS_API = "http://127.0.0.1:5001/working-days";
 
function alertMessage(msg,type="info"){
  let color = type==="success"?"green":type==="error"?"red":"blue";
  $("#alerts").html(`<div class="alert" style="border:1px solid ${color}; color:${color}">${msg}</div>`);
  setTimeout(()=>$("#alerts").html(""),4000);
}
 
function showSection(id){
  $("section").hide();
  $("#"+id).show();
  if(id==="listSection") loadProjects();
}
 
// Validation functions
function isValidName(name){
  return /^[A-Za-z\s]+$/.test(name);
}
function isValidNumber(num){
  return !isNaN(num) && Number(num) >= 0;
}
function isValidDate(date){
  return !date || !isNaN(new Date(date).getTime());
}
 
// Load Projects
function loadProjects(page = 1, status = projStatusFilter) {
  projCurrentPage = page;
  projStatusFilter = status;
 
  // Build request body per requirement
  const reqBody = { status: status, page: page, pageSize: projPageSize };
 
  $.ajax({
    url: API.list,
    method: "POST",
    contentType: "application/json",
    data: JSON.stringify(reqBody),
    success: function (res) {
      // Expect `res.data` to be array and optionally res.totalCount
      const projects = res.data || [];
 
      // Update pagination UI
      $("#pageInfo").text(`Page ${projCurrentPage}`);
      // Enable/disable prev
      $("#prevPage").prop('disabled', projCurrentPage <= 1);
 
      // If API returned totalCount, compute whether next exists
      let hasNext = false;
      if (typeof res.totalCount === 'number') {
        const totalPages = Math.ceil(res.totalCount / projPageSize);
        hasNext = projCurrentPage < totalPages;
      } else {
        // Fallback: if returned items length === pageSize, assume possible next
        hasNext = Array.isArray(projects) && projects.length === projPageSize;
      }
      $("#nextPage").prop('disabled', !hasNext);
 
      // Fetch working days (external) then render rows
      $.ajax({
        url: WORKING_DAYS_API,
        method: "GET",
        dataType: "json",
        success: function (wres) {
          const wdMap = {};
          (wres.working_days || []).forEach(item => { wdMap[item.projectID] = item.workingDays; });
 
          const rows = projects.map(p => rowForProject(p, wdMap[p.projectID] || '')).join("");
          $("#projectTable tbody").html(rows);
        },
        error: function () {
          // If working-days endpoint fails, render projects without working days
          alertMessage("Failed to load working days", "error");
          const rows = projects.map(p => rowForProject(p, '')).join("");
          $("#projectTable tbody").html(rows);
        }
      });
    },
    error: function () { alertMessage("Failed to load projects", "error"); }
  });
}
 
// Helper to build table row for a project
function rowForProject(p, workingDaysValue) {
  return `
    <tr>
      <td>${p.projectID}</td>
      <td>${p.projectName}</td>
      <td>${p.clientName}</td>
      <td>${p.capacity}</td>
      <td class="working-days-col">${workingDaysValue}</td>
      <td>${p.startDate?.split('T')[0]||''}</td>
      <td>${p.endDate?.split('T')[0]||''}</td>
      <td>${p.projectStatus}</td>
      <td>
        <button class="btn primary editBtn"
          data-id="${p.projectID}"
          data-name="${p.projectName}"
          data-client="${p.clientName}"
          data-capacity="${p.capacity}"
          data-start="${p.startDate?.split('T')[0]||''}"
          data-end="${p.endDate?.split('T')[0]||''}"
          data-status="${p.projectStatus}">
          Edit
        </button>
        ${
          (p.projectStatus||"").toLowerCase() === "inactive"
            ? `<button class="btn warn activateBtn" data-id="${p.projectID}">Activate</button>`
            : `<button class="btn warn deactivateBtn" data-id="${p.projectID}">Deactivate</button>`
        }
      </td>
    </tr>`;
}
 
// Add Project
$("#addProjectForm").on("submit",function(e){
  e.preventDefault();
 
  let name = $("#add_name").val().trim();
  let client = $("#add_client").val().trim();
  let capacity = $("#add_capacity").val().trim();
  let start = $("#add_start").val();
  let end = $("#add_end").val();
  let status = $("#add_status").val();
 
  if(!name) return alertMessage("Project Name is required","error");
  if(!isValidName(name)) return alertMessage("Project Name can contain only letters and spaces","error");
  if(client && !isValidName(client)) return alertMessage("Client Name can contain only letters and spaces","error");
  if(capacity && !isValidNumber(capacity)) return alertMessage("Capacity must be a positive number","error");
  if(start && !isValidDate(start)) return alertMessage("Start Date is invalid","error");
  if(end && !isValidDate(end)) return alertMessage("End Date is invalid","error");
  if(start && end && new Date(end) < new Date(start)) return alertMessage("End Date cannot be before Start Date","error");
  if(!status) return alertMessage("Status is required","error");
 
  let data = {
    ProjectName: name,
    ClientName: client,
    Capacity: Number(capacity||0),
    StartDate: start,
    EndDate: end,
    ProjectStatus: status
  };
 
  $.ajax({
    url: API.add, method:"POST", contentType:"application/json", data: JSON.stringify(data),
    success:function(res){
      alertMessage("Project added","success");
      $("#addProjectForm")[0].reset();
      showSection("listSection");
    },
    error:function(){ alertMessage("Failed to add project","error"); }
  });
});
 
// Open Update Form
$(document).on("click",".editBtn",function(){
  $("#upd_projectID").val($(this).data("id"));
  $("#upd_name").val($(this).data("name"));
  $("#upd_client").val($(this).data("client"));
  $("#upd_capacity").val($(this).data("capacity"));
  $("#upd_start").val($(this).data("start"));
  $("#upd_end").val($(this).data("end"));
 
  // Status dropdown
  let statusValue = $(this).data("status")?.trim() || "";
  if(statusValue.toLowerCase() === "active") statusValue = "Active";
  else if(statusValue.toLowerCase() === "inactive") statusValue = "Inactive";
 
  $("#upd_status").val(statusValue);
  showSection("updateSection");
});
 
// Update Project
$("#updateProjectForm").on("submit",function(e){
  e.preventDefault();
 
  let name = $("#upd_name").val().trim();
  let client = $("#upd_client").val().trim();
  let capacity = $("#upd_capacity").val().trim();
  let start = $("#upd_start").val();
  let end = $("#upd_end").val();
  let status = $("#upd_status").val();
 
  if(!name) return alertMessage("Project Name is required","error");
  if(!isValidName(name)) return alertMessage("Project Name can contain only letters and spaces","error");
  if(client && !isValidName(client)) return alertMessage("Client Name can contain only letters and spaces","error");
  if(capacity && !isValidNumber(capacity)) return alertMessage("Capacity must be a positive number","error");
  if(start && !isValidDate(start)) return alertMessage("Start Date is invalid","error");
  if(end && !isValidDate(end)) return alertMessage("End Date is invalid","error");
  if(start && end && new Date(end) < new Date(start)) return alertMessage("End Date cannot be before Start Date","error");
  if(!status) return alertMessage("Status is required","error");
 
  let data = {
    ProjectID: Number($("#upd_projectID").val()),
    ProjectName: name,
    ClientName: client,
    Capacity: Number(capacity||0),
    StartDate: start,
    EndDate: end,
    ProjectStatus: status
  };
 
  $.ajax({
    url: API.update, method:"POST", contentType:"application/json", data: JSON.stringify(data),
    success:function(res){
      alertMessage("Project updated","success");
      showSection("listSection");
    },
    error:function(){ alertMessage("Failed to update project","error"); }
  });
});
 
// Deactivate Project
$(document).on("click",".deactivateBtn",function(){
  let id = $(this).data("id");
  $.ajax({
    url: API.deactivate, method:"POST", contentType:"application/json",
    data: JSON.stringify({ ProjectID:id }),
    success:function(){ alertMessage("Project deactivated","success"); loadProjects(); },
    error:function(){ alertMessage("Failed to deactivate","error"); }
  });
});
 
// Activate Project (new, minimal; same styling as Deactivate via "btn warn")
$(document).on("click",".activateBtn",function(){
  let id = $(this).data("id");
  // If your DTO uses boolean instead: { ProjectID:id, IsActive:true }
  $.ajax({
    url: API.setStatus, method:"POST", contentType:"application/json",
    data: JSON.stringify({ ProjectID:id, ProjectStatus:"Active" }),
    success:function(){ alertMessage("Project activated","success"); loadProjects(); },
    error:function(){ alertMessage("Failed to activate","error"); }
  });
});
 
$(document).ready(()=>showSection("listSection"));
// Wire up filter and pagination handlers
$(document).ready(function(){
  // When status filter changes, reload page 1
  $("#statusFilter").on('change', function(){
    const s = $(this).val();
    projCurrentPage = 1;
    loadProjects(projCurrentPage, s);
  });
 
  // Prev / Next
  $("#prevPage").on('click', function(){ if(projCurrentPage > 1) loadProjects(projCurrentPage - 1, projStatusFilter); });
  $("#nextPage").on('click', function(){ loadProjects(projCurrentPage + 1, projStatusFilter); });
 
  // Initialize filter default and load first page
  $("#statusFilter").val(projStatusFilter);
  loadProjects(projCurrentPage, projStatusFilter);
});
 
 
 