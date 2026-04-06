const API_BASE = "https://localhost:7230/api/allocation"; // Adjust if needed
const EMPLOYEES_API = "https://localhost:7230/api/Employee/GetAllEmployees";
const PROJECTS_API = "https://localhost:7230/api/projects/get-all";
const PROJECTS_SEARCH_API = "https://localhost:7230/api/projects/search";

// Map to store project objects keyed by projectID (as string)
const PROJECTS_MAP = new Map();

// -------------------- PERSISTENT ALERT NOTIFICATION --------------------
function showAlert(message, type = 'info', duration = 1500) {
    const alertBox = document.getElementById('alertNotification');
    if (!alertBox) return;
    
    alertBox.textContent = message;
    alertBox.className = type; // Set type class (success, error, warning, info)
    alertBox.style.display = 'flex';
    alertBox.style.animation = 'none'; // Reset animation
    
    // Trigger reflow to restart animation
    void alertBox.offsetWidth;
    
    alertBox.style.animation = `popUp 0.5s ease, popDown 0.5s ease ${duration}ms forwards`;
    
    // Auto-hide after duration
    setTimeout(() => {
        alertBox.style.display = 'none';
    }, duration + 500);
}

// -------------------- HELPER VALIDATION FUNCTIONS --------------------
function isValidDate(dateStr) {
    return !dateStr || !isNaN(new Date(dateStr).getTime());
}

function isStartBeforeEnd(start, end) {
    if (!start || !end) return true; // End is optional
    return new Date(start) <= new Date(end);
}

// -------------------- ASSIGN FLOW (modal) --------------------
// global cache for employees
let EMPLOYEES_LIST = [];

