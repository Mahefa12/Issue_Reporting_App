// Build full ASP.NET Razor Page only when not targeting Windows desktop TFM.
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
public class IdeasModel : PageModel
{
    private readonly IIdeaRepository _ideaRepo;

    public IdeasModel(IIdeaRepository ideaRepo)
    {
        _ideaRepo = ideaRepo;
    }

    public IReadOnlyList<ImprovementIdea> Ideas { get; private set; } = Array.Empty<ImprovementIdea>();
    public int SubmittedCount { get; private set; }
    public int AcceptedCount { get; private set; }
    public int RejectedCount { get; private set; }

    public void OnGet() => Load();

    private void Load()
    {
        Ideas = _ideaRepo.GetAll().ToList();
        SubmittedCount = Ideas.Count(i => string.Equals(i.Status, "Submitted", StringComparison.OrdinalIgnoreCase));
        AcceptedCount = Ideas.Count(i => string.Equals(i.Status, "Accepted", StringComparison.OrdinalIgnoreCase));
        RejectedCount = Ideas.Count(i => string.Equals(i.Status, "Rejected", StringComparison.OrdinalIgnoreCase));
    }

    public IActionResult OnPostAcceptIdea(int id)
    {
        var idea = _ideaRepo.GetById(id);
        if (idea != null)
        {
            idea.Status = "Accepted";
            _ideaRepo.Update(idea);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostRejectIdea(int id)
    {
        var idea = _ideaRepo.GetById(id);
        if (idea != null)
        {
            idea.Status = "Rejected";
            _ideaRepo.Update(idea);
        }
        return RedirectToPage();
    }
}
#else
namespace IssuesReportingWeb.Pages.Admin
{
    // Safe stub for desktop build to avoid ASP.NET references
    public class IdeasModel { }
}
#endif