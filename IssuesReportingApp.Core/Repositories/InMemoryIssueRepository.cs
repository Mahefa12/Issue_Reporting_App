using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public class InMemoryIssueRepository : IIssueRepository
{
    private readonly List<Issue> _issues = new();
    private int _nextId = 1;
    
    public InMemoryIssueRepository()
    {
    }

    public InMemoryIssueRepository(IEnumerable<Issue> seed)
    {
        if (seed != null)
        {
            foreach (var i in seed)
            {
                i.Id = _nextId++;
                _issues.Add(i);
            }
        }
    }

    public IEnumerable<Issue> GetAll() => _issues.OrderByDescending(i => i.CreatedDate);

    public Issue? GetById(int id) => _issues.FirstOrDefault(i => i.Id == id);

    public Issue Add(Issue issue)
    {
        issue.Id = _nextId++;
        issue.CreatedDate = DateTime.UtcNow;
        _issues.Add(issue);
        return issue;
    }

    public bool Update(Issue issue)
    {
        var existing = GetById(issue.Id);
        if (existing == null) return false;
        existing.Title = issue.Title;
        existing.Description = issue.Description;
        existing.Category = issue.Category;
        existing.Priority = issue.Priority;
        existing.Status = issue.Status;
        existing.ReporterName = issue.ReporterName;
        existing.ContactInfo = issue.ContactInfo;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;
        return _issues.Remove(existing);
    }
}