namespace Final_v1.Domain.Entites
{
    public class Allocation
    {
        public int AllocationID { get; set; }

        public int EmployeeID { get; set; }
        public int ProjectID { get; set; }

        public DateTime AllocationStartDate { get; set; }
        public DateTime? AllocationEndDate { get; set; }

        public int AllocationPercentage { get; set; } = 100;

        // Navigation (not mandatory, depends on your architecture)
        public Employee? Employee { get; set; }
        public Project? Project { get; set; }
    }
}
