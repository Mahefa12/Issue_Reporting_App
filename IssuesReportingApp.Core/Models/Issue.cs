namespace IssuesReportingApp.Models;

public class Issue
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Received";
    public string? ReporterName { get; set; }
    public string? ContactInfo { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}