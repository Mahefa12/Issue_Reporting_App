using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using IssuesReportingApp.Models;
using IssuesReportingApp.Services;
using IssuesReportingApp.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Ensure HTTP/HTTPS endpoints and allow override via ASPNETCORE_URLS
builder.WebHost.UseKestrel();
var urlsEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
var urls = string.IsNullOrWhiteSpace(urlsEnv)
    ? "http://localhost:5000;https://localhost:5001"
    : urlsEnv;
builder.WebHost.UseUrls(urls);

// Enable MVC controllers/views (pure MVC setup)
builder.Services.AddControllersWithViews();

// Cookie authentication for simple admin login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = "IssuesReportingAuth";
    });
// Authorization: protect only the Admin area; public pages remain accessible
builder.Services.AddAuthorization();

// Register core services and seed datasets
var seededPrefs = Seed.Preferences();
var seededHistory = Seed.History();
builder.Services.AddSingleton<UserPreferences>(seededPrefs);
builder.Services.AddSingleton<SearchHistory>(seededHistory);

// Seed events and announcements; share with event repository and recommendation engine
var seededEvents = Seed.Events(28);
builder.Services.AddSingleton<IEventRepository>(sp => new InMemoryEventRepository(seededEvents));
builder.Services.AddScoped<RecommendationEngine>(sp =>
{
    var prefs = sp.GetRequiredService<UserPreferences>();
    var history = sp.GetRequiredService<SearchHistory>();
    // Disambiguate repository method resolution by using the concrete implementation
    var eventRepoConcrete = (InMemoryEventRepository)sp.GetRequiredService<IEventRepository>();
    var events = eventRepoConcrete.GetAll().ToList();
    return new RecommendationEngine(prefs, history, events);
});

// In-memory repositories for Issues, Ideas, and Service Requests with substantial seed data
var seededIssues = Seed.Issues(36);
var seededIdeas = Seed.Ideas(24);
var seededRequests = Seed.ServiceRequests(40);
builder.Services.AddSingleton<IIssueRepository>(sp => new InMemoryIssueRepository(seededIssues));
builder.Services.AddSingleton<IIdeaRepository>(sp => new InMemoryIdeaRepository(seededIdeas));
builder.Services.AddSingleton<IServiceRequestRepository>(sp => new InMemoryServiceRequestRepository(seededRequests));
builder.Services.AddSingleton<ServiceRequestIndex>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redirect HTTP -> HTTPS when HTTPS endpoint is available
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Stub handler to silence dev-tool requests for Vite client (not used here)
app.MapGet("/@vite/client", (HttpResponse res) =>
{
    res.ContentType = "application/javascript";
    return "/* Vite client not in use; returning empty stub */";
});

