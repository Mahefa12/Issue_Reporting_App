using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> _events = new();
    private int _nextId = 1;

    public InMemoryEventRepository(IEnumerable<Event>? seed = null)
    {
        if (seed != null)
        {
            foreach (var e in seed)
            {
                e.Id = _nextId++;
                _events.Add(e);
            }
        }
    }

    public IEnumerable<Event> GetAll() => _events.OrderByDescending(e => e.Priority).ThenBy(e => e.StartDate);

    public Event? GetById(int id) => _events.FirstOrDefault(e => e.Id == id);

    public Event Add(Event ev)
    {
        ev.Id = _nextId++;
        ev.CreatedDate = DateTime.Now;
        _events.Add(ev);
        return ev;
    }

    public bool Update(Event ev)
    {
        var existing = GetById(ev.Id);
        if (existing == null) return false;
        existing.Title = ev.Title;
        existing.Description = ev.Description;
        existing.StartDate = ev.StartDate;
        existing.EndDate = ev.EndDate;
        existing.Location = ev.Location;
        existing.Category = ev.Category;
        existing.Organizer = ev.Organizer;
        existing.Priority = ev.Priority;
        existing.IsAnnouncement = ev.IsAnnouncement;
        existing.ContactInfo = ev.ContactInfo;
        existing.TicketPrice = ev.TicketPrice;
        existing.MaxAttendees = ev.MaxAttendees;
        existing.CurrentAttendees = ev.CurrentAttendees;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;
        return _events.Remove(existing);
    }
}