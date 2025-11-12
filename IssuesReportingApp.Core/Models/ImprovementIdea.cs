namespace IssuesReportingApp.Models;

public class ImprovementIdea
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int Votes { get; set; } = 0;
    public string Status { get; set; } = "Submitted"; // Submitted, Accepted
}