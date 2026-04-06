namespace Final_v1.Domain.Entites
{
    public class Project
    {

            public int ProjectID { get; set; }

            public string ProjectName { get; set; } = string.Empty;

            public string ClientName { get; set; } = string.Empty;

            public int Capacity { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public string ProjectStatus { get; set; } = "Active"; // 'Active' | 'Inactive'

        public int Assigned { get; set; }

    }

    }