using System;

namespace IssuesReportingWeb.Controllers
{
    public class IssuesController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IssuesReportingApp.Repositories.IIssueRepository _repo;
        public IssuesController(IssuesReportingApp.Repositories.IIssueRepository repo) => _repo = repo;

        public class IssuesViewModel
        {
            public IssuesReportingApp.Models.Issue Input { get; set; } = new IssuesReportingApp.Models.Issue();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Index()
        {
            var vm = new IssuesViewModel();
            return View(vm);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.IActionResult Index(IssuesReportingApp.Models.Issue input)
        {
            input.Category ??= string.Empty;
            input.ReporterName ??= string.Empty;
            input.ContactInfo ??= string.Empty;

            if (string.IsNullOrWhiteSpace(input.Title)) ModelState.AddModelError("Input.Title", "Title is required");
            if (string.IsNullOrWhiteSpace(input.Description)) ModelState.AddModelError("Input.Description", "Description is required");
            if (string.IsNullOrWhiteSpace(input.Category)) ModelState.AddModelError("Input.Category", "Category is required");

            if (!ModelState.IsValid)
            {
                var vmInvalid = new IssuesViewModel { Input = input };
                return View(vmInvalid);
            }

            input.Status = "Received";
            input.Priority = string.IsNullOrWhiteSpace(input.Priority) ? "Medium" : input.Priority;
            var created = _repo.Add(input);
            TempData["Success"] = $"Report submitted: #{created.Id} — {created.Title}";
            return RedirectToAction("Index");
        }
    }
}