using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public class InMemoryServiceRequestRepository : IServiceRequestRepository
{
    private readonly List<ServiceRequest> _requests = new();
    private int _nextId = 1;
    
    public InMemoryServiceRequestRepository(IEnumerable<ServiceRequest>? seed = null)
    {
        if (seed == null)
        {
            // Default small sample data
            Seed();
        }
        else
        {
            foreach (var r in seed)
            {
                r.Id = _nextId++;
                _requests.Add(r);
            }
        }
    }

    public IEnumerable<ServiceRequest> GetAll() =>
        _requests.OrderBy(r => r.Priority).ThenByDescending(r => r.CreatedDate);

    public ServiceRequest? GetByIdentifier(string identifier) =>
        _requests.FirstOrDefault(r => r.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase));

    public ServiceRequest Add(ServiceRequest request)
    {
        request.Id = _nextId++;
        request.CreatedDate = DateTime.UtcNow;
        _requests.Add(request);
        return request;
    }

    public bool Update(ServiceRequest request)
    {
        var existing = GetByIdentifier(request.Identifier);
        if (existing == null) return false;
        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Category = request.Category;
        existing.Priority = request.Priority;
        existing.Status = request.Status;
        existing.UpdatedDate = DateTime.UtcNow;
        existing.Dependencies = request.Dependencies ?? new List<string>();
        return true;
    }

    public bool Delete(string identifier)
    {
        var existing = GetByIdentifier(identifier);
        if (existing == null) return false;
        _requests.Remove(existing);
        return true;
    }

    private void Seed()
    {
        Add(new ServiceRequest
        {
            Identifier = "ISSUE-1",
            Title = "Streetlight repair",
            Description = "Lamp post not working on Main St.",
            Category = "Infrastructure",
            Priority = 1,
            Status = ServiceRequestStatus.InProgress,
            Dependencies = new List<string>{"ISSUE-3"}
        });
        Add(new ServiceRequest
        {
            Identifier = "ISSUE-2",
            Title = "Waste collection delay",
            Description = "Missed pickup in Zone 5.",
            Category = "Sanitation",
            Priority = 2,
            Status = ServiceRequestStatus.Received
        });
        Add(new ServiceRequest
        {
            Identifier = "ISSUE-3",
            Title = "Supply part procurement",
            Description = "Awaiting part for lamp post.",
            Category = "Procurement",
            Priority = 1,
            Status = ServiceRequestStatus.InProgress
        });
        Add(new ServiceRequest
        {
            Identifier = "ISSUE-4",
            Title = "Pothole fix",
            Description = "Road repair request.",
            Category = "Infrastructure",
            Priority = 3,
            Status = ServiceRequestStatus.Completed
        });
    }
}