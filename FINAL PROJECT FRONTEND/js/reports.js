// =======================
// Section Switching Logic
// =======================
document.querySelectorAll(".menu button").forEach(btn => {
    btn.addEventListener("click", () => {
        const sectionID = btn.getAttribute("data-section");

        // Hide all
        document.querySelectorAll(".section").forEach(sec => sec.style.display = "none");

        // Show selected (use grid so section's grid layout takes effect)
        document.getElementById(sectionID).style.display = "grid";

        // Load charts for that section
        loadSection(sectionID);
    });
});

// =======================
// Section Loader
// =======================
function loadSection(sectionID) {
    switch(sectionID) {
        case "employeesSection":
            loadEmployeeCharts();
            loadExtraEmployeeCharts(); 
            break;
        case "projectsSection":
            loadProjectCharts();
            break;
        case "allocationSection":
            loadAllocationCharts();
            break;
        case "benchSection":
            loadBenchCharts();
            break;
        case "skillsSection":
            loadSkillCharts();
            break;
    }
}

// =======================
// Placeholder Chart Loaders
// =======================
let employeeChart, projectChart, allocationChart, benchChart, skillsChart;

function loadEmployeeCharts() {
    const ctx = document.getElementById("employeeChart").getContext("2d");

    if (employeeChart) employeeChart.destroy();

    employeeChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels: ["Loading..."],
            datasets: [{ data: [1], backgroundColor: "#4caf50" }]
        }
    });
}

function loadProjectCharts() {
    const ctx = document.getElementById("projectChart").getContext("2d");

    if (projectChart) projectChart.destroy();

    projectChart = new Chart(ctx, {
        type: "line",
        data: {
            labels: ["Loading..."],
            datasets: [{ data: [1], borderColor: "#2196f3" }]
        }
    });
}

function loadAllocationCharts() {
    const ctx = document.getElementById("allocationChart").getContext("2d");

    if (allocationChart) allocationChart.destroy();

    allocationChart = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ["Loading"],
            datasets: [{ data: [100], backgroundColor: ["#ff9800"] }]
        }
    });
}

function loadBenchCharts() {
    const ctx = document.getElementById("benchChart").getContext("2d");

    if (benchChart) benchChart.destroy();

    benchChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels: ["Loading"],
            datasets: [{ data: [1], backgroundColor: "#f44336" }]
        }
    });
}

function loadSkillCharts() {
    const ctx = document.getElementById("skillsChart").getContext("2d");

    if (skillsChart) skillsChart.destroy();

    skillsChart = new Chart(ctx, {
        type: "pie",
        data: {
            labels: ["Loading"],
            datasets: [{ data: [1], backgroundColor: ["#9c27b0"] }]
        }
    });
}
// projects
// ========================
// API ENDPOINTS
// ========================
const API = {
    projects: "https://localhost:7230/api/projects/get-all",
    overlap: "https://localhost:7230/api/Report/project-overlap",
    utilization: "https://localhost:7230/api/Report/project-utilization",
    projectAllocation: p => `https://localhost:7230/api/Allocation/ByProject?projectName=${encodeURIComponent(p)}`
};

// ========================
// GLOBAL CHART OBJECTS
// ========================
let capUtilChart, statusChart, overlapChart, timelineChart, projectAllocationChart, roleChart;



// ========================
// MAIN LOADER
// ========================
async function loadProjectCharts() {
    const projects = await fetchProjects();
    const utilization = await fetchUtilization();
    const overlap = await fetchOverlap();

    populateProjectDropdown(projects);
    buildCapUtilChart(projects, utilization);
    buildStatusChart(projects);
    buildOverlapChart(overlap);
    buildTimelineChart(projects);
}



// ========================
// FETCHERS
// ========================
async function fetchProjects() {
    const res = await fetch(API.projects, { method: "POST" });
    const data = await res.json();
    return data.working_days || [];
}

