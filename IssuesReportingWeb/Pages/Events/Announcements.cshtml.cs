using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Services;
using System.Collections.Generic;

public class AnnouncementsModel : PageModel
{
    private readonly RecommendationEngine _engine;

    public AnnouncementsModel(RecommendationEngine engine)
    {
        _engine = engine;
    }

    public List<IssuesReportingApp.Models.Event> Announcements { get; private set; } = new();

    public void OnGet()
    {
        Announcements = _engine.GetAnnouncements(8);
    }
}