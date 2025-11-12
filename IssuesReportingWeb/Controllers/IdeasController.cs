using System;
using System.Collections.Generic;
using System.Linq;

namespace IssuesReportingWeb.Controllers
{
    public class IdeasController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IssuesReportingApp.Repositories.IIdeaRepository _repo;
        public IdeasController(IssuesReportingApp.Repositories.IIdeaRepository repo) => _repo = repo;

        public class IdeasIndexViewModel
        {
            public IEnumerable<IssuesReportingApp.Models.ImprovementIdea> Items { get; set; } = Enumerable.Empty<IssuesReportingApp.Models.ImprovementIdea>();
            public string? Query { get; set; }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Index([Microsoft.AspNetCore.Mvc.FromQuery] string? q = null)
        {
            var vm = new IdeasIndexViewModel { Query = q };
            var all = _repo.GetAll();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var query = q.Trim().ToLowerInvariant();
                all = all.Where(i =>
                    (i.Title ?? string.Empty).ToLowerInvariant().Contains(query) ||
                    (i.SubmittedBy ?? string.Empty).ToLowerInvariant().Contains(query));
            }
            vm.Items = all;
            return View(vm);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.IActionResult Vote(int id)
        {
            _repo.Vote(id);
            return RedirectToAction("Index");
        }

        public class IdeasCreateViewModel
        {
            public IssuesReportingApp.Models.ImprovementIdea Input { get; set; } = new IssuesReportingApp.Models.ImprovementIdea();
            public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Create()
        {
            var vm = new IdeasCreateViewModel { Categories = DefaultCategories };
            return View(vm);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public Microsoft.AspNetCore.Mvc.IActionResult Create(IssuesReportingApp.Models.ImprovementIdea input)
        {
            if (string.IsNullOrWhiteSpace(input.Title)) ModelState.AddModelError("Input.Title", "Title is required");
            if (string.IsNullOrWhiteSpace(input.Description)) ModelState.AddModelError("Input.Description", "Description is required");
            if (string.IsNullOrWhiteSpace(input.Category)) ModelState.AddModelError("Input.Category", "Category is required");

            if (!ModelState.IsValid)
            {
                var vm = new IdeasCreateViewModel { Input = input, Categories = DefaultCategories };
                return View(vm);
            }

            _repo.Add(input);
            return RedirectToAction("Index");
        }

        private static readonly string[] DefaultCategories = new[]
        {
            "Parks",
            "Transport",
            "Public Safety",
            "Sanitation",
            "Infrastructure",
            "Community Programs",
            "Environment",
            "Technology",
            "Accessibility",
            "Other"
        };
    }
}