async function fetchUtilization() {
    const res = await fetch(API.utilization, { method: "POST" });
    return await res.json();
}

async function fetchOverlap() {
    const res = await fetch(API.overlap, { method: "POST" });
    return await res.json();
}

async function fetchAllocByProject(projectName) {
    const res = await fetch(API.projectAllocation(projectName), { method: "POST" });
    return await res.json();
}



// ========================
//  CAPACITY vs UTILIZATION
// ========================
function buildCapUtilChart(projects, utilization) {
    const labels = projects.map(p => p.projectName);
    const capacity = projects.map(p => p.capacity);

    const utilMap = {};
    utilization.forEach(u => utilMap[u.projectName] = u.utilization);

    const utilValues = labels.map(name => utilMap[name] || 0);

    const ctx = document.getElementById("capUtilChart");

    if (capUtilChart) capUtilChart.destroy();

    capUtilChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets: [
                { label: "Capacity", data: capacity, backgroundColor: "#66bb6a" },
                { label: "Utilization %", data: utilValues, backgroundColor: "#42a5f5" }
            ]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });
}



// ========================
//  ACTIVE vs INACTIVE PROJECTS
// ========================
function buildStatusChart(projects) {
    const active = projects.filter(p => p.projectStatus === "Active").length;
    const inactive = projects.length - active;

    const ctx = document.getElementById("statusChart");

    if (statusChart) statusChart.destroy();

    statusChart = new Chart(ctx, {
        type: "pie",
        data: {
            labels: ["Active", "Inactive"],
            datasets: [{
                data: [active, inactive],
                backgroundColor: ["#2e7d32", "#c62828"]
            }]
        }
    });
}






// ========================
// EMPLOYEE ALLOCATION + ROLE DISTRIBUTION
// ========================
async function loadProjectAllocation(projectName) {
    const data = await fetchAllocByProject(projectName);

    const labels = data.map(x => x.fullName);
    const values = data.map(x => x.allocationPercentage);

    const ctx = document.getElementById("projectAllocationChart");
    if (projectAllocationChart) projectAllocationChart.destroy();

    projectAllocationChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Allocation %",
                data: values,
                backgroundColor: "#7e57c2"
            }]
        }
    });
}
async function loadRoleDistribution(projectName) {
    let data = await fetchAllocByProject(projectName);
    const canvas = document.getElementById("roleChart");
    const container = canvas.parentElement;

    // Remove old message
    const oldMsg = document.getElementById("noRoleMsg");
    if (oldMsg) oldMsg.remove();

    // ===========================
    //  Detect NO allocations
    // ===========================
    if (!Array.isArray(data) || data.message) {

        if (roleChart) {
            roleChart.destroy();
            roleChart = null;
        }

        canvas.style.display = "none";

        const msg = document.createElement("p");
        msg.id = "noRoleMsg";
        msg.textContent = "No allocations yet for this project";
        msg.style.color = "#b71c1c";
        msg.style.fontWeight = "bold";
        msg.style.padding = "10px 0";
        msg.style.fontSize = "14px";

        container.appendChild(msg);
        return;
    }

    // ===========================
    //  Build role counts
    // ===========================
    const roleCount = {};

    data.forEach(x => {
        if (x.designation && x.designation.trim() !== "") {
            roleCount[x.designation] = (roleCount[x.designation] || 0) + 1;
        }
    });

    // ===========================
    //  If all roles null → treat as no data
    // ===========================
    if (Object.keys(roleCount).length === 0) {

        if (roleChart) {
            roleChart.destroy();
            roleChart = null;
        }

        canvas.style.display = "none";

        const msg = document.createElement("p");
        msg.id = "noRoleMsg";
        msg.textContent = "No role/designation data found for this project";
        msg.style.color = "#b71c1c";
        msg.style.fontWeight = "bold";
        msg.style.padding = "10px 0";
        msg.style.fontSize = "14px";

        container.appendChild(msg);
        return;
    }

    // ===========================
    //  VALID DATA → SHOW CHART
    // ===========================
    canvas.style.display = "block";

    if (roleChart) roleChart.destroy();

    const labels = Object.keys(roleCount);
    const values = Object.values(roleCount);
    const ctx = canvas;

    roleChart = new Chart(ctx, {
        type: "pie",
        data: {
            labels,
            datasets: [{
                data: values,
                backgroundColor: labels.map((_, i) =>
                    `hsl(${i * 60}, 75%, 55%)`
                )
            }]
        }
    });
}