// Function to fetch employees already assigned to a project by project name
async function getAssignedEmployeesForProject(projectName) {
    try {
        const response = await fetch(`https://localhost:7230/api/Allocation/ByProject?projectName=${encodeURIComponent(projectName)}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
        
        if (response.ok) {
            const data = await response.json();
            return Array.isArray(data) ? data.map(emp => emp.employeeID) : [];
        }
    } catch (err) {
        console.error('Failed to fetch assigned employees:', err);
    }
    return [];
}

function openAssignModal(projectId) {
    const modal = document.getElementById('assignModal');
    const assignProjIdInput = document.getElementById('assignProjectId');
    const startInput = document.getElementById('assignStartDate');
    const endInput = document.getElementById('assignEndDate');
    const empSelect = document.getElementById('assignEmployeeSelect');
    const respBox = document.getElementById('assignResponse');

    respBox.innerText = '';
    assignProjIdInput.value = projectId;

    const proj = PROJECTS_MAP.get(String(projectId));
    if (proj) {
        // fill dates from project defaults
        const s = proj.startDate ? proj.startDate.split('T')[0] : '';
        const e = proj.endDate ? proj.endDate.split('T')[0] : '';
        startInput.value = s;
        endInput.value = e;
    }

    // populate employee dropdown, excluding already assigned employees
    if (empSelect) {
        empSelect.innerHTML = '<option value="">-- Select Employee --</option>';
        
        // Get project name for API call
        const projectName = proj?.projectName || '';
        
        if (projectName) {
            // Load assigned employees asynchronously using the Allocation API
            getAssignedEmployeesForProject(projectName).then(assignedIds => {
                const assignedSet = new Set(assignedIds);
                
                EMPLOYEES_LIST.forEach(emp => {
                    // Only show employees not already assigned to this project
                    if (!assignedSet.has(emp.employeeID)) {
                        const opt = document.createElement('option');
                        opt.value = emp.employeeID;
                        opt.textContent = emp.fullName;
                        empSelect.appendChild(opt);
                    }
                });
                
                // Show message if all employees are already assigned
                if (empSelect.querySelectorAll('option').length === 1) {
                    showAlert('All employees are already assigned to this project', 'info');
                }
            });
        } else {
            // Fallback: show all employees if no project name
            EMPLOYEES_LIST.forEach(emp => {
                const opt = document.createElement('option');
                opt.value = emp.employeeID;
                opt.textContent = emp.fullName;
                empSelect.appendChild(opt);
            });
        }
    }

    if (modal) modal.style.display = 'block';
}

async function submitAssignFromModal() {
    const projId = document.getElementById('assignProjectId').value;
    const empSel = document.getElementById('assignEmployeeSelect');
    const startDate = document.getElementById('assignStartDate').value;
    const endDate = document.getElementById('assignEndDate').value;
    const respBox = document.getElementById('assignResponse');

    respBox.innerText = '';

    if (!projId) { showAlert('Project not selected', 'warning'); return; }
    if (!empSel || !empSel.value) { showAlert('Please select an employee', 'warning'); return; }
    if (!startDate || !isValidDate(startDate)) { showAlert('Start date is required/invalid', 'warning'); return; }
    if (!endDate || !isValidDate(endDate)) { showAlert('End date is required/invalid', 'warning'); return; }
    if (!isStartBeforeEnd(startDate, endDate)) { showAlert('End date cannot be before start date', 'error'); return; }

    // Validate end date relative to project end date: do not allow allocation to end after project end date
    const proj = PROJECTS_MAP.get(String(projId));
    if (proj && proj.endDate) {
        const projEnd = proj.endDate.split('T')[0];
        if (new Date(endDate) > new Date(projEnd)) {
            showAlert('Allocation end date cannot be after the project end date (' + projEnd + ')', 'error');
            return;
        }
    }

    const selectedEmployeeName = empSel.options[empSel.selectedIndex]?.text?.trim() || '';

    const payload = {
        EmployeeID: Number(empSel.value),
        EmployeeName: selectedEmployeeName,
        ProjectID: Number(projId),
        ProjectName: proj?.projectName || '',
        AllocationStartDate: startDate,
        AllocationEndDate: endDate || null
    };

    try {
        const response = await fetch(`${API_BASE}/AssignProject`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await response.json();
        showAlert(data.message || 'Employee assigned successfully!', 'success');

        // close modal after short delay and refresh current page
        setTimeout(() => {
            const modal = document.getElementById('assignModal');
            if (modal) modal.style.display = 'none';
            loadProjectsPage(currentPage);
        }, 900);
    } catch (error) {
        showAlert('Error: ' + error.message, 'error');
    }
}

// -------------------- LOAD EMPLOYEES & PROJECTS --------------------
async function loadEmployeesAndProjects() {
    const empSel = document.getElementById('employeeSelect');
    const projSel = document.getElementById('projectSelect');
    const projSearchSel = document.getElementById('projectSearchSelect');

    try {
        // Employees
        const empResp = await fetch(EMPLOYEES_API, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({})
        });

        if (empResp.ok) {
            const employees = await empResp.json();
            if (Array.isArray(employees)) {
                EMPLOYEES_LIST = employees.slice();
                const histSel = document.getElementById('employeeHistorySelect');
                const assignSel = document.getElementById('assignEmployeeSelect');
                employees.forEach(emp => {
                    const opt = document.createElement('option');
                    opt.value = emp.employeeID;
                    opt.textContent = emp.fullName;
                    if (empSel) empSel.appendChild(opt);

                    if (histSel) {
                        const hOpt = document.createElement('option');
                        hOpt.value = emp.employeeID;
                        hOpt.textContent = emp.fullName;
                        histSel.appendChild(hOpt);
                    }

                    if (assignSel) {
                        const aOpt = document.createElement('option');
                        aOpt.value = emp.employeeID;
                        aOpt.textContent = emp.fullName;
                        assignSel.appendChild(aOpt);
                    }
                });
            }
        }
    } catch (err) {
        console.error('Failed to load employees', err);
    }

    try {
        // Projects
        const projResp = await fetch(PROJECTS_API, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({})
        });

        if (projResp.ok) {
            const projData = await projResp.json();
            const list = projData?.working_days || [];
            list.forEach(p => {
                try { PROJECTS_MAP.set(String(p.projectID), p); } catch (e) { /* ignore */ }

                // Also add to search dropdown if present
                if (projSearchSel) {
                    const searchOpt = document.createElement('option');
                    searchOpt.value = p.projectName;
                    searchOpt.textContent = p.projectName;
                    projSearchSel.appendChild(searchOpt);
                }
            });
        }
    } catch (err) {
        console.error('Failed to load projects', err);
    }
}

// Load dropdowns once page is ready
// Pagination state
let currentPage = 1;
const pageSize = 4; // default page size

// Load dropdowns and initial project page once DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    loadEmployeesAndProjects();
    setupPaginationControls();
    loadProjectsPage(1);
    setupNavModals();
});

// -------------------- PAGINATION & PROJECT LIST --------------------
async function loadProjectsPage(page) {
    currentPage = page || 1;
    const projectsList = document.getElementById('projectsList');
    const pageInfo = document.getElementById('pageInfo');
    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');

    if (!projectsList || !pageInfo || !prevBtn || !nextBtn) return;

    pageInfo.textContent = `Page ${currentPage}`;
    projectsList.innerHTML = 'Loading...';

    try {
        const resp = await fetch(PROJECTS_SEARCH_API, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status: 'Active', page: currentPage, pageSize: pageSize })
        });

        if (!resp.ok) {
            projectsList.innerHTML = 'Failed to load projects';
            return;
        }

        const json = await resp.json();
        const list = json?.data || [];

        // store projects in map for other lookups
        list.forEach(p => { try { PROJECTS_MAP.set(String(p.projectID), p); } catch (e) { } });

        renderProjects(list);

        // prev button enabled if page > 1
        prevBtn.disabled = currentPage <= 1;

        // if returned items less than pageSize, assume last page and disable next
        nextBtn.disabled = !Array.isArray(list) || list.length < pageSize;
    } catch (err) {
        console.error('Failed to load paged projects', err);
        projectsList.innerHTML = 'Error loading projects';
    }
}

function renderProjects(list) {
    const projectsList = document.getElementById('projectsList');
    if (!projectsList) return;

    if (!Array.isArray(list) || list.length === 0) {
        projectsList.innerHTML = '<p>No projects found.</p>';
        return;
    }

    // render as a table with one project per row
    projectsList.innerHTML = '';
    const table = document.createElement('table');
    table.className = 'projects-table';
    table.innerHTML = `
        <thead>
            <tr>
                <th>ID</th>
                <th>Project</th>
                <th>Client</th>
                <th>Assigned / Capacity</th>
                <th>Action</th>
            </tr>
        </thead>
        <tbody></tbody>
    `;

    list.forEach(p => {
        const assigned = typeof p.assigned === 'number' ? p.assigned : 0;
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${p.projectID}</td>
            <td><div class="project-title">${p.projectName}</div></td>
            <td>${p.clientName || ''}</td>
            <td>${assigned} / ${p.capacity}</td>
            <td></td>
        `;

        const actionTd = tr.querySelector('td:last-child');
        if (assigned < p.capacity) {
            const btn = document.createElement('button');
            btn.textContent = 'Assign Employee';
            btn.addEventListener('click', () => openAssignModal(p.projectID));
            actionTd.appendChild(btn);
        } else {
            const span = document.createElement('span');
            span.className = 'full-capacity';
            span.textContent = 'Full';
            actionTd.appendChild(span);
        }

        table.querySelector('tbody').appendChild(tr);
    });

    projectsList.appendChild(table);
}

