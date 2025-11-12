using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;
using IssuesReportingApp.Services;

public class IndexModel : PageModel
{
    private readonly RecommendationEngine _engine;
    private readonly IIssueRepository _issues;
    private readonly IServiceRequestRepository _requests;
    private readonly IIdeaRepository _ideas;
    private readonly IEventRepository _eventsRepo;

    public IndexModel(RecommendationEngine engine, IIssueRepository issues, IServiceRequestRepository requests, IIdeaRepository ideas, IEventRepository eventsRepo)
    {
        _engine = engine;
        _issues = issues;
        _requests = requests;
        _ideas = ideas;
        _eventsRepo = eventsRepo;
    }

    public List<RecommendationResult> Events { get; private set; } = new();
    public IEnumerable<Issue> RecentIssues { get; private set; } = Enumerable.Empty<Issue>();
    public IEnumerable<ImprovementIdea> RecentIdeas { get; private set; } = Enumerable.Empty<ImprovementIdea>();
    public IEnumerable<Event> LatestAnnouncements { get; private set; } = Enumerable.Empty<Event>();

    public void OnGet()
    {
        // Events: show one most recent by CreatedDate among recommendations
        Events = _engine.GetRecommendations(10)
            .OrderByDescending(r => r.Event.CreatedDate)
            .Take(1)
            .ToList();

        // Issues: service requests are treated as reported issues; show one most recent
        var fromRequests = _requests.GetAll().Select(RequestIssueMapper.ToIssue);
        RecentIssues = fromRequests
            .Concat(_issues.GetAll())
            .OrderByDescending(i => i.CreatedDate)
            .Take(1);

        // Ideas: show one most recent submission by CreatedDate
        RecentIdeas = _ideas.GetAll()
            .OrderByDescending(i => i.CreatedDate)
            .Take(1);

        // Announcements: show latest 3 events flagged as announcements
        // Cast to concrete repository to avoid ambiguous GetAll() resolution
        LatestAnnouncements = ((InMemoryEventRepository)_eventsRepo).GetAll()
            .Where(e => e.IsAnnouncement)
            .OrderByDescending(e => e.CreatedDate)
            .Take(3);
    }
}