async function populateProjectDropdown(projects) {
    const dropdown1 = document.getElementById("projectDropdown");
    const dropdown2 = document.getElementById("roleProjectDropdown");

    dropdown1.innerHTML = "";
    dropdown2.innerHTML = "";

    projects.forEach(p => {
        const opt1 = document.createElement("option");
        opt1.value = p.projectName;
        opt1.textContent = p.projectName;

        const opt2 = opt1.cloneNode(true);

        dropdown1.appendChild(opt1);
        dropdown2.appendChild(opt2);
    });

    dropdown1.addEventListener("change", () =>
        loadProjectAllocation(dropdown1.value)
    );

    dropdown2.addEventListener("change", () =>
        loadRoleDistribution(dropdown2.value)
    );

    // Auto-load both charts for first project
    if (projects.length) {
        loadProjectAllocation(projects[0].projectName);
        loadRoleDistribution(projects[0].projectName);
    }
}

// projects 

// employees

let deptChart, genderChart, expChart, allocChart, skillChart;

async function loadEmployeeCharts() {
    const employees = await fetchEmployees();
    const skillAvailability = await fetchSkillAvailability();

    // ============================
    //  Employee Count by Designation
    // ============================
    const designationCount = {};
    employees.forEach(e => {
        const d = e.designation || "Unknown";
        designationCount[d] = (designationCount[d] || 0) + 1;
    });

    const ctx1 = document.getElementById("employeeChart").getContext("2d");
    if (deptChart) deptChart.destroy();
    deptChart = new Chart(ctx1, {
        type: "bar",
        data: {
            labels: Object.keys(designationCount),
            datasets: [{
                label: "Employees",
                data: Object.values(designationCount),
                backgroundColor: "#4caf50"
            }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });

    // ============================
    //  Gender Distribution
    // ============================
    const genderCount = {};
    employees.forEach(e => {
        const g = e.gender || "Other";
        genderCount[g] = (genderCount[g] || 0) + 1;
    });

    const genderCtx = document.getElementById("genderChart").getContext("2d");
    if (genderChart) genderChart.destroy();
    genderChart = new Chart(genderCtx, {
        type: "pie",
        data: {
            labels: Object.keys(genderCount),
            datasets: [{
                data: Object.values(genderCount),
                backgroundColor: ["#2196f3","#ff4081","#ffeb3b"]
            }]
        }
    });

    // ============================
    //  Experience Distribution
    // ============================
    const expBuckets = { "0-1": 0, "1-3": 0, "3-5": 0, "5+": 0 };
    employees.forEach(e => {
        const exp = e.experienceInYears || 0;
        if (exp <= 1) expBuckets["0-1"]++;
        else if (exp <= 3) expBuckets["1-3"]++;
        else if (exp <= 5) expBuckets["3-5"]++;
        else expBuckets["5+"]++;
    });

    const expCtx = document.getElementById("expChart").getContext("2d");
    if (expChart) expChart.destroy();
    expChart = new Chart(expCtx, {
        type: "bar",
        data: {
            labels: Object.keys(expBuckets),
            datasets: [{
                label: "Employees",
                data: Object.values(expBuckets),
                backgroundColor: "#ff9800"
            }]
        }
    });

    // ============================
    //  Employee Allocation Status
    // ============================
    let allocated = 0, unallocated = 0;

    // Fetch each employee's history
    await Promise.all(employees.map(async e => {
        const history = await fetchEmployeeHistory(e.employeeID);
        if (Array.isArray(history) && history.length > 0) allocated++;
        else unallocated++;
    }));

    const allocCtx = document.getElementById("allocChart").getContext("2d");
    if (allocChart) allocChart.destroy();
    allocChart = new Chart(allocCtx, {
        type: "doughnut",
        data: {
            labels: ["Allocated", "Unallocated"],
            datasets: [{
                data: [allocated, unallocated],
                backgroundColor: ["#66bb6a","#ef5350"]
            }]
        }
    });

    // ============================
    //  Skill Availability
    // ============================
    const skillCtx = document.getElementById("skillChart").getContext("2d");
    if (skillChart) skillChart.destroy();
    skillChart = new Chart(skillCtx, {
        type: "bar",
        data: {
            labels: skillAvailability.map(s => s.skillName),
            datasets: [{
                label: "Available Employees",
                data: skillAvailability.map(s => s.availableCount),
                backgroundColor: "#42a5f5"
            }]
        }
    });
}

// ============================
// FETCH FUNCTIONS
// ============================
async function fetchEmployees() {
    const res = await fetch("https://localhost:7230/api/Employee/GetAllEmployees", { method: "POST" });
    return await res.json();
}

async function fetchEmployeeHistory(employeeId) {
    const res = await fetch(`https://localhost:7230/api/Report/employee-history/${employeeId}`, { method: "POST" });
    return await res.json();
}

async function fetchSkillAvailability() {
    const res = await fetch("https://localhost:7230/api/Report/skill-availability", { method: "POST" });
    return await res.json();
}

let designationTimeChart, genderByDesignationChart, skillHeatmapChart;

async function loadExtraEmployeeCharts() {
    const employees = await fetchEmployees();

    // ============================
    //  Top 3 Designations Over Time
    // ============================
    const joinData = {}; // {designation: {YYYY-MM: count}}
    employees.forEach(e => {
        const doj = new Date(e.dateOfJoining);
        const month = `${doj.getFullYear()}-${String(doj.getMonth() + 1).padStart(2,'0')}`;
        const desig = e.designation || "Unknown";

        if (!joinData[desig]) joinData[desig] = {};
        joinData[desig][month] = (joinData[desig][month] || 0) + 1;
    });

    // Pick top 3 designations
    const desigCounts = Object.keys(joinData).map(d => ({
        designation: d,
        total: Object.values(joinData[d]).reduce((a,b)=>a+b,0)
    })).sort((a,b)=>b.total-a.total).slice(0,3);

    const months = [...new Set(employees.map(e => {
        const d = new Date(e.dateOfJoining);
        return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}`;
    }))].sort();

    const datasets = desigCounts.map((d,i)=>({
        label: d.designation,
        data: months.map(m => joinData[d.designation][m] || 0),
        borderColor: `hsl(${i*100},70%,50%)`,
        fill: false
    }));

    const ctx1 = document.getElementById("designationTimeChart").getContext("2d");
    if (designationTimeChart) designationTimeChart.destroy();
    designationTimeChart = new Chart(ctx1, {
        type: "line",
        data: { labels: months, datasets },
        options: { responsive: true, maintainAspectRatio: false }
    });

    // ============================
    // 2️ Gender by Designation (Stacked Bar)
    // ============================
    const desigs = [...new Set(employees.map(e=>e.designation || "Unknown"))];
    const genders = ["M","F","O"];
    const genderData = genders.map(g => desigs.map(d => 
        employees.filter(e=> (e.designation||"Unknown")===d && (e.gender||"Other")===g).length
    ));

    const ctx2 = document.getElementById("genderByDesignationChart").getContext("2d");
    if (genderByDesignationChart) genderByDesignationChart.destroy();
    genderByDesignationChart = new Chart(ctx2, {
        type: "bar",
        data: {
            labels: desigs,
            datasets: genders.map((g,i)=>({
                label: g,
                data: genderData[i],
                backgroundColor: i===0?"#2196f3":i===1?"#ff4081":"#ffeb3b"
            }))
        },
        options: { responsive: true, maintainAspectRatio: false, scales: { x: { stacked: true }, y: { stacked: true } } }
    });


    
}

// employees 

//bench

// ============================
// ADDITIONAL BENCH CHARTS
// ============================

// Global chart objects
let benchSkillEmployeeChart,
    benchPercentageChart,
    benchBySkillChart,
    projectBenchChart,
    projectTimelineChart,
    benchExperienceChart,
    employeeSkillCountChart;

// ============================
// LOAD BENCH CHARTS
// ============================
async function loadBenchCharts() {
    const benchData = await fetchBenchEmployees();
    const benchPercData = await fetchBenchPercentage();
    const benchBySkillData = await fetchBenchBySkill();
    const capacitySummary = await fetchCapacitySummary();
    const activeProjects = await fetchActiveProjectDates();

    // 1 Bench Employees by Skill (Stacked Bar)
    buildBenchSkillEmployeeChart(benchData);

    //  Bench Percentage vs Assigned (Doughnut)
    buildBenchPercentageChart(benchPercData);

    //  Bench Count by Skill (Horizontal Bar)
    buildBenchBySkillChart(benchBySkillData);

    //  Project Assignment vs Bench (Stacked Bar)
    buildProjectBenchChart(capacitySummary);

    //  Active Project Timelines (Gantt-style)
    buildProjectTimelineChart(activeProjects);

    //  Bench Experience Distribution (Histogram)
    buildBenchExperienceChart(benchData);

    //  Employee Skill Count
    buildEmployeeSkillCountChart(benchData);
}

// ============================
// FETCH FUNCTIONS
// ============================
async function fetchBenchEmployees() {
    const res = await fetch("https://localhost:7230/api/BenchManagement/bench-employees");
    return await res.json();
}

async function fetchBenchPercentage() {
    const res = await fetch("https://localhost:7230/api/BenchManagement/bench-percentage");
    const data = await res.json();
    return data.benchPercentage || 0;
}

async function fetchBenchBySkill() {
    const res = await fetch("https://localhost:7230/api/BenchManagement/bench-by-skill");
    return await res.json();
}

async function fetchCapacitySummary() {
    const res = await fetch("https://localhost:7230/api/BenchManagement/capacity-summary");
    return await res.json();
}

async function fetchActiveProjectDates() {
    const res = await fetch("https://localhost:7230/api/BenchManagement/active-project-dates");
    return await res.json();
}

// ============================
// BUILD FUNCTIONS
// ============================

//  Bench Employees by Skill (Stacked Bar)
function buildBenchSkillEmployeeChart(data) {
    const employees = [...new Set(data.map(d=>d.employeeName))];
    const skills = [...new Set(data.map(d=>d.skill))];

    const datasets = skills.map((skill,i)=>({
        label: skill,
        data: employees.map(emp=>data.filter(d=>d.employeeName===emp && d.skill===skill).length),
        backgroundColor: `hsl(${i*60},70%,50%)`
    }));

    const ctx = document.getElementById("benchSkillEmployeeChart").getContext("2d");
    if (benchSkillEmployeeChart) benchSkillEmployeeChart.destroy();

    benchSkillEmployeeChart = new Chart(ctx, {
        type: "bar",
        data: { labels: employees, datasets },
        options: { responsive:true, maintainAspectRatio:false, scales:{ x:{ stacked:true }, y:{ stacked:true } } }
    });
}

//  Bench Percentage vs Assigned (Doughnut)
function buildBenchPercentageChart(benchPerc) {
    const ctx = document.getElementById("benchPercentageChart").getContext("2d");
    if (benchPercentageChart) benchPercentageChart.destroy();

    benchPercentageChart = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ["Bench", "Assigned"],
            datasets:[{ data:[benchPerc, 100-benchPerc], backgroundColor:["#f44336","#4caf50"] }]
        }
    });
}

//  Bench Count by Skill (Horizontal Bar)
function buildBenchBySkillChart(data) {
    const ctx = document.getElementById("benchBySkillChart").getContext("2d");
    if (benchBySkillChart) benchBySkillChart.destroy();

    benchBySkillChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels: data.map(d=>d.skillName),
            datasets: [{ label: "Bench Count", data: data.map(d=>d.benchCount), backgroundColor:"#42a5f5" }]
        },
        options: { indexAxis: 'y', responsive:true, maintainAspectRatio:false }
    });
}

//  Project Assignment vs Bench (Stacked Bar)
function buildProjectBenchChart(data) {
    const labels = data.projects.map(p=>p.projectName);
    const assigned = data.projects.map(p=>p.assigned);
    const remaining = data.projects.map(p=>p.capacity - p.assigned);

    const ctx = document.getElementById("projectBenchChart").getContext("2d");
    if (projectBenchChart) projectBenchChart.destroy();

    projectBenchChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets:[
                { label:"Assigned", data:assigned, backgroundColor:"#4caf50" },
                { label:"Available Capacity", data:remaining, backgroundColor:"#ff9800" }
            ]
        },
        options: { responsive:true, maintainAspectRatio:false, scales:{ x:{ stacked:true }, y:{ stacked:true } } }
    });
}

//  Active Project Timelines (Gantt-style)
function buildProjectTimelineChart(data) {
    const labels = data.projects.map(p=>p.projectName);
    const startDates = data.projects.map(p=>new Date(p.startDate).getTime());
    const endDates = data.projects.map(p=>new Date(p.endDate).getTime());
    const durations = endDates.map((e,i)=> (e-startDates[i])/(1000*60*60*24)); // days
    const ctx = document.getElementById("projectTimelineChart").getContext("2d");
    if (projectTimelineChart) projectTimelineChart.destroy();

    projectTimelineChart = new Chart(ctx, {
        type: "bar",
        data: {
            labels,
            datasets:[{
                label:"Project Duration (days)",
                data: durations,
                backgroundColor:"#2196f3"
            }]
        },
        options:{ indexAxis:'y', responsive:true, maintainAspectRatio:false }
    });
}

// Bench Experience Distribution (Histogram)
function buildBenchExperienceChart(data) {
    const buckets = {"0-2":0,"3-5":0,"6+":0};
    data.forEach(d=>{
        const exp = d.experience || 0;
        if(exp<=2) buckets["0-2"]++;
        else if(exp<=5) buckets["3-5"]++;
        else buckets["6+"]++;
    });

    const ctx = document.getElementById("benchExperienceChart").getContext("2d");
    if (benchExperienceChart) benchExperienceChart.destroy();

    benchExperienceChart = new Chart(ctx,{
        type:"bar",
        data:{ labels:Object.keys(buckets), datasets:[{ label:"Employees", data:Object.values(buckets), backgroundColor:"#ff9800" }] },
        options:{ responsive:true, maintainAspectRatio:false }
    });
}

// Employee Skill Count
function buildEmployeeSkillCountChart(data) {
    const employees = [...new Set(data.map(d=>d.employeeName))];
    const counts = employees.map(emp => data.filter(d=>d.employeeName===emp).length);

    const ctx = document.getElementById("employeeSkillCountChart").getContext("2d");
    if (employeeSkillCountChart) employeeSkillCountChart.destroy();

    employeeSkillCountChart = new Chart(ctx,{
        type:"bar",
        data:{ labels:employees, datasets:[{ label:"Skill Count", data:counts, backgroundColor:"#9c27b0" }] },
        options:{ responsive:true, maintainAspectRatio:false }
    });
}


//bench


// skills charts
let skillsChart2, skillExperienceChart, employeeSkillCountChart2;

async function loadSkillCharts() {
    // Fetch employees and skill list
    const [employeesRes, skillsRes] = await Promise.all([
        fetch("https://localhost:7230/api/Employee/GetAllEmployees", { method: "POST" }),
        fetch("https://localhost:7230/api/Skill/list", { method: "POST" })
    ]);

    const employees = await employeesRes.json();
    const skills = await skillsRes.json();

    const skillNames = skills.map(s => s.skillName);

    //  Skill Popularity Bar
    const skillCount = {};
    skillNames.forEach(skill => skillCount[skill] = 0);

    employees.forEach(emp => {
        emp.skills.forEach(skill => {
            if (skillCount[skill] !== undefined) skillCount[skill]++;
        });
    });

    const ctx1 = document.getElementById("skillsChart").getContext("2d");

    if (skillsChart2) skillsChart2.destroy();

    skillsChart2 = new Chart(ctx1, {
        type: "bar",
        data: {
            labels: Object.keys(skillCount),
            datasets: [{
                label: "Number of Employees",
                data: Object.values(skillCount),
                backgroundColor: "#42a5f5"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: "Skill Popularity" } }
        }
    });

    //  Skill Experience vs Employee Count (Bubble Chart)
  
// Map skill names to numeric indices
const skillIndexMap = {};
skillNames.forEach((skill, index) => {
    skillIndexMap[skill] = index;
});

// Prepare data points using indices for x
const skillData = skillNames.map(skill => {
    const users = employees.filter(e => e.skills.includes(skill));
    const avgExp = users.reduce((a,b) => a + (b.experienceInYears || 0), 0) / (users.length || 1);
    return {
        x: skillIndexMap[skill],  // numeric index for x
        y: avgExp,
        r: Math.sqrt(users.length) * 5 // bubble radius
    };
});

const ctx2 = document.getElementById("skillDepthCoverageChart").getContext("2d");

if (skillExperienceChart) skillExperienceChart.destroy();

skillExperienceChart = new Chart(ctx2, {
    type: "bubble",
    data: {
        datasets: [{
            label: "Skill Depth vs Coverage",
            data: skillData,
            backgroundColor: "#ff9800"
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            x: {
                title: { display: true, text: "Skills" },
                ticks: {
                    callback: function(value) {
                        // Show skill name for each numeric index
                        return skillNames[value] || '';
                    },
                    autoSkip: false,
                    maxRotation: 90,
                    minRotation: 45,
                },
                min: 0,
                max: skillNames.length - 1
            },
            y: {
                title: { display: true, text: "Average Experience (Years)" },
                beginAtZero: true
            }
        }
    }
});


    //  Employee Skill Count Distribution
    const skillCountDist = {};
    employees.forEach(e => {
        const count = e.skills.length;
        skillCountDist[count] = (skillCountDist[count] || 0) + 1;
    });

    const ctx3 = document.getElementById("employeeSkillCountDistributionChart").getContext("2d");

    if (employeeSkillCountChart2) employeeSkillCountChart2.destroy();

    employeeSkillCountChart2 = new Chart(ctx3, {
        type: "bar",
        data: {
            labels: Object.keys(skillCountDist),
            datasets: [{
                label: "Number of Employees",
                data: Object.values(skillCountDist),
                backgroundColor: "#7e57c2"
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { title: { display: true, text: "Employee Skill Count Distribution" } },
            scales: { x: { title: { display: true, text: "Number of Skills per Employee" } } }
        }
    });
}


//skills

// Automatically open Projects section and load its charts when reports.html is opened
document.addEventListener('DOMContentLoaded', () => {
    const projectsSection = document.getElementById('employeesSection');
    if (projectsSection) {
        // hide any other sections
        document.querySelectorAll('.section').forEach(sec => sec.style.display = 'none');
        // show projects section using grid so CSS layout applies
        projectsSection.style.display = 'grid';
        // load project charts by default
        if (typeof loadEmployeeCharts === 'function') {
            loadEmployeeCharts();
            loadExtraEmployeeCharts(); 
        }
    }
});

