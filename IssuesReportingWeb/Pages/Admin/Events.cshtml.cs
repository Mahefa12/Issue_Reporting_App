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
public class EventsModel : PageModel
{
    private readonly IEventRepository _eventRepo;

    public EventsModel(IEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
    }

    public IReadOnlyList<Event> Events { get; private set; } = Array.Empty<Event>();

    public class NewEventInput
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now.AddHours(1);
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(2);
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Organizer { get; set; } = string.Empty;
        public int Priority { get; set; } = 2;
        public bool IsAnnouncement { get; set; }
    }

    [BindProperty]
    public NewEventInput NewEvent { get; set; } = new();

    public void OnGet() => Load();

    private void Load()
    {
        var repo = (InMemoryEventRepository)_eventRepo;
        Events = repo.GetAll().ToList();
    }

    public IActionResult OnPostAddEvent()
    {
        if (string.IsNullOrWhiteSpace(NewEvent.Title)) ModelState.AddModelError("NewEvent.Title", "Title is required");
        if (NewEvent.EndDate <= NewEvent.StartDate) ModelState.AddModelError("NewEvent.EndDate", "End date must be after start date");
        if (!ModelState.IsValid)
        {
            Load();
            return Page();
        }

        var ev = new IssuesReportingApp.Models.Event
        {
            Title = NewEvent.Title,
            Description = NewEvent.Description,
            StartDate = NewEvent.StartDate,
            EndDate = NewEvent.EndDate,
            Location = NewEvent.Location,
            Category = NewEvent.Category,
            Organizer = NewEvent.Organizer,
            Priority = NewEvent.Priority,
            IsAnnouncement = NewEvent.IsAnnouncement,
        };
        ((InMemoryEventRepository)_eventRepo).Add(ev);
        return RedirectToPage();
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

        var updated = new IssuesReportingApp.Models.Event
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