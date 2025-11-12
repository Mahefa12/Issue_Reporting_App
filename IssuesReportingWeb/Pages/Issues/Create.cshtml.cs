using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;

namespace IssuesReportingWeb.Pages.Issues;

public class CreateModel : PageModel
{
    private readonly IIssueRepository _repo;
    public CreateModel(IIssueRepository repo) => _repo = repo;

    [BindProperty]
    public Issue Input { get; set; } = new Issue();

    public void OnGet() { }

    public IActionResult OnPost()
    {
        // Ensure optional fields are safely defaulted when left blank
        Input.Category ??= string.Empty;
        Input.ReporterName ??= string.Empty;
        Input.ContactInfo ??= string.Empty;

        if (string.IsNullOrWhiteSpace(Input.Title)) ModelState.AddModelError("Input.Title", "Title is required");
        if (string.IsNullOrWhiteSpace(Input.Description)) ModelState.AddModelError("Input.Description", "Description is required");
        if (string.IsNullOrWhiteSpace(Input.Category)) ModelState.AddModelError("Input.Category", "Category is required");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Force default status to Received irrespective of posted value
        Input.Status = "Received";
        Input.Priority = string.IsNullOrWhiteSpace(Input.Priority) ? "Medium" : Input.Priority;
        var created = _repo.Add(Input);
        TempData["Success"] = $"Report submitted: #{created.Id} — {created.Title}";
        return RedirectToPage("Index");
    }
}