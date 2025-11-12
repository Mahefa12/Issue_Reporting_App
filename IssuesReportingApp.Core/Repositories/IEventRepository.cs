using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public interface IEventRepository
{
    IEnumerable<Event> GetAll();
    Event? GetById(int id);
    Event Add(Event ev);
    bool Update(Event ev);
    bool Delete(int id);
}