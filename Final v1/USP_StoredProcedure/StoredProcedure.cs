namespace Final_v1.USP_StoredProcedure
{
    public class StoredProcedure
    {
        public const string AddSkill = "SP_AddSkill";
        public const string AssignSkillToEmployee = "SP_AssignSkillToEmployee";
        public const string DeleteSkill = "SP_DeleteSkill";
        public const string RemoveSkillFromEmployee = "SP_RemoveSkillFromEmployee";
        public const string UpdateSkill = "SP_UpdateSkill";
        public const string GetAllSkills = "SP_GetAllSkills";
        public const string GetSkillsByEmployee = "SP_GetSkillsByEmployee";


        public const string CheckProjectOverlap = "usp_CheckProjectOverlap";
        public const string GetEmployeeAllocationHistory = "usp_GetEmployeeAllocationHistory";
        public const string GetProjectUtilization = "usp_GetProjectUtilization";
        public const string GetSkillAvailability = "usp_GetSkillAvailability";


        public const string CreateProject = "dbo.usp_Projects_Create";
        public const string DeactivateProject = "dbo.usp_Projects_Deactivate";
        public const string GetProjectById = "dbo.usp_Projects_GetById";
        public const string SearchProjects = "dbo.usp_Projects_Search";
        public const string SetCapacity = "dbo.usp_Projects_SetCapacity";
        public const string SetDates = "dbo.usp_Projects_SetDates";
        public const string SetStatus = "dbo.usp_Projects_SetStatus";
        public const string UpdateProject = "dbo.usp_Projects_Update";
        public const string GetAllProjects = "dbo.GetAllProjects";

        public const string AddEmployee = "SP_AddEmployee";
        public const string GetAllEmployees = "SP_GetAllEmployees";
        public const string UpdateEmployee = "SP_UpdateEmployee";
        public const string SetEmployeeStatus = "SP_SetEmployeeStatus";

        public const string AddEmployeeSkill = "dbo.SP_AddEmployeeSkill";
        public const string GetAllocationHistoryByEmployee = "dbo.GetAllocationHistoryByEmployee";
        public const string GetEmployeesByProject = "GetEmployeesByProject";
        public const string AssignEmployeeToProject = "AssignEmployeeToProject";


        public const string GetBenchEmployees = "dbo.GetBenchEmployees";
        public const string GetBenchPercentage = "dbo.GetBenchPercentage";
        public const string GetBenchBySkill = "dbo.GetBenchBySkill";
        public const string GetBenchAndProjectCounts = "dbo.GetBenchAndProjectCounts";
        public const string GetProjectCapacitySummary = "dbo.GetProjectCapacitySummary";
        public const string GetActiveProjectDates = "dbo.GetActiveProjectDates";

        public const string CountActiveEmployeesAndProjects = "dbo.CountActiveEmployeesAndProjects";

    }
}