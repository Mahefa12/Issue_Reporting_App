#if !WINDOWS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingWeb.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EventsAddModel : PageModel
{
    private readonly IEventRepository _eventRepo;
    private static readonly string[] DefaultCategories = new[]
    {
        "Community","Maintenance","Infrastructure","Safety","Health","Education",
        "Culture","Sports","Technology","Announcement"
    };

    public List<SelectListItem> CategoryOptions { get; private set; } = new();

    public EventsAddModel(IEventRepository eventRepo)
    {
        _eventRepo = eventRepo;
        InitializeCategoryOptions();
    }

    public class NewEventInput
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDateDate { get; set; }
        public TimeSpan? StartDateTime { get; set; }
        public DateTime? EndDateDate { get; set; }
        public TimeSpan? EndDateTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Organizer { get; set; } = string.Empty;
        public int Priority { get; set; } = 2;
        public bool IsAnnouncement { get; set; }
    }

    [BindProperty]
    public NewEventInput NewEvent { get; set; } = new();

    public void OnGet()
    {
        InitializeCategoryOptions();
    }

    private void InitializeCategoryOptions()
    {
        var repoCategories = ((InMemoryEventRepository)_eventRepo).GetAll()
            .Select(e => e.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var all = DefaultCategories.Concat(repoCategories)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c)
            .ToList();

        CategoryOptions = all
            .Select(c => new SelectListItem { Text = c, Value = c })
            .ToList();
    }

    private bool Validate(NewEventInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title)) ModelState.AddModelError("NewEvent.Title", "Title is required");
        // Dates and times are optional. Only validate ordering if both are fully provided.
        var hasStart = input.StartDateDate.HasValue || input.StartDateTime.HasValue;
        var hasEnd = input.EndDateDate.HasValue || input.EndDateTime.HasValue;
        if (hasStart && hasEnd)
        {
            var start = Combine(input.StartDateDate, input.StartDateTime) ?? DateTime.Now;
            var end = Combine(input.EndDateDate, input.EndDateTime) ?? start;
            if (end < start)
            {
                ModelState.AddModelError("NewEvent.EndDateDate", "End must not be before start");
            }
        }
        return ModelState.IsValid;
    }

    private static DateTime? Combine(DateTime? date, TimeSpan? time)
    {
        if (!date.HasValue && !time.HasValue) return null;
        var d = (date ?? DateTime.Today).Date;
        var t = time ?? TimeSpan.Zero;
        return d.Add(t);
    }

    public IActionResult OnPostAddEvent()
    {
        NewEvent.IsAnnouncement = false;
        if (!Validate(NewEvent)) return Page();

        var start = Combine(NewEvent.StartDateDate, NewEvent.StartDateTime) ?? DateTime.Now;
        var end = Combine(NewEvent.EndDateDate, NewEvent.EndDateTime) ?? start.AddHours(1);

        var ev = new Event
        {
            Title = NewEvent.Title,
            Description = NewEvent.Description,
            StartDate = start,
            EndDate = end,
            Location = NewEvent.Location,
            Category = NewEvent.Category,
            Organizer = NewEvent.Organizer,
            Priority = NewEvent.Priority,
            IsAnnouncement = NewEvent.IsAnnouncement,
        };
        ((InMemoryEventRepository)_eventRepo).Add(ev);
        return RedirectToPage();
    }

    public IActionResult OnPostAddAnnouncement()
    {
        NewEvent.IsAnnouncement = true;
        if (!Validate(NewEvent)) return Page();

        var start = Combine(NewEvent.StartDateDate, NewEvent.StartDateTime) ?? DateTime.Now;
        var end = Combine(NewEvent.EndDateDate, NewEvent.EndDateTime) ?? start;

        var ev = new Event
        {
            Title = NewEvent.Title,
            Description = NewEvent.Description,
            StartDate = start,
            EndDate = end,
            Location = NewEvent.Location,
            Category = NewEvent.Category,
            Organizer = NewEvent.Organizer,
            Priority = NewEvent.Priority,
            IsAnnouncement = true,
        };
        ((InMemoryEventRepository)_eventRepo).Add(ev);
        return RedirectToPage();
    }
}
#endif