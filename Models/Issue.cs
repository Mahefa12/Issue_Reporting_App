using System;
using IssuesReportingApp.DataStructures;

namespace IssuesReportingApp.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CustomLinkedList<string> AttachedFiles { get; set; } = new CustomLinkedList<string>();
        public DateTime ReportedDate { get; set; }
        public string Status { get; set; } = "Submitted";

        public Issue()
        {
            AttachedFiles = new CustomLinkedList<string>();
            ReportedDate = DateTime.Now;
            Status = "Submitted";
        }

        public Issue(string location, string category, string description) : this()
        {
            Location = location;
            Category = category;
            Description = description;
        }
    }
}