#if !WINDOWS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace IssuesReportingWeb.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EventsManageModel : PageModel
{
    private readonly IEventRepository _eventRepo;

    public EventsManageModel(IEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
    }

    public IReadOnlyList<Event> Events { get; private set; } = Array.Empty<Event>();
    public int UpcomingCount { get; private set; }
    public int OngoingCount { get; private set; }
    public int CompletedCount { get; private set; }
    public int AnnouncementsCount { get; private set; }

    public void OnGet() => Load();

    private void Load()
    {
        var repo = (InMemoryEventRepository)_eventRepo;
        Events = repo.GetAll().ToList();
        UpcomingCount = Events.Count(e => e.IsUpcoming && !e.IsAnnouncement);
        OngoingCount = Events.Count(e => e.IsOngoing && !e.IsAnnouncement);
        CompletedCount = Events.Count(e => e.IsCompleted && !e.IsAnnouncement);
        AnnouncementsCount = Events.Count(e => e.IsAnnouncement);
    }

    public IActionResult OnPostDeleteEvent(int id)
    {
        ((InMemoryEventRepository)_eventRepo).Delete(id);
        return RedirectToPage();
    }

    public IActionResult OnPostDeletePastEvents()
    {
        var repo = (InMemoryEventRepository)_eventRepo;
        foreach (var e in repo.GetAll().Where(e => e.EndDate < DateTime.Now).ToList())
        {
            repo.Delete(e.Id);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostUpdateEvent(
        int id,
        string title,
        string description,
        DateTime startDate,
        DateTime endDate,
        string location,
        string organizer,
        string category,
        int priority,
        bool isAnnouncement)
    {
        if (string.IsNullOrWhiteSpace(title)) ModelState.AddModelError("title", "Title is required");
        if (endDate <= startDate) ModelState.AddModelError("endDate", "End date must be after start date");
        if (!ModelState.IsValid)
        {
            Load();
            return Page();
        }

        var updated = new Event
        {
            Id = id,
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Location = location,
            Organizer = organizer,
            Category = category,
            Priority = priority,
            IsAnnouncement = isAnnouncement,
        };

        ((InMemoryEventRepository)_eventRepo).Update(updated);
        return RedirectToPage();
    }
}
#endif