function setupPaginationControls() {
    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');
    if (prevBtn) prevBtn.addEventListener('click', () => { if (currentPage > 1) loadProjectsPage(currentPage - 1); });
    if (nextBtn) nextBtn.addEventListener('click', () => { loadProjectsPage(currentPage + 1); });
}

// -------------------- NAV / MODAL HANDLERS --------------------
function setupNavModals() {
    const navHistory = document.getElementById('navHistory');
    const navEmployeeAlloc = document.getElementById('navEmployeeAlloc');
    const historyModal = document.getElementById('historyModal');
    const assignModal = document.getElementById('assignModal');
    const employeeAllocModal = document.getElementById('employeeAllocModal');
    const closeHistory = document.getElementById('closeHistory');
    const closeAssign = document.getElementById('closeAssign');
    const closeEmployeeAlloc = document.getElementById('closeEmployeeAlloc');
    const assignSubmitBtn = document.getElementById('assignSubmit');
    const backHistory = document.getElementById('backHistory');
    const backAssign = document.getElementById('backAssign');
    const backEmployeeAlloc = document.getElementById('backEmployeeAlloc');

    if (navHistory && historyModal) {
        navHistory.addEventListener('click', (e) => { e.preventDefault(); historyModal.style.display = 'block'; });
    }
    if (navEmployeeAlloc && employeeAllocModal) {
        navEmployeeAlloc.addEventListener('click', (e) => { e.preventDefault(); employeeAllocModal.style.display = 'block'; });
    }
    if (closeHistory && historyModal) closeHistory.addEventListener('click', () => historyModal.style.display = 'none');
    if (closeAssign && assignModal) closeAssign.addEventListener('click', () => assignModal.style.display = 'none');
    if (closeEmployeeAlloc && employeeAllocModal) closeEmployeeAlloc.addEventListener('click', () => employeeAllocModal.style.display = 'none');
    
    // Back button handlers
    if (backHistory && historyModal) backHistory.addEventListener('click', () => historyModal.style.display = 'none');
    if (backAssign && assignModal) backAssign.addEventListener('click', () => assignModal.style.display = 'none');
    if (backEmployeeAlloc && employeeAllocModal) backEmployeeAlloc.addEventListener('click', () => employeeAllocModal.style.display = 'none');
    
    if (assignSubmitBtn) assignSubmitBtn.addEventListener('click', (e) => { e.preventDefault(); submitAssignFromModal(); });

    // close on outside click
    window.addEventListener('click', (e) => {
        if (e.target === historyModal) historyModal.style.display = 'none';
        if (e.target === assignModal) assignModal.style.display = 'none';
        if (e.target === employeeAllocModal) employeeAllocModal.style.display = 'none';
    });
}

