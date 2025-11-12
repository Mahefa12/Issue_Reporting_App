using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingWeb.Controllers
{
    public class EventsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IssuesReportingApp.Services.RecommendationEngine _engine;

        public EventsController(IssuesReportingApp.Services.RecommendationEngine engine)
        {
            _engine = engine;
        }

        public class EventsViewModel
        {
            public string? Query { get; set; }
            public bool ShowAll { get; set; }
            public List<IssuesReportingApp.Services.RecommendationResult> Recommendations { get; set; } = new();
            public List<IssuesReportingApp.Models.Event> AllEvents { get; set; } = new();
            public List<IssuesReportingApp.Models.Event> Announcements { get; set; } = new();
            public List<string> AvailableLocations { get; set; } = new();
            public List<string> AvailableCategories { get; set; } = new();
            public string? Category { get; set; }
            public string? Location { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Priority { get; set; }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Index(
            [Microsoft.AspNetCore.Mvc.FromQuery] string? query,
            [Microsoft.AspNetCore.Mvc.FromQuery] bool showAll = false,
            [Microsoft.AspNetCore.Mvc.FromQuery] string? category = null,
            [Microsoft.AspNetCore.Mvc.FromQuery] string? location = null,
            [Microsoft.AspNetCore.Mvc.FromQuery] DateTime? startDate = null,
            [Microsoft.AspNetCore.Mvc.FromQuery] DateTime? endDate = null,
            [Microsoft.AspNetCore.Mvc.FromQuery] string? priority = null)
        {
            var vm = new EventsViewModel
            {
                Query = query,
                ShowAll = showAll,
                Category = category,
                Location = location,
                StartDate = startDate,
                EndDate = endDate,
                Priority = priority
            };

            vm.Announcements = _engine.GetAnnouncements(8);

            // Build dynamic filter lists from all seeded events
            vm.AvailableLocations = _engine.GetAllEvents()
                .Where(e => !string.IsNullOrWhiteSpace(e.Location))
                .Select(e => e.Location!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
            vm.AvailableCategories = _engine.GetAllEvents()
                .Where(e => !string.IsNullOrWhiteSpace(e.Category))
                .Select(e => e.Category!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            // If any filter is specified, default to showing all events
            if (!vm.ShowAll && (
                !string.IsNullOrWhiteSpace(vm.Category) ||
                !string.IsNullOrWhiteSpace(vm.Location) ||
                vm.StartDate.HasValue || vm.EndDate.HasValue ||
                !string.IsNullOrWhiteSpace(vm.Priority)))
            {
                vm.ShowAll = true;
            }

            if (vm.ShowAll)
            {
                vm.AllEvents = _engine.GetAllEvents()
                    .Where(e => !e.IsAnnouncement)
                    .Where(e => FilterMatches(e, vm))
                    .ToList();
            }
            else
            {
                vm.Recommendations = _engine.GetRecommendations(10)
                    .Where(r => FilterMatches(r.Event, vm))
                    .ToList();
            }

            return View(vm);
        }

        private static bool FilterMatches(IssuesReportingApp.Models.Event e, EventsViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.Category) && !string.Equals(e.Category, vm.Category, StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.IsNullOrWhiteSpace(vm.Location) && (e.Location?.IndexOf(vm.Location, StringComparison.OrdinalIgnoreCase) ?? -1) < 0)
                return false;
            if (vm.StartDate.HasValue && e.StartDate < vm.StartDate.Value)
                return false;
            if (vm.EndDate.HasValue && e.EndDate > vm.EndDate.Value)
                return false;
            if (!string.IsNullOrWhiteSpace(vm.Priority) && !string.Equals(e.PriorityText, vm.Priority, StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.IActionResult Like(int id)
        {
            _engine.RecordFeedback(id, liked: true);
            return RedirectToAction("Index");
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.IActionResult Dislike(int id)
        {
            _engine.RecordFeedback(id, liked: false);
            return RedirectToAction("Index");
        }
    }
}