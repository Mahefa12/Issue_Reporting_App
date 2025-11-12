using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IssuesReportingApp.Models;
using IssuesReportingApp.Repositories;

namespace IssuesReportingWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly IIdeaRepository _ideas;
        private readonly IEventRepository _events;
        private readonly IServiceRequestRepository _requests;

        public AdminController(IIdeaRepository ideas, IEventRepository events, IServiceRequestRepository requests)
        {
            _ideas = ideas;
            _events = events;
            _requests = requests;
        }

        public class LoginViewModel
        {
            public class LoginInput
            {
                public string Username { get; set; } = string.Empty;
                public string Password { get; set; } = string.Empty;
            }

            public LoginInput Input { get; set; } = new();
            public string? ErrorMessage { get; set; }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = "Admin Login";
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            const string DemoUser = "admin";
            const string DemoPass = "admin123";

            if (string.IsNullOrWhiteSpace(model.Input.Username)) ModelState.AddModelError("Input.Username", "Username is required");
            if (string.IsNullOrWhiteSpace(model.Input.Password)) ModelState.AddModelError("Input.Password", "Password is required");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.Equals(model.Input.Username, DemoUser, StringComparison.Ordinal) ||
                !string.Equals(model.Input.Password, DemoPass, StringComparison.Ordinal))
            {
                model.ErrorMessage = "Invalid credentials";
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, DemoUser),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = false });

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Logout()
        {
            ViewData["Title"] = "Logout";
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public class IndexViewModel
        {
            // Event metrics
            public int EventsUpcoming { get; set; }
            public int EventsOngoing { get; set; }
            public int EventsCompleted { get; set; }
            public int EventsAnnouncements { get; set; }
            // Service request metrics
            public int RequestsReceived { get; set; }
            public int RequestsInProgress { get; set; }
            public int RequestsWaitingOnExternal { get; set; }
            public int RequestsCompleted { get; set; }
            public int RequestsCancelled { get; set; }
            // Idea metrics
            public int IdeasSubmitted { get; set; }
            public int IdeasAccepted { get; set; }
            public int IdeasRejected { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new IndexViewModel();

            var evs = _events.GetAll();
            vm.EventsAnnouncements = evs.Count(e => e.IsAnnouncement);
            vm.EventsUpcoming = evs.Count(e => e.IsUpcoming && !e.IsAnnouncement);
            vm.EventsOngoing = evs.Count(e => e.IsOngoing && !e.IsAnnouncement);
            vm.EventsCompleted = evs.Count(e => e.IsCompleted && !e.IsAnnouncement);

            var reqs = _requests.GetAll();
            vm.RequestsReceived = reqs.Count(r => r.Status == ServiceRequestStatus.Received);
            vm.RequestsInProgress = reqs.Count(r => r.Status == ServiceRequestStatus.InProgress);
            vm.RequestsWaitingOnExternal = reqs.Count(r => r.Status == ServiceRequestStatus.WaitingOnExternal);
            vm.RequestsCompleted = reqs.Count(r => r.Status == ServiceRequestStatus.Completed);
            vm.RequestsCancelled = reqs.Count(r => r.Status == ServiceRequestStatus.Cancelled);

            var ideas = _ideas.GetAll();
            vm.IdeasSubmitted = ideas.Count(i => string.Equals(i.Status, "Submitted", StringComparison.OrdinalIgnoreCase));
            vm.IdeasAccepted = ideas.Count(i => string.Equals(i.Status, "Accepted", StringComparison.OrdinalIgnoreCase));
            vm.IdeasRejected = ideas.Count(i => string.Equals(i.Status, "Rejected", StringComparison.OrdinalIgnoreCase));

            ViewData["Title"] = "Admin Dashboard";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Events()
        {
            ViewData["Title"] = "Events Hub";
            return View();
        }

        public class EventsAddViewModel
        {
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

            public NewEventInput NewEvent { get; set; } = new();
            public List<SelectListItem> CategoryOptions { get; set; } = new();
        }

        private static readonly string[] DefaultCategories = new[]
        {
            "Community","Maintenance","Infrastructure","Safety","Health","Education",
            "Culture","Sports","Technology","Announcement"
        };

        private void InitializeCategoryOptions(EventsAddViewModel vm)
        {
            var repoCategories = ((InMemoryEventRepository)_events).GetAll()
                .Select(e => e.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var all = DefaultCategories.Concat(repoCategories)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            vm.CategoryOptions = all
                .Select(c => new SelectListItem { Text = c, Value = c })
                .ToList();
        }

        private static DateTime? Combine(DateTime? date, TimeSpan? time)
        {
            if (!date.HasValue && !time.HasValue) return null;
            var d = (date ?? DateTime.Today).Date;
            var t = time ?? TimeSpan.Zero;
            return d.Add(t);
        }

        private bool Validate(EventsAddViewModel.NewEventInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title)) ModelState.AddModelError("NewEvent.Title", "Title is required");
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EventsAdd()
        {
            var vm = new EventsAddViewModel();
            InitializeCategoryOptions(vm);
            ViewData["Title"] = "Add Events or Announcements";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddEvent(EventsAddViewModel vm)
        {
            vm.NewEvent.IsAnnouncement = false;
            if (!Validate(vm.NewEvent))
            {
                InitializeCategoryOptions(vm);
                ViewData["Title"] = "Add Events or Announcements";
                return View("EventsAdd", vm);
            }

            var start = Combine(vm.NewEvent.StartDateDate, vm.NewEvent.StartDateTime) ?? DateTime.Now;
            var end = Combine(vm.NewEvent.EndDateDate, vm.NewEvent.EndDateTime) ?? start.AddHours(1);

            var ev = new Event
            {
                Title = vm.NewEvent.Title,
                Description = vm.NewEvent.Description,
                StartDate = start,
                EndDate = end,
                Location = vm.NewEvent.Location,
                Category = vm.NewEvent.Category,
                Organizer = vm.NewEvent.Organizer,
                Priority = vm.NewEvent.Priority,
                IsAnnouncement = vm.NewEvent.IsAnnouncement,
            };
            ((InMemoryEventRepository)_events).Add(ev);
            return RedirectToAction("EventsAdd");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddAnnouncement(EventsAddViewModel vm)
        {
            vm.NewEvent.IsAnnouncement = true;
            if (!Validate(vm.NewEvent))
            {
                InitializeCategoryOptions(vm);
                ViewData["Title"] = "Add Events or Announcements";
                return View("EventsAdd", vm);
            }

            var start = Combine(vm.NewEvent.StartDateDate, vm.NewEvent.StartDateTime) ?? DateTime.Now;
            var end = Combine(vm.NewEvent.EndDateDate, vm.NewEvent.EndDateTime) ?? start;

            var ev = new Event
            {
                Title = vm.NewEvent.Title,
                Description = vm.NewEvent.Description,
                StartDate = start,
                EndDate = end,
                Location = vm.NewEvent.Location,
                Category = vm.NewEvent.Category,
                Organizer = vm.NewEvent.Organizer,
                Priority = vm.NewEvent.Priority,
                IsAnnouncement = true,
            };
            ((InMemoryEventRepository)_events).Add(ev);
            return RedirectToAction("EventsAdd");
        }

        public class EventsManageViewModel
        {
            public IReadOnlyList<Event> Events { get; set; } = Array.Empty<Event>();
            public int UpcomingCount { get; set; }
            public int OngoingCount { get; set; }
            public int CompletedCount { get; set; }
            public int AnnouncementsCount { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EventsManage()
        {
            var repo = (InMemoryEventRepository)_events;
            var all = repo.GetAll().ToList();
            var vm = new EventsManageViewModel
            {
                Events = all,
                UpcomingCount = all.Count(e => e.IsUpcoming && !e.IsAnnouncement),
                OngoingCount = all.Count(e => e.IsOngoing && !e.IsAnnouncement),
                CompletedCount = all.Count(e => e.IsCompleted && !e.IsAnnouncement),
                AnnouncementsCount = all.Count(e => e.IsAnnouncement)
            };
            ViewData["Title"] = "Manage Events and Announcements";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            ((InMemoryEventRepository)_events).Delete(id);
            return RedirectToAction("EventsManage");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeletePastEvents()
        {
            var repo = (InMemoryEventRepository)_events;
            foreach (var e in repo.GetAll().Where(e => e.EndDate < DateTime.Now).ToList())
            {
                repo.Delete(e.Id);
            }
            return RedirectToAction("EventsManage");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateEvent(int id, string title, string description, DateTime startDate, DateTime endDate, string location, string organizer, string category, int priority, bool isAnnouncement)
        {
            if (string.IsNullOrWhiteSpace(title)) ModelState.AddModelError("title", "Title is required");
            if (endDate <= startDate) ModelState.AddModelError("endDate", "End date must be after start date");
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EventsManage");
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

            ((InMemoryEventRepository)_events).Update(updated);
            return RedirectToAction("EventsManage");
        }

        public class RequestsViewModel
        {
            public IReadOnlyList<ServiceRequest> Requests { get; set; } = Array.Empty<ServiceRequest>();
            public int ReceivedCount { get; set; }
            public int InProgressCount { get; set; }
            public int WaitingOnExternalCount { get; set; }
            public int CompletedCount { get; set; }
            public int CancelledCount { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Requests()
        {
            var items = _requests.GetAll().ToList();
            var vm = new RequestsViewModel
            {
                Requests = items,
                ReceivedCount = items.Count(r => r.Status == ServiceRequestStatus.Received),
                InProgressCount = items.Count(r => r.Status == ServiceRequestStatus.InProgress),
                WaitingOnExternalCount = items.Count(r => r.Status == ServiceRequestStatus.WaitingOnExternal),
                CompletedCount = items.Count(r => r.Status == ServiceRequestStatus.Completed),
                CancelledCount = items.Count(r => r.Status == ServiceRequestStatus.Cancelled),
            };
            ViewData["Title"] = "Manage Requests / Reports";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateRequest(string identifier, ServiceRequestStatus status)
        {
            var req = _requests.GetByIdentifier(identifier);
            if (req != null)
            {
                req.Status = status;
                _requests.Update(req);
            }
            return RedirectToAction("Requests");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteRequest(string identifier)
        {
            _requests.Delete(identifier);
            return RedirectToAction("Requests");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCompletedRequests()
        {
            foreach (var r in _requests.GetAll().Where(r => r.Status == ServiceRequestStatus.Completed || r.Status == ServiceRequestStatus.Cancelled).ToList())
            {
                _requests.Delete(r.Identifier);
            }
            return RedirectToAction("Requests");
        }

        public class IdeasViewModel
        {
            public IReadOnlyList<ImprovementIdea> Ideas { get; set; } = Array.Empty<ImprovementIdea>();
            public int SubmittedCount { get; set; }
            public int AcceptedCount { get; set; }
            public int RejectedCount { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Ideas()
        {
            var items = _ideas.GetAll().ToList();
            var vm = new IdeasViewModel
            {
                Ideas = items,
                SubmittedCount = items.Count(i => string.Equals(i.Status, "Submitted", StringComparison.OrdinalIgnoreCase)),
                AcceptedCount = items.Count(i => string.Equals(i.Status, "Accepted", StringComparison.OrdinalIgnoreCase)),
                RejectedCount = items.Count(i => string.Equals(i.Status, "Rejected", StringComparison.OrdinalIgnoreCase)),
            };
            ViewData["Title"] = "Manage Ideas";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AcceptIdea(int id)
        {
            var idea = _ideas.GetById(id);
            if (idea != null)
            {
                idea.Status = "Accepted";
                _ideas.Update(idea);
            }
            return RedirectToAction("Ideas");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RejectIdea(int id)
        {
            var idea = _ideas.GetById(id);
            if (idea != null)
            {
                idea.Status = "Rejected";
                _ideas.Update(idea);
            }
            return RedirectToAction("Ideas");
        }
    }
}