// -------------------- VIEW ALLOCATION HISTORY --------------------
async function viewHistory() {
    const histSelect = document.getElementById("employeeHistorySelect");
    const employeeId = histSelect?.value ? parseInt(histSelect.value) : NaN;

    if (!employeeId) { showAlert("Please select an employee", 'warning'); return; }

    try {
        const response = await fetch(`${API_BASE}/history/${employeeId}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({})
        });

        if (!response.ok) {
            const err = await response.json();
            showAlert("Error: " + err.message, 'error');
            return;
        }

        const data = await response.json();
        const tbody = document.querySelector("#historyTable tbody");
        tbody.innerHTML = "";

        if (!Array.isArray(data) || data.length === 0) {
            showAlert("No allocation history found", 'info');
            document.getElementById("historyTable").style.display = "none";
            return;
        }

        data.forEach(row => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${row.allocationID}</td>
                <td>${row.projectName}</td>
                <td>${row.allocationStartDate}</td>
                <td>${row.allocationEndDate || '-'}</td>
                <td>${row.allocationPercentage}</td>
            `;
            tbody.appendChild(tr);
        });

        document.getElementById("historyTable").style.display = "table";
        showAlert(`Found ${data.length} allocation(s)`, 'success');
    } catch (err) {
        console.error(err);
        showAlert("Request failed: " + err.message, 'error');
    }
}

// -------------------- SEARCH EMPLOYEES BY PROJECT --------------------
$(document).ready(function () {
    $("#searchBtn").click(function () {
        let projectName = $("#projectSearchSelect").val().trim();

        if (projectName === "") {
            showAlert("Please select a project.", 'warning');
            return;
        }

        $.ajax({
            url: "https://localhost:7230/api/Allocation/ByProject?projectName=" + encodeURIComponent(projectName),
            method: "POST", // Ensure this matches your backend GET route
            success: function (data) {
                $("#resultTable tbody").empty();

                if (!data || data.length === 0) {
                    showAlert("No employees found for this project.", 'info');
                    $("#resultTable").hide();
                    return;
                }

                data.forEach(emp => {
                    $("#resultTable tbody").append(`
                        <tr>
                            <td>${emp.employeeID}</td>
                            <td>${emp.fullName}</td>
                            <td>${emp.email}</td>
                            <td>${emp.designation}</td>
                        </tr>
                    `);
                });

                showAlert(`Found ${data.length} employee(s) for this project`, 'success');
                $("#resultTable").show();
            },
            error: function (xhr) {
                showAlert("Error retrieving employees: " + xhr.responseText, 'error');
            }
        });
    });
});
