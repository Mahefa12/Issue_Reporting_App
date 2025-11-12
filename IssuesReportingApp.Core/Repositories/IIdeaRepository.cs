using IssuesReportingApp.Models;

namespace IssuesReportingApp.Repositories;

public interface IIdeaRepository
{
    IEnumerable<ImprovementIdea> GetAll();
    ImprovementIdea? GetById(int id);
    ImprovementIdea Add(ImprovementIdea idea);
    bool Update(ImprovementIdea idea);
    bool Vote(int id);
    bool Delete(int id);
}