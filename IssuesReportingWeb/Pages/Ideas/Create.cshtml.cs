using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;

namespace IssuesReportingWeb.Pages.Ideas;

public class CreateModel : PageModel
{
    private readonly IIdeaRepository _repo;
    public CreateModel(IIdeaRepository repo) => _repo = repo;

    [BindProperty]
    public ImprovementIdea Input { get; set; } = new ImprovementIdea();

    public IReadOnlyList<string> Categories { get; private set; } = Array.Empty<string>();

    public void OnGet()
    {
        Categories = DefaultCategories;
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Input.Title)) ModelState.AddModelError("Input.Title", "Title is required");
        if (string.IsNullOrWhiteSpace(Input.Description)) ModelState.AddModelError("Input.Description", "Description is required");
        if (string.IsNullOrWhiteSpace(Input.Category)) ModelState.AddModelError("Input.Category", "Category is required");

        if (!ModelState.IsValid)
        {
            Categories = DefaultCategories;
            return Page();
        }

        _repo.Add(Input);
        return RedirectToPage("Index");
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