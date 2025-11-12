using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using IssuesReportingApp.Services;
using System.Linq;
using System;

public class EventsIndexModel : PageModel
{
    private readonly RecommendationEngine _engine;

    public EventsIndexModel(RecommendationEngine engine)
    {
        _engine = engine;
    }

    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool ShowAll { get; set; }

    public List<RecommendationResult> Recommendations { get; private set; } = new();
    public List<IssuesReportingApp.Models.Event> AllEvents { get; private set; } = new();
    public List<IssuesReportingApp.Models.Event> Announcements { get; private set; } = new();
    public List<string> AvailableLocations { get; private set; } = new();
    public List<string> AvailableCategories { get; private set; } = new();

    // Filters
    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Location { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Priority { get; set; }

    public void OnGet()
    {
        Announcements = _engine.GetAnnouncements(8);
        // Build dynamic filter lists from all seeded events
        AvailableLocations = _engine.GetAllEvents()
            .Where(e => !string.IsNullOrWhiteSpace(e.Location))
            .Select(e => e.Location!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
        AvailableCategories = _engine.GetAllEvents()
            .Where(e => !string.IsNullOrWhiteSpace(e.Category))
            .Select(e => e.Category!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        // If any filter is specified, default to showing all events
        if (!ShowAll && (
            !string.IsNullOrWhiteSpace(Category) ||
            !string.IsNullOrWhiteSpace(Location) ||
            StartDate.HasValue || EndDate.HasValue ||
            !string.IsNullOrWhiteSpace(Priority)))
        {
            ShowAll = true;
        }
        if (ShowAll)
        {
            AllEvents = _engine.GetAllEvents()
                .Where(e => !e.IsAnnouncement)
                .Where(FilterMatches)
                .ToList();
        }
        else
        {
            Recommendations = _engine.GetRecommendations(10)
                .Where(r => FilterMatches(r.Event))
                .ToList();
        }
    }

    private bool FilterMatches(IssuesReportingApp.Models.Event e)
    {
        if (!string.IsNullOrWhiteSpace(Category) && !string.Equals(e.Category, Category, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(Location) && (e.Location?.IndexOf(Location, StringComparison.OrdinalIgnoreCase) ?? -1) < 0)
            return false;
        if (StartDate.HasValue && e.StartDate < StartDate.Value)
            return false;
        if (EndDate.HasValue && e.EndDate > EndDate.Value)
            return false;
        if (!string.IsNullOrWhiteSpace(Priority) && !string.Equals(e.PriorityText, Priority, StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    public IActionResult OnPostLike(int id)
    {
        _engine.RecordFeedback(id, liked: true);
        return RedirectToPage(new { showAll = ShowAll, category = Category, location = Location, startDate = StartDate?.ToString("yyyy-MM-dd"), endDate = EndDate?.ToString("yyyy-MM-dd"), priority = Priority });
    }

    public IActionResult OnPostDislike(int id)
    {
        _engine.RecordFeedback(id, liked: false);
        return RedirectToPage(new { showAll = ShowAll, category = Category, location = Location, startDate = StartDate?.ToString("yyyy-MM-dd"), endDate = EndDate?.ToString("yyyy-MM-dd"), priority = Priority });
    }
}