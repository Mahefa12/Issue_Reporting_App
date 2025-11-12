using IssuesReportingApp.Models;

namespace IssuesReportingApp.Services
{
    public static class RequestIssueMapper
    {
        public static Issue ToIssue(ServiceRequest r)
        {
            return new Issue
            {
                Title = r.Title,
                Description = r.Description,
                Category = r.Category,
                Priority = r.Priority switch { 1 => "High", 2 => "Medium", _ => "Low" },
                Status = r.Status switch
                {
                    ServiceRequestStatus.InProgress => "In Progress",
                    ServiceRequestStatus.Received => "Received",
                    ServiceRequestStatus.Completed => "Completed",
                    _ => r.Status.ToString()
                },
                CreatedDate = r.CreatedDate
            };
        }

        public static ServiceRequest ToServiceRequest(Issue i)
        {
            var priority = i.Priority?.Equals("High", StringComparison.OrdinalIgnoreCase) == true
                ? 1
                : i.Priority?.Equals("Medium", StringComparison.OrdinalIgnoreCase) == true
                    ? 2
                    : 3;

            var status = ServiceRequestStatus.Received;
            var s = i.Status?.Trim() ?? string.Empty;
            // Normalize legacy or spaced statuses
            if (s.Equals("In Progress", StringComparison.OrdinalIgnoreCase)) s = "InProgress";
            else if (s.Equals("Open", StringComparison.OrdinalIgnoreCase)) s = "Received";
            else if (s.Equals("Resolved", StringComparison.OrdinalIgnoreCase)) s = "Completed";
            if (Enum.TryParse<ServiceRequestStatus>(s, true, out var parsed))
            {
                status = parsed;
            }

            return new ServiceRequest
            {
                Identifier = $"ISSUE-{i.Id}",
                Title = i.Title,
                Description = i.Description,
                Category = i.Category ?? string.Empty,
                Priority = priority,
                Status = status,
                CreatedDate = i.CreatedDate,
                Dependencies = new List<string>()
            };
        }
    }
}