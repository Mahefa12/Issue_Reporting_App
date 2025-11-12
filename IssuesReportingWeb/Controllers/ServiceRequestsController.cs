using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IssuesReportingApp.Models;
using IssuesReportingApp.Repositories;
using IssuesReportingApp.Services;

namespace IssuesReportingWeb.Controllers
{
    public class ServiceRequestsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IssuesReportingApp.Services.ServiceRequestIndex _index;
        private readonly IssuesReportingApp.Repositories.IIssueRepository _issues;

        public ServiceRequestsController(IssuesReportingApp.Services.ServiceRequestIndex index, IssuesReportingApp.Repositories.IIssueRepository issues)
        {
            _index = index;
            _issues = issues;
        }

        public class ServiceRequestsViewModel
        {
            public string? IdentifierQuery { get; set; }
            public string? SearchQuery { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public List<IssuesReportingApp.Models.ServiceRequest> Items { get; set; } = new();
            public IssuesReportingApp.Models.ServiceRequest? TrackedRequest { get; set; }
            public List<string>? DependencyPath { get; set; }
            public List<IssuesReportingApp.Models.ServiceRequest> NextUp { get; set; } = new();
            public List<(string u, string v)>? DependencyBackbone { get; set; }
            public List<IssuesReportingApp.Models.ServiceRequest> Alphabetical { get; set; } = new();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public Microsoft.AspNetCore.Mvc.IActionResult Index(string? identifierQuery, string? searchQuery, DateTime? startDate, DateTime? endDate)
        {
            var vm = new ServiceRequestsViewModel
            {
                IdentifierQuery = identifierQuery,
                SearchQuery = searchQuery,
                StartDate = startDate,
                EndDate = endDate
            };

            IEnumerable<IssuesReportingApp.Models.ServiceRequest> fromRequests = _index.All();
            if (startDate.HasValue || endDate.HasValue)
            {
                var start = startDate?.Date ?? DateTime.MinValue;
                // Make the end bound inclusive through end-of-day
                var end = (endDate?.Date ?? DateTime.MaxValue).AddDays(1).AddTicks(-1);
                fromRequests = _index.RangeByCreatedDate(start, end);
            }
            var reqList = fromRequests.ToList();
            var fromIssues = _issues.GetAll().Select(IssuesReportingApp.Services.RequestIssueMapper.ToServiceRequest);
            vm.Items = reqList
                .Concat(fromIssues)
                .OrderBy(r => r.Priority)
                .ThenByDescending(r => r.UpdatedDate ?? r.CreatedDate)
                .ToList();
            vm.NextUp = _index.NextToProcess(5);
            vm.Alphabetical = _index.Alphabetical(10).ToList();

            var query = (searchQuery ?? identifierQuery)?.Trim();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var match = vm.Items.FirstOrDefault(r =>
                    string.Equals(r.Identifier, query, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(r.Title) && r.Title.Contains(query, StringComparison.OrdinalIgnoreCase)));

                if (match != null)
                {
                    vm.TrackedRequest = match;
                    var firstDep = match.Dependencies?.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(firstDep))
                    {
                        vm.DependencyPath = _index.DependencyPath(match.Identifier, firstDep);
                    }
                    vm.DependencyBackbone = _index.DependencyMstForComponent(match.Identifier);
                }
            }

            return View(vm);
        }
    }
}
