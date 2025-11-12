using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingWeb.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IssuesReportingApp.Services.RecommendationEngine _engine;
        private readonly IssuesReportingApp.Repositories.IIssueRepository _issues;
        private readonly IssuesReportingApp.Repositories.IServiceRequestRepository _requests;
        private readonly IssuesReportingApp.Repositories.IIdeaRepository _ideas;
        private readonly IssuesReportingApp.Repositories.IEventRepository _eventsRepo;

        public HomeController(IssuesReportingApp.Services.RecommendationEngine engine,
                              IssuesReportingApp.Repositories.IIssueRepository issues,
                              IssuesReportingApp.Repositories.IServiceRequestRepository requests,
                              IssuesReportingApp.Repositories.IIdeaRepository ideas,
                              IssuesReportingApp.Repositories.IEventRepository eventsRepo)
        {
            _engine = engine;
            _issues = issues;
            _requests = requests;
            _ideas = ideas;
            _eventsRepo = eventsRepo;
        }

        public class HomeViewModel
        {
            public List<IssuesReportingApp.Services.RecommendationResult> Events { get; set; } = new();
            public IEnumerable<IssuesReportingApp.Models.Issue> RecentIssues { get; set; } = Enumerable.Empty<IssuesReportingApp.Models.Issue>();
            public IEnumerable<IssuesReportingApp.Models.ImprovementIdea> RecentIdeas { get; set; } = Enumerable.Empty<IssuesReportingApp.Models.ImprovementIdea>();
            public IEnumerable<IssuesReportingApp.Models.Event> LatestAnnouncements { get; set; } = Enumerable.Empty<IssuesReportingApp.Models.Event>();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Index()
        {
            var vm = new HomeViewModel();

            // Events: show one most recent by CreatedDate among recommendations
            vm.Events = _engine.GetRecommendations(10)
                .OrderByDescending(r => r.Event.CreatedDate)
                .Take(1)
                .ToList();

            // Issues: service requests are treated as reported issues; show one most recent
            var fromRequests = _requests.GetAll().Select(IssuesReportingApp.Services.RequestIssueMapper.ToIssue);
            vm.RecentIssues = fromRequests
                .Concat(_issues.GetAll())
                .OrderByDescending(i => i.CreatedDate)
                .Take(1)
                .ToList();

            // Ideas: show one most recent submission by CreatedDate
            vm.RecentIdeas = _ideas.GetAll()
                .OrderByDescending(i => i.CreatedDate)
                .Take(1)
                .ToList();

            // Announcements: show latest 3 events flagged as announcements
            var eventsConcrete = (IssuesReportingApp.Repositories.InMemoryEventRepository)_eventsRepo;
            vm.LatestAnnouncements = eventsConcrete.GetAll()
                .Where(e => e.IsAnnouncement)
                .OrderByDescending(e => e.CreatedDate)
                .Take(3)
                .ToList();

            return View(vm);
        }
    }
}