// Conventional MVC route only
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static class Seed
{
    public static UserPreferences Preferences()
    {
        var prefs = new UserPreferences();
        // Seed some interactions across categories and locations
        var categories = new[] { "Community", "Infrastructure", "Health", "Education", "Culture", "Sanitation" };
        var locations = new[] { "Pretoria", "Cape Town", "Durban", "Johannesburg", "Bloemfontein", "Gqeberha" };
        foreach (var c in categories)
        {
            prefs.RecordCategoryInteraction(c, Random.Shared.Next(3, 8));
        }
        foreach (var l in locations)
        {
            prefs.RecordLocationInteraction(l, Random.Shared.Next(2, 6));
        }
        prefs.RecordTimePreference(DateTime.Now.AddDays(-7));
        prefs.RecordTimePreference(DateTime.Now);
        return prefs;
    }

    public static SearchHistory History()
    {
        var history = new SearchHistory();
        var pastQueries = new[]
        {
            "streetlight repair", "waste collection", "road closure notice", "community clean-up",
            "tech fair", "park yoga", "pothole", "water outage", "town hall"
        };
        foreach (var q in pastQueries)
        {
            var when = DateTime.Now.AddDays(-Random.Shared.Next(1, 30));
            history.AddSearch(new SearchEntry(q) { Timestamp = when });
        }
        return history;
    }

    public static List<Event> Events(int count)
    {
        var now = DateTime.Now;
        var categories = new[] { "Community", "Culture", "Education", "Health", "Infrastructure", "Public Service" };
        var locations = new[] { "Pretoria", "Cape Town", "Durban", "Johannesburg", "Bloemfontein", "Gqeberha", "Polokwane", "City Hall" };
        var baseEvents = new List<Event>
        {
            new Event("Community Clean-up Day", "Join residents to clean local park.", now.AddDays(5), now.AddDays(5).AddHours(2), "Pretoria", "Community", "City Council", 1){ IsAnnouncement = false },
            new Event("Food Market", "Local vendors and live music.", now.AddDays(10), now.AddDays(10).AddHours(6), "Cape Town", "Culture", "Community Org", 2),
            new Event("Tech Fair", "Showcase startups and workshops.", now.AddDays(20), now.AddDays(20).AddHours(8), "Durban", "Education", "Tech Hub", 1),
            new Event("Park Yoga", "Free yoga session.", now.AddDays(2), now.AddDays(2).AddHours(1), "Polokwane", "Health", "Wellness SA", 3),
            new Event("Art Exhibition", "Contemporary SA artists.", now.AddDays(30), now.AddDays(30).AddHours(4), "Johannesburg", "Culture", "Art Council", 2),
            new Event("Water Service Interruption", "Scheduled maintenance will cause a temporary water outage.", now.AddDays(1), now.AddDays(1).AddHours(4), "Gqeberha", "Public Service", "Municipal Services", 1){ IsAnnouncement = true },
            new Event("Road Closure Notice", "Main Street closed for resurfacing. Use alternate routes.", now.AddDays(3), now.AddDays(3).AddHours(8), "Bloemfontein", "Infrastructure", "Public Works", 1){ IsAnnouncement = true },
            new Event("Town Hall Meeting", "Community briefing on upcoming service changes.", now.AddDays(2), now.AddDays(2).AddHours(2), "City Hall", "Community", "City Council", 1){ IsAnnouncement = true },
        };

        var list = new List<Event>(baseEvents);
        var eventTitles = new (string Title, string Category)[]
        {
            ("Neighborhood Garden Day", "Community"),
            ("Local Music Night", "Culture"),
            ("Coding Workshop", "Education"),
            ("Health Screening Fair", "Health"),
            ("Bike Safety Ride", "Community"),
            ("Art in the Park", "Culture"),
            ("Book Donation Drive", "Education"),
            ("Wellness Walk", "Health"),
        };
        var announcementTitles = new (string Title, string Category)[]
        {
            ("Power Maintenance Window", "Public Service"),
            ("Traffic Diversion Advisory", "Infrastructure"),
            ("Water Pipe Replacement Schedule", "Public Service"),
            ("Bridge Inspection Notice", "Infrastructure"),
            ("Library Renovation Closure", "Public Service"),
        };
        for (int i = 0; i < count - baseEvents.Count; i++)
        {
            var start = now.AddDays(4 + i);
            var end = start.AddHours(2 + (i % 6));
            var isAnnouncement = i % 5 == 0; // every 5th item is an announcement
            var tpl = isAnnouncement
                ? announcementTitles[i % announcementTitles.Length]
                : eventTitles[i % eventTitles.Length];
            var cat = tpl.Category;
            var title = tpl.Title;
            var loc = locations[i % locations.Length];
            var prio = 1 + (i % 3);
            list.Add(new Event(title, $"Auto-generated {cat} event.", start, end, loc, cat, "City Council", prio)
            {
                IsAnnouncement = isAnnouncement,
                MaxAttendees = 100 + i * 5,
                CurrentAttendees = 20 + i * 2,
                TicketPrice = isAnnouncement ? null : (decimal?)(20 + i),
                ContactInfo = "info@city.gov",
            });
        }
        // Assign sequential IDs for deterministic ordering in repo constructor
        for (int i = 0; i < list.Count; i++) list[i].Id = i + 1;
        return list;
    }

    public static List<Issue> Issues(int count)
    {
        var categories = new[] { "Infrastructure", "Sanitation", "Health", "Community", "Safety" };
        var priorities = new[] { "High", "Medium", "Low" };
        var statuses = new[] { "Received", "InProgress", "Waiting", "Completed" };
        var titles = new[]
        {
            "Pothole on Main Road",
            "Streetlight Not Working",
            "Water Leak Near School",
            "Broken Traffic Light",
            "Overflowing Trash Bin",
            "Noise Complaint Downtown",
            "Damaged Park Bench",
            "Graffiti Cleanup Request",
            "Illegal Dumping in Alley",
            "Cracked Sidewalk Repair",
            "Blocked Drainage on Oak St",
            "Road Sign Missing",
        };
        var list = new List<Issue>();
        for (int i = 1; i <= count; i++)
        {
            var title = titles[(i - 1) % titles.Length];
            var category = categories[i % categories.Length];
            list.Add(new Issue
            {
                Title = title,
                Description = $"Report: {title.ToLower()}.",
                Category = category,
                Priority = priorities[i % priorities.Length],
                Status = statuses[i % statuses.Length],
                ReporterName = $"Reporter {i}",
                ContactInfo = $"reporter{i}@example.com",
                CreatedDate = DateTime.UtcNow.AddDays(-i)
            });
        }
        return list;
    }

    public static List<ImprovementIdea> Ideas(int count)
    {
        var categories = new[] { "Parks", "Transport", "Digital", "Community", "Education" };
        var statuses = new[] { "Submitted", "Accepted" };
        var list = new List<ImprovementIdea>();
        var ideaTitles = new[]
        {
            "Add Bike Lanes on Main",
            "Upgrade Park Lighting",
            "Launch City Services Mobile App",
            "Install Recycling Bins Downtown",
            "Create After-school Tutoring Program",
            "Build Community Garden Beds",
            "Expand Bus Route Coverage",
            "Add Safe Pedestrian Crossings",
            "Digitize Permit Applications",
            "Start Weekend Coding Clubs",
            "Install Water-saving Irrigation",
            "Introduce Car-free Sundays",
        };
        for (int i = 1; i <= count; i++)
        {
            var title = ideaTitles[(i - 1) % ideaTitles.Length];
            list.Add(new ImprovementIdea
            {
                Title = title,
                Description = $"Auto-generated idea description {i}.",
                Category = categories[i % categories.Length],
                SubmittedBy = $"User {i}",
                CreatedDate = DateTime.UtcNow.AddDays(-i),
                Votes = 5 * (i % 10),
                Status = statuses[i % statuses.Length]
            });
        }
        return list;
    }

    public static List<ServiceRequest> ServiceRequests(int count)
    {
        var categories = new[] { "Infrastructure", "Sanitation", "Procurement", "Community", "Health" };
        var statuses = new[] { ServiceRequestStatus.Received, ServiceRequestStatus.InProgress, ServiceRequestStatus.WaitingOnExternal, ServiceRequestStatus.Completed };
        var titles = new[]
        {
            "Streetlight Repair on Main St",
            "Waste Collection Delay in Zone 5",
            "Water Outage in Riverside",
            "Pothole Repair on Elm Ave",
            "Traffic Light Malfunction at 3rd & Pine",
            "Park Vandalism Report",
            "Road Closure Coordination",
            "Tree Trimming Request",
            "Noise Complaint Investigation",
            "Public Wi-Fi Outage Near Library",
            "Sidewalk Crack Fix on Oak St",
            "Drainage Blockage on Willow Ln",
        };
        var list = new List<ServiceRequest>();
        for (int i = 1; i <= count; i++)
        {
            // Use a high offset to avoid collisions with identifiers derived from Issue.Id
            var id = $"ISSUE-{1000 + i}";
            var title = titles[(i - 1) % titles.Length];
            var created = DateTime.UtcNow.AddDays(-i);
            // Vary updated date: sometimes null, sometimes a few days after creation
            DateTime? updated = (i % 3 == 0) ? null : created.AddHours(Random.Shared.Next(6, 96));
            list.Add(new ServiceRequest
            {
                Identifier = id,
                Title = title,
                Description = $"Request: {title.ToLower()}.",
                Category = categories[i % categories.Length],
                Priority = 1 + (i % 3),
                Status = statuses[i % statuses.Length],
                CreatedDate = created,
                UpdatedDate = updated,
                Dependencies = new List<string>()
            });
        }
        // Create dependency edges forming small components and chains
        for (int i = 1; i <= count; i++)
        {
            if (i % 4 == 0 && i + 1 <= count)
            {
                list[i - 1].Dependencies.Add($"ISSUE-{1000 + (i + 1)}");
            }
            if (i % 7 == 0 && i + 2 <= count)
            {
                list[i - 1].Dependencies.Add($"ISSUE-{1000 + (i + 2)}");
            }
            if (i % 10 == 0 && i - 3 >= 1)
            {
                list[i - 1].Dependencies.Add($"ISSUE-{1000 + (i - 3)}");
            }
        }
        return list;
    }
}
