using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Models;

namespace IssuesReportingWeb.Pages.Ideas;

public class IndexModel : PageModel
{
    private readonly IIdeaRepository _repo;
    public IndexModel(IIdeaRepository repo) => _repo = repo;

    public IEnumerable<ImprovementIdea> Items { get; private set; } = Enumerable.Empty<ImprovementIdea>();

    public void OnGet(string? q = null)
    {
        var all = _repo.GetAll();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var query = q.Trim().ToLowerInvariant();
            all = all.Where(i =>
                (i.Title ?? string.Empty).ToLowerInvariant().Contains(query) ||
                (i.SubmittedBy ?? string.Empty).ToLowerInvariant().Contains(query));
        }
        Items = all;
    }

    public IActionResult OnPostVote(int id)
    {
        _repo.Vote(id);
        return RedirectToPage();
    }
}