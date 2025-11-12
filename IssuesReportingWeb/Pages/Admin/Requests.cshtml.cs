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
public class RequestsModel : PageModel
{
    private readonly IServiceRequestRepository _requestRepo;

    public RequestsModel(IServiceRequestRepository requestRepo)
    {
        _requestRepo = requestRepo;
    }

    public IReadOnlyList<ServiceRequest> Requests { get; private set; } = Array.Empty<ServiceRequest>();
    public int ReceivedCount { get; private set; }
    public int InProgressCount { get; private set; }
    public int WaitingOnExternalCount { get; private set; }
    public int CompletedCount { get; private set; }
    public int CancelledCount { get; private set; }

    public void OnGet() => Load();

    private void Load()
    {
        Requests = _requestRepo.GetAll().ToList();
        ReceivedCount = Requests.Count(r => r.Status == ServiceRequestStatus.Received);
        InProgressCount = Requests.Count(r => r.Status == ServiceRequestStatus.InProgress);
        WaitingOnExternalCount = Requests.Count(r => r.Status == ServiceRequestStatus.WaitingOnExternal);
        CompletedCount = Requests.Count(r => r.Status == ServiceRequestStatus.Completed);
        CancelledCount = Requests.Count(r => r.Status == ServiceRequestStatus.Cancelled);
    }

    public IActionResult OnPostUpdateRequest(string identifier, ServiceRequestStatus status)
    {
        var req = _requestRepo.GetByIdentifier(identifier);
        if (req != null)
        {
            req.Status = status;
            _requestRepo.Update(req);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostDeleteRequest(string identifier)
    {
        _requestRepo.Delete(identifier);
        return RedirectToPage();
    }

    public IActionResult OnPostDeleteCompletedRequests()
    {
        foreach (var r in _requestRepo.GetAll().Where(r => r.Status == ServiceRequestStatus.Completed || r.Status == ServiceRequestStatus.Cancelled).ToList())
        {
            _requestRepo.Delete(r.Identifier);
        }
        return RedirectToPage();
    }
}
#else
namespace IssuesReportingWeb.Pages.Admin
{
    // Safe stub for desktop build to avoid ASP.NET references
    public class RequestsModel { }
}
#endif