using System;
using System.Collections.Generic;

namespace IssuesReportingApp.Models
{
    public enum ServiceRequestStatus
    {
        Received,
        InProgress,
        WaitingOnExternal,
        Completed,
        Cancelled
    }

    public class ServiceRequest : IComparable<ServiceRequest>
    {
        public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty; // e.g., ISSUE-1
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Priority { get; set; } = 3; // 1=High, 2=Medium, 3=Low
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Received;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public List<string> Dependencies { get; set; } = new(); // identifiers of related requests

        // For heap ordering: smaller priority first, then earlier creation date
        public int CompareTo(ServiceRequest? other)
        {
            if (other == null) return -1;
            var byPriority = Priority.CompareTo(other.Priority);
            if (byPriority != 0) return byPriority;
            return CreatedDate.CompareTo(other.CreatedDate);
        }
    }
}