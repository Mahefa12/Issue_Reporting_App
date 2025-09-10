using System;
using System.Collections.Generic;

namespace IssuesReportingApp.Models
{
    public class ImprovementIdea
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SubmitterName { get; set; } = string.Empty;
        public string SubmitterEmail { get; set; } = string.Empty;
        public List<string> AttachedFiles { get; set; } = new List<string>();
        public DateTime SubmittedDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Submitted";
        public string ExpectedBenefit { get; set; } = string.Empty;
        public string EstimatedCost { get; set; } = string.Empty;

        public ImprovementIdea()
        {
            SubmittedDate = DateTime.Now;
        }

        public ImprovementIdea(string title, string category, string description, string submitterName, string submitterEmail)
        {
            Title = title;
            Category = category;
            Description = description;
            SubmitterName = submitterName;
            SubmitterEmail = submitterEmail;
            SubmittedDate = DateTime.Now;
        }
    }
}