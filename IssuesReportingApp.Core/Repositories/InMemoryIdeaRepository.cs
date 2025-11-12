using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public class InMemoryIdeaRepository : IIdeaRepository
{
    private readonly List<ImprovementIdea> _ideas = new();
    private int _nextId = 1;

    public InMemoryIdeaRepository()
    {
    }

    public InMemoryIdeaRepository(IEnumerable<ImprovementIdea> seed)
    {
        if (seed != null)
        {
            foreach (var idea in seed)
            {
                idea.Id = _nextId++;
                _ideas.Add(idea);
            }
        }
    }

    public IEnumerable<ImprovementIdea> GetAll() => _ideas.OrderByDescending(i => i.Votes).ThenByDescending(i => i.CreatedDate);

    public ImprovementIdea? GetById(int id) => _ideas.FirstOrDefault(i => i.Id == id);

    public ImprovementIdea Add(ImprovementIdea idea)
    {
        idea.Id = _nextId++;
        idea.CreatedDate = DateTime.UtcNow;
        _ideas.Add(idea);
        return idea;
    }

    public bool Update(ImprovementIdea idea)
    {
        var existing = GetById(idea.Id);
        if (existing == null) return false;
        existing.Title = idea.Title;
        existing.Description = idea.Description;
        existing.Category = idea.Category;
        existing.SubmittedBy = idea.SubmittedBy;
        existing.Status = idea.Status;
        return true;
    }

    public bool Vote(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;
        existing.Votes++;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;
        return _ideas.Remove(existing);
    }
}