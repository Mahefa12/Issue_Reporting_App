using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Models;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Services;

namespace IssuesReportingWeb.Pages.ServiceRequests;

public class IndexModel : PageModel
{
    private readonly ServiceRequestIndex _index;
    private readonly IIssueRepository _issues;

    public IndexModel(ServiceRequestIndex index, IIssueRepository issues)
    {
        _index = index;
        _issues = issues;
    }

    [BindProperty(SupportsGet = true)]
    public string? IdentifierQuery { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public List<ServiceRequest> Items { get; private set; } = new();
    public ServiceRequest? TrackedRequest { get; private set; }
    public List<string>? DependencyPath { get; private set; }
    public List<ServiceRequest> NextUp { get; private set; } = new();
    public List<(string u, string v)>? DependencyBackbone { get; private set; }
    public List<ServiceRequest> Alphabetical { get; private set; } = new();

    public void OnGet()
    {
        // Unify: include reported issues as service requests
        IEnumerable<ServiceRequest> fromRequests = _index.All();
        if (StartDate.HasValue && EndDate.HasValue)
        {
            // Use BST-backed range query for created date
            fromRequests = _index.RangeByCreatedDate(StartDate.Value, EndDate.Value);
        }
        var reqList = fromRequests.ToList();
        var fromIssues = _issues.GetAll().Select(RequestIssueMapper.ToServiceRequest);
        Items = reqList
            .Concat(fromIssues)
            .OrderBy(r => r.Priority)
            .ThenByDescending(r => r.UpdatedDate ?? r.CreatedDate)
            .ToList();
        NextUp = _index.NextToProcess(5);
        Alphabetical = _index.Alphabetical(10).ToList();

        var query = (SearchQuery ?? IdentifierQuery)?.Trim();
        if (!string.IsNullOrWhiteSpace(query))
        {
            // Prefer exact identifier match, otherwise fallback to title contains
            var match = Items.FirstOrDefault(r =>
                string.Equals(r.Identifier, query, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(r.Title) && r.Title.Contains(query, StringComparison.OrdinalIgnoreCase)));

            if (match != null)
            {
                TrackedRequest = match;
                var firstDep = match.Dependencies?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstDep))
                {
                    DependencyPath = _index.DependencyPath(match.Identifier, firstDep);
                }
                // Build minimal dependency backbone (MST) for the connected component
                DependencyBackbone = _index.DependencyMstForComponent(match.Identifier);
            }
        }
    }
}