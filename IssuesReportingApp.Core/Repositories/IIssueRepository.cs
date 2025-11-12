using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public interface IIssueRepository
{
    IEnumerable<Issue> GetAll();
    Issue? GetById(int id);
    Issue Add(Issue issue);
    bool Update(Issue issue);
    bool Delete(int id);
}