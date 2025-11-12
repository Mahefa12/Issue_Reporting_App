using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Models;
using IssuesReportingApp.Repositories;

namespace IssuesReportingWeb.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IIdeaRepository _ideas;
    private readonly IEventRepository _events;
    private readonly IServiceRequestRepository _requests;

    // Event metrics
    public int EventsUpcoming { get; private set; }
    public int EventsOngoing { get; private set; }
    public int EventsCompleted { get; private set; }
    public int EventsAnnouncements { get; private set; }

    // Service request metrics
    public int RequestsReceived { get; private set; }
    public int RequestsInProgress { get; private set; }
    public int RequestsWaitingOnExternal { get; private set; }
    public int RequestsCompleted { get; private set; }
    public int RequestsCancelled { get; private set; }

    // Idea metrics
    public int IdeasSubmitted { get; private set; }
    public int IdeasAccepted { get; private set; }

    public IndexModel(IIdeaRepository ideas, IEventRepository events, IServiceRequestRepository requests)
    {
        _ideas = ideas;
        _events = events;
        _requests = requests;
    }

    public void OnGet()
    {
        // Events
        var evs = _events.GetAll();
        EventsAnnouncements = evs.Count(e => e.IsAnnouncement);
        EventsUpcoming = evs.Count(e => e.IsUpcoming && !e.IsAnnouncement);
        EventsOngoing = evs.Count(e => e.IsOngoing && !e.IsAnnouncement);
        EventsCompleted = evs.Count(e => e.IsCompleted && !e.IsAnnouncement);

        // Service Requests
        var reqs = _requests.GetAll();
        RequestsReceived = reqs.Count(r => r.Status == ServiceRequestStatus.Received);
        RequestsInProgress = reqs.Count(r => r.Status == ServiceRequestStatus.InProgress);
        RequestsWaitingOnExternal = reqs.Count(r => r.Status == ServiceRequestStatus.WaitingOnExternal);
        RequestsCompleted = reqs.Count(r => r.Status == ServiceRequestStatus.Completed);
        RequestsCancelled = reqs.Count(r => r.Status == ServiceRequestStatus.Cancelled);

        // Ideas
        var ideas = _ideas.GetAll();
        IdeasSubmitted = ideas.Count(i => string.Equals(i.Status, "Submitted", StringComparison.OrdinalIgnoreCase));
        IdeasAccepted = ideas.Count(i => string.Equals(i.Status, "Accepted", StringComparison.OrdinalIgnoreCase));